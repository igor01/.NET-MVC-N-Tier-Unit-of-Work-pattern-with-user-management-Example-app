using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Authentication
{
    public class Client
    {
        public Client()
        {
            ClientUsers = new List<ClientUsers>();
            RefreshTokens = new List<RefreshToken>();
        }

        public Guid ClientId { get; set; }
        public string ClientAppId { get; set; } //application id
        public string Secret { get; set; } //The Secret column is hashed so if anyone has an access to the database will not be able to see the secrets
        public string Name { get; set; }

        /*
         * Confidential or NonConfidential
         * 
         * Confidential client should send the secret once the access token is requested
         */
        public string WebApiApplicationTypes { get; set; }
        public string WebApiApplicationDataAccessTypes { get; set; }
        public string Status { get; set; } //activate or deactivate this client
        public int RefreshTokenLifeTime { get; set; } //set when the refresh token (not the access token) will expire in minutes
        public int AccessTokenLifeTime { get; set; } // in minutes
        public DateTime DateCreated { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? DateModified { get; set; }
        public string AllowedOrigin { get; set; }  //set domain of the application - optional set this to "*"
        public virtual ICollection<ClientUsers> ClientUsers { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
