using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.WebApi
{
    public class ClientViewModel
    {
        public ClientViewModel()
        {

        }
        public ClientViewModel(DAL.Models.Authentication.Client client)
        {
            ClientId = client.ClientId;
            Name = client.Name;
            WebApiApplicationTypes = client.WebApiApplicationTypes;
            WebApiApplicationDataAccessTypes = client.WebApiApplicationDataAccessTypes;
            Status = client.Status;
            RefreshTokenLifeTime = client.RefreshTokenLifeTime;
            AccessTokenLifeTime = client.AccessTokenLifeTime;
            AllowedOrigin = client.AllowedOrigin;
        }

        public Guid ClientId { get; set; }
        public string Name { get; set; }
        public string WebApiApplicationTypes { get; set; }
        public string WebApiApplicationDataAccessTypes { get; set; }
        public string Status { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public int AccessTokenLifeTime { get; set; }
        public string AllowedOrigin { get; set; }
    }
}
