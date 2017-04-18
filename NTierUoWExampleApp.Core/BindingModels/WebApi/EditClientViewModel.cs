using NTierUoWExampleApp.Common.Enum.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.WebApi
{
    public class EditClientViewModel
    {
        public EditClientViewModel()
        {

        }

        public EditClientViewModel(DAL.Models.Authentication.Client client)
        {
            ClientId = client.ClientId;
            ClientAppId = client.ClientAppId;
            Secret = string.Empty;
            Name = client.Name;

            WebApiApplicationTypes tmpWebApiApplicationTypes;
            Enum.TryParse<WebApiApplicationTypes>(client.WebApiApplicationTypes, out tmpWebApiApplicationTypes);
            this.WebApiApplicationTypes = tmpWebApiApplicationTypes;

            WebApiApplicationDataAccessTypes tmpWebApiApplicationDataAccessTypes;
            Enum.TryParse<WebApiApplicationDataAccessTypes>(client.WebApiApplicationDataAccessTypes, out tmpWebApiApplicationDataAccessTypes);
            this.WebApiApplicationDataAccessTypes = tmpWebApiApplicationDataAccessTypes;

            ClientStatusEnum tmpStatus;
            Enum.TryParse<ClientStatusEnum>(client.Status, out tmpStatus);
            this.Status = tmpStatus;

            RefreshTokenLifeTime = client.RefreshTokenLifeTime;
            AccessTokenLifeTime = client.AccessTokenLifeTime;
            AllowedOrigin = client.AllowedOrigin;
        }


        public Guid ClientId { get; set; }

        [Required]
        public string ClientAppId { get; set; } 

        [Display(Name = "Password")]
        public string Secret { get; set; } 

        [Display(Name = "Change password")]
        public bool ChangeSecret { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Application type")]
        public WebApiApplicationTypes WebApiApplicationTypes { get; set; }

        [Required]
        [Display(Name = "Data access type")]
        public WebApiApplicationDataAccessTypes WebApiApplicationDataAccessTypes { get; set; }

        [Required]
        public ClientStatusEnum Status { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        [Display(Name = "Refresh token life time")]
        public int RefreshTokenLifeTime { get; set; } //set when the refresh token (not the access token) will expire in minutes


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        [Display(Name = "Access token life time")]
        public int AccessTokenLifeTime { get; set; }


        public DateTime DateCreated { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? DateModified { get; set; }

        [Required]
        [Display(Name = "Allowed origin")]
        public string AllowedOrigin { get; set; }  //set domain of the application - optional set this to "*"
    }
}
