using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Authentication
{
    public class RefreshToken
    {
        public string RefreshTokenId { get; set; }
        public string Name { get; set; }
        public string IMSI { get; set; } //if you want to bind token with mobile phone
        public Guid ClientId { get; set; }
        public string ClientAppId { get; set; }

        //The Issued UTC and Expires UTC columns are for displaying purpose only
        public DateTime IssuedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }

        //contains a serialized representation for the ticket for specific user
        //contains all the claims and ticket properties for this user
        //The Owin middle-ware will use this string to build the new access token auto-magically
        [Required]
        public string ProtectedTicket { get; set; }
        public virtual Client Client { get; set; }
    }
}
