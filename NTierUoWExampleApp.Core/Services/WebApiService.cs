using NTierUoWExampleApp.Core.BindingModels.Account;
using NTierUoWExampleApp.Core.BindingModels.JqGrid;
using NTierUoWExampleApp.Core.BindingModels.WebApi;
using NTierUoWExampleApp.Core.Utility.Authentication;
using NTierUoWExampleApp.Core.Utility.JqGrid;
using NTierUoWExampleApp.DAL.Models.Authentication;
using NTierUoWExampleApp.DAL.UnitOfWork;
using NTierUoWExampleApp.DAL.UnitOfWork.IUoW;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Services
{
    public class WebApiService
    {
        private IUnitOfWork unitOfWork;
        private ApplicationLogger logger;
        public WebApiService()
        {
            unitOfWork = new AppUnitOfWork();
            logger = new ApplicationLogger(unitOfWork);
        }

        public WebApiService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            logger = new ApplicationLogger(unitOfWork);
        }

        public IUnitOfWork UnitOfWork
        {
            get { return this.unitOfWork; }
        }


        #region web
        public JqGridResultViewModel<ClientViewModel> GetWebApiClients(JqGridSearchModel jqGridSearchModel)
        {
            try
            {
                if (jqGridSearchModel != null)
                {
                    List<ClientViewModel> model = new List<ClientViewModel>();

                    JqGridPaging<DAL.Models.Authentication.Client, ClientViewModel> JqGridPaging = new JqGridPaging<DAL.Models.Authentication.Client, ClientViewModel>(unitOfWork.ClientRepository.GetAllQueryable(), "ClientId", jqGridSearchModel);
                    var clients = JqGridPaging.GetResult();

                    if (clients != null)
                    {
                        foreach (var client in clients)
                        {
                            model.Add(new ClientViewModel(client));
                        }
                    }

                    return JqGridPaging.GetJqGridResult(model);
                }
                return null;
            }
            catch (Exception e)
            {
                LogError("GetWebApiClients", e);
                Trace.TraceError(string.Format("GetWebApiClients in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task CreateClient(CreateClientViewModel model, string userId, string username)
        {
            try
            {
                var tempApp = unitOfWork.ClientRepository.GetAllQueryable()
                     .Where(x => x.ClientAppId == model.ClientAppId)
                     .ToList();

                if (tempApp != null && tempApp.Count > 0)
                {
                    throw new ValidationException("Aplication with this Client app id already exist.");
                }


                DAL.Models.Authentication.Client client = new DAL.Models.Authentication.Client();

                client.ClientAppId = model.ClientAppId;
                client.AllowedOrigin = model.AllowedOrigin;
                client.CreatedById = userId;
                client.CreatedByUsername = username;
                client.DateCreated = DateTime.UtcNow;
                client.Name = model.Name;
                client.RefreshTokenLifeTime = model.RefreshTokenLifeTime;
                client.AccessTokenLifeTime = model.AccessTokenLifeTime;
                client.Secret = AuthHelper.GetHash(model.Secret);
                client.Status = model.Status.ToString(); ;
                client.WebApiApplicationDataAccessTypes = model.WebApiApplicationDataAccessTypes.ToString();
                client.WebApiApplicationTypes = model.WebApiApplicationTypes.ToString();

                unitOfWork.ClientRepository.Create(client);
                await unitOfWork.SaveChangesAsync();

            }
            catch (Exception e)
            {
                await LogError("CreateClient", e);
                Trace.TraceError("CreateClient in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public async Task DeleteClient(Guid clientId, string userId, string username)
        {
            try
            {
                var client = await unitOfWork.ClientRepository.FindByIdAsync(clientId);
                if (client == null) throw new ValidationException("Client not found.");

                unitOfWork.ClientRepository.Delete(client);
                await unitOfWork.SaveChangesAsync();

            }
            catch (Exception e)
            {
                await LogError("DeleteClient", e);
                Trace.TraceError("DeleteClient in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public async Task<DAL.Models.Authentication.Client> GetClientByIdAsync(Guid clientId)
        {
            try
            {
                var client = await unitOfWork.ClientRepository.FindByIdAsync(clientId);
                if (client == null) throw new ValidationException("Client not found.");
                return client;
            }
            catch (Exception e)
            {
                await LogError("GetClientByIdAsync", e);
                Trace.TraceError("GetClientByIdAsync in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public async Task EditClient(EditClientViewModel model, string userId, string username)
        {
            try
            {
                var client = await GetClientByIdAsync(model.ClientId);

                client.ClientAppId = model.ClientAppId;
                client.AllowedOrigin = model.AllowedOrigin;
                client.Name = model.Name;
                client.RefreshTokenLifeTime = model.RefreshTokenLifeTime;
                client.AccessTokenLifeTime = model.AccessTokenLifeTime;

                if (model.ChangeSecret && !string.IsNullOrEmpty(model.Secret))
                {
                    client.Secret = AuthHelper.GetHash(model.Secret);
                }

                client.Status = model.Status.ToString();
                client.WebApiApplicationDataAccessTypes = model.WebApiApplicationDataAccessTypes.ToString();
                client.WebApiApplicationTypes = model.WebApiApplicationTypes.ToString();

                unitOfWork.ClientRepository.Update(client);
                await unitOfWork.SaveChangesAsync();

            }
            catch (Exception e)
            {
                await LogError("EditClient", e);
                Trace.TraceError("EditClient in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public JqGridResultViewModel<RefreshTokenViewModel> GetClientUsersRefreshTokens(Guid clientId, JqGridSearchModel jqGridSearchModel = null)
        {
            try
            {
                if (jqGridSearchModel != null)
                {
                    List<RefreshTokenViewModel> model = new List<RefreshTokenViewModel>();

                    JqGridPaging<RefreshToken, RefreshTokenViewModel> JqGridPaging = new JqGridPaging<RefreshToken, RefreshTokenViewModel>(unitOfWork.RefreshTokenRepository.GetAllQueryable(), "RefreshTokenId", jqGridSearchModel, "ClientId", clientId);
                    var refreshTokens = JqGridPaging.GetResult();

                    if (refreshTokens != null)
                    {
                        foreach (var token in refreshTokens)
                        {
                            model.Add(new RefreshTokenViewModel(token));
                        }
                    }

                    return JqGridPaging.GetJqGridResult(model);
                }
                return null;
            }
            catch (Exception e)
            {
                LogError("GetClientUsersRefreshTokens", e);
                Trace.TraceError("GetClientUsersRefreshTokens in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public async Task RevokeUser(string refreshTokenId, string userId, string username)
        {
            try
            {
                var token = await unitOfWork.RefreshTokenRepository.FindByIdAsync(refreshTokenId);
                if (token == null) throw new ValidationException("Token not found.");

                unitOfWork.RefreshTokenRepository.Delete(token);
                await unitOfWork.SaveChangesAsync();

            }
            catch (Exception e)
            {
                await LogError("RevokeUser", e);
                Trace.TraceError("RevokeUser in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public DAL.Models.Authentication.Client FindClientByAppId(string clientAppId)
        {
            try
            {
                var client = (from c in unitOfWork.ClientRepository.GetAllQueryable()
                              .Where(x => x.ClientAppId == clientAppId)
                              select c).ToList();

                if (client == null || client.Count == 0) throw new ValidationException("Client not found.");
                return client.FirstOrDefault();
            }
            catch (Exception e)
            {
                LogError("FindClientByAppId", e);
                Trace.TraceError("FindClientByAppId in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            try
            {
                var client = FindClientByAppId(token.ClientAppId);

                if (client == null) throw new ValidationException("Client not found.");

                token.ClientId = client.ClientId;

                //check if token for this user and client already exist, if yes then delete it first
                var tempToken = (from t in unitOfWork.RefreshTokenRepository.GetAllQueryable()
                                 .Where(x => x.ClientId == client.ClientId && x.Name == token.Name)
                                 select t).ToList();

                if (tempToken != null && tempToken.Count > 0)
                {
                    foreach (var t in tempToken)
                    {
                        unitOfWork.RefreshTokenRepository.Delete(t);
                        await unitOfWork.SaveChangesAsync();
                    }
                }


                unitOfWork.RefreshTokenRepository.Create(token);
                return await unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                await LogError("AddRefreshToken", e);
                Trace.TraceError("AddRefreshToken in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            try
            {
                return await unitOfWork.RefreshTokenRepository.FindByIdAsync(refreshTokenId);
            }
            catch (Exception e)
            {
                LogError("FindRefreshToken", e);
                Trace.TraceError("FindRefreshToken in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public async Task RemoveRefreshToken(string refreshTokenId)
        {
            try
            {
                var token = await unitOfWork.RefreshTokenRepository.FindByIdAsync(refreshTokenId);
                unitOfWork.RefreshTokenRepository.Delete(token);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                LogError("RemoveRefreshToken", e);
                Trace.TraceError("RemoveRefreshToken in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        public bool IsUserTokenRevoked(string username, string clientAppId)
        {
            try
            {
                var token = unitOfWork.RefreshTokenRepository.GetAllQueryable()
                    .Where(x => x.Name == username && x.ClientAppId == clientAppId)
                    .ToList();
                return token == null || token.Count == 0;
                
            }
            catch (Exception e)
            {
                LogError("IsUserTokenRevoked", e);
                Trace.TraceError("IsUserTokenRevoked in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        #endregion

        #region api
        public async Task<List<UserViewModel>> GetUsers()
        {
            try
            {
                var users = await unitOfWork.UserRepository.GetAllAsync();

                if (users == null || users.Count == 0) throw new ValidationException("Users not found.");

                List<UserViewModel> model = new List<UserViewModel>();

                foreach(var user in users)
                {
                    model.Add(new UserViewModel(user));
                }

                return model;
            }
            catch(Exception e)
            {
                await LogError("GetUsers", e);
                Trace.TraceError("GetUsers in webapi service error : {0}", e.Message);
                throw e;
            }
        }
        #endregion

        #region logger
        public async Task LogError(string errorMethodName, Exception e)
        {
            try
            {
                string innerExceptionMessage = e.InnerException != null ? e.InnerException.Message : null;
                await logger.LogError(errorMethodName, Common.Enum.Logger.LogTypeEnum.Error.ToString(), e.Message, innerExceptionMessage, e.StackTrace);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}
