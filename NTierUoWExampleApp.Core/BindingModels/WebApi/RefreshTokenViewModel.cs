using NTierUoWExampleApp.DAL.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.WebApi
{
    public class RefreshTokenViewModel
    {
        public RefreshTokenViewModel()
        {

        }

        public RefreshTokenViewModel(RefreshToken token)
        {
            RefreshTokenId = token.RefreshTokenId;
            Name = token.Name;
            IMSI = token.IMSI;
            ClientId = token.ClientId;
            ClientAppId = token.ClientAppId;
            IssuedUtc = token.IssuedUtc;
            ExpiresUtc = token.ExpiresUtc;
            ProtectedTicket = token.ProtectedTicket;
        }

        public string RefreshTokenId { get; set; }
        public string Name { get; set; }
        public string IMSI { get; set; }
        public Guid ClientId { get; set; }
        public string ClientAppId { get; set; }

        public DateTime IssuedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }

        public string ProtectedTicket { get; set; }
    }
}
