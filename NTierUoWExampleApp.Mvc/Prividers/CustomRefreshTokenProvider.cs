using Microsoft.Owin.Security.Infrastructure;
using NTierUoWExampleApp.Core.Services;
using NTierUoWExampleApp.DAL.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NTierUoWExampleApp.Mvc.Prividers
{
    public class CustomRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientAppId = context.Ticket.Properties.Dictionary["as:client_app_id"];

            if (string.IsNullOrEmpty(clientAppId))
            {
                return;
            }

            //We are generating a unique identifier for the refresh token
            var refreshTokenId = Guid.NewGuid().ToString("n");

            WebApiService service = new WebApiService();

            var client = service.FindClientByAppId(clientAppId);

            /*
             * We are reading the refresh token life time value from the Owin
             * context where we set this value once we validate the client,
             * this value will be used to determine how long the refresh token
             * will be valid for, this should be in minutes.
             */
            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken()
            {
                RefreshTokenId = Core.Utility.Authentication.AuthHelper.GetHash(refreshTokenId),
                ClientAppId = clientAppId,
                Name = context.Ticket.Identity.Name ?? client.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };


            /*
             * we are setting the IssuedUtc, and ExpiresUtc values for the ticket,
             * setting those properties will determine how long the refresh token will be valid for.
             */
            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            // serialize the ticket content
            token.ProtectedTicket = context.SerializeTicket();

            // save record in RefreshTokens table
            /*
             * We are checking that the token which will be saved on the database is unique
             * for this User and the Client, if it not unique we’ll delete the existing one
             * and store new refresh token.
             */
            var result = await service.AddRefreshToken(token);

            if (result)
            {
                //send back the refresh token id
                context.SetToken(refreshTokenId);
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            /*
             * We need to set this header in this method because the method “GrantResourceOwnerCredentials”
             * where we set this header is never get executed once we request access token using refresh
             * tokens (grant_type=refresh_token).
             */
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });


            /*
             * We get the refresh token id from the request, then hash this id and look for this token
             * using the hashed refresh token id in table “RefreshTokens”, if the refresh token is found,
             * we will use the magical signed string which contains a serialized representation for the
             * ticket to build the ticket and identities for the user mapped to this refresh token.
             */
            string hashedTokenId = Core.Utility.Authentication.AuthHelper.GetHash(context.Token);

            WebApiService service = new WebApiService();

            var refreshToken = await service.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);

                /*
                 * We’ll remove the existing refresh token from tables “RefreshTokens”
                 * because in our logic we are allowing only one refresh token per user and client.
                 */
                await service.RemoveRefreshToken(hashedTokenId);
            }
        }
    }
}