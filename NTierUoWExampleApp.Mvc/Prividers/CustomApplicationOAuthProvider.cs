using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using NTierUoWExampleApp.Common.Enum.Authentication;
using NTierUoWExampleApp.Common.Enum.User;
using NTierUoWExampleApp.Core.Services;
using NTierUoWExampleApp.DAL.Models.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace NTierUoWExampleApp.Mvc.Prividers
{
    public class CustomApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private WebApiService service;
        public CustomApplicationOAuthProvider()
        {
            service = new WebApiService();
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;

            /*
             * We are trying to get the Client id and secret from the authorization header
             * using a basic scheme so one way to send the client_id/client_secret is to base64
             * encode the (client_id:client_secret) and send it in the Authorization header.
             * The other way is to sent the client_id/client_secret as “x-www-form-urlencoded”.
             * This way both options are supported.
             */

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                //context.Validated();
                context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            //Get client by id
            try
            {
                client = service.FindClientByAppId(context.ClientId);
            }
            catch(Exception e)
            {

            }
            


            if (client == null)
            {
                //we need to check our database if the client is registered
                context.SetError("client_not_registered", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }


            /* If its confidential app then the client secret is mandatory and it will
             * be validated against the secret stored in the database.
             */
            if (client.WebApiApplicationDataAccessTypes == WebApiApplicationDataAccessTypes.Confidential.ToString())
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientSecret", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    if (client.Secret != Core.Utility.Authentication.AuthHelper.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientSecret", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            //check if the client is active
            if (client.Status != Common.Enum.Authentication.ClientStatusEnum.Active.ToString())
            {
                context.SetError("client_not_active", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            //check if client is internal or global
            /*
             *  if it's global dont check for user because its not in our database
             */
            if (client.WebApiApplicationTypes == WebApiApplicationTypes.Global.ToString())
            {
                if (client.AllowedOrigin != "*" && context.Request.Uri.Host != client.AllowedOrigin)
                {
                    context.SetError("invalid_host", "Client allowed origin is invalid.");
                    return Task.FromResult<object>(null);
                }

                context.OwinContext.Set<string>("clientType", WebApiApplicationTypes.Global.ToString());
                context.OwinContext.Set<string>("clientAppId", client.ClientAppId);
            }
            else
            {
                context.OwinContext.Set<string>("clientType", WebApiApplicationTypes.Internal.ToString());
                context.OwinContext.Set<string>("clientAppId", client.ClientAppId);
            }

            /*
             * We need to store the client allowed origin and refresh token life time
             * value on the Owin context so it will be available once we generate the
             * refresh token and set its expiry life time.
             */
            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());
            context.Options.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(client.AccessTokenLifeTime);

            /*
             * If all is valid we mark the context as valid context which means that
             * client check has passed and the code flow can proceed to the next step.
             */
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            AccountService service = new AccountService(Startup.DataProtectionProvider);

            /*
             * Reading the allowed origin value for this client from the Owin context,
             * then we use this value to add the header “Access-Control-Allow-Origin”
             * to Owin context response, by doing this and for any JavaScript application
             * we’ll prevent using the same client id to build another JavaScript application
             * hosted on another domain; because the origin for all requests coming from
             * this app will be from a different domain and the back-end API will return 405 status.
             */
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            if (allowedOrigin == null) allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            //get client type
            var clientType = context.OwinContext.Get<string>("clientType");

            //get client app id
            string clientAppId = string.Empty;
            string clientName = string.Empty;


            //check the username, password and role for the user if client type is internal
            AuthenticationTicket ticket = null;
            //string userRole = string.Empty;
            //IdentityUser user;
            DAL.Models.Account.User user;

            if (clientType == WebApiApplicationTypes.Internal.ToString())
            {
                if (context.UserName == null || context.Password == null)
                {
                    context.SetError("invalid_grant", "The username and password are required.");
                    return;
                }

                user = await service.GetByUsernameAndPassword(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("username_or_password_incorect", "The user name or password is incorrect.");
                    return;
                }

                if (user.PermanentLock)
                {
                    context.SetError("account_locked", "This account has been locked.");
                    return;
                }

                //get user system role
                var systemRole = service.GetSystemRoleForUser(user.Id, true);
                if (systemRole != "Administrator")
                {
                    context.SetError("forbidden", "You don't have permission to access this application.");
                    return;
                }

                if (user.Status == UserStatusEnum.Inactive.ToString())
                {
                    context.SetError("inactive_user", "The user is not active.");
                    return;
                }



                try
                {
                    clientAppId = context.OwinContext.Get<string>("clientAppId");
                    if (clientAppId != null && clientAppId != string.Empty)
                    {
                        WebApiService webApiService = new WebApiService(service.UnitOfWork);

                        //Get client by id
                        try
                        {
                            var client = webApiService.FindClientByAppId(context.ClientId);
                            clientName = client.Name;
                        }
                        catch (Exception e)
                        {
                            context.SetError("client_not_registered", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                            return;
                        }
                        
                    }
                }
                catch (Exception e)
                {

                }


                //generate set of claims for this user along with authentication properties
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("Id", user.Id));
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identity.AddClaim(new Claim("ClientType", WebApiApplicationTypes.Internal.ToString()));
                identity.AddClaim(new Claim("ClientAppId", context.ClientId));

                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_app_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", context.UserName
                    }
                });

                ticket = new AuthenticationTicket(identity, props);
            }
            else
            {
                //generate set of claims for this user along with authentication properties
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("ClientType", WebApiApplicationTypes.Global.ToString()));

                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_app_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    }
                });

                ticket = new AuthenticationTicket(identity, props);
            }

            //by calling this method access token will be generated
            context.Validated(ticket);

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            /*
             * We are reading the client id value from the original ticket, this is the client id
             * which get stored in the magical signed string, then we compare this client id against
             * the client id sent with the request, if they are different we’ll reject this request
             * because we need to make sure that the refresh token used here is bound to the same
             * client when it was generated.
             */
            var originalClient = context.Ticket.Properties.Dictionary["as:client_app_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_refresh_token", "Refresh token is issued to a different client.");
                return Task.FromResult<object>(null);
            }

            
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);

            context.Validated(newTicket);

            /*
             * the flow for the code will hit method “CreateAsync” in class “CustomRefreshTokenProvider”
             * and a new refresh token is generated and returned in the response along with the new access token. 
             */
            return Task.FromResult<object>(null);
        }
    }
}