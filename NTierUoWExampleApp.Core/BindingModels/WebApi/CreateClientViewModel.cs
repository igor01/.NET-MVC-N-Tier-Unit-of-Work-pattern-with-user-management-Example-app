using NTierUoWExampleApp.Common.Enum.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.WebApi
{
    public class CreateClientViewModel
    {
        [Required]
        [Display(Name = "Client id")]
        public string ClientAppId { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Secret { get; set; }

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
        public int RefreshTokenLifeTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        [Display(Name = "Access token life time")]
        public int AccessTokenLifeTime { get; set; }

        [Required]
        [Display(Name = "Allowed origin")]
        public string AllowedOrigin { get; set; }
    }
}
