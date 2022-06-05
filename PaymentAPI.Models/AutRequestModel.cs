using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.Models
{
    public class AutRequestModel
    {
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string UserName { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
