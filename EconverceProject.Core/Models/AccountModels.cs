using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EConvercePorject.Core.Models
{
    public class ApplymentRequestAccountModel
    {
        [Required]
        [StringLength(25)]
        public string Username { get; set; }
        [Required]
        [StringLength(16,MinimumLength =6)]
        public string Password { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 6)]
        [Compare(nameof(Password))]//eşleştirmeyi yapıyoruz
        public string RePassword { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string ContactName { get; set; }
        [StringLength(50)]
        [EmailAddress]
        public string ContactEmail { get; set; }
    }
    public class ApplymentAccountResponseModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
      
        

        public string CompanyName { get; set; }
     
        public string ContactName { get; set; }
       
        public string ContactEmail { get; set; }

    }
    public class RegisterRequestModel
    {
        [Required]
        [StringLength(25)]
        public string Username { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 6)]
        [Compare(nameof(Password))]//eşleştirmeyi yapıyoruz
        public string RePassword { get; set; }
    }
    public class RegisterResponseModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }
    public class AuthenticateRequestModel
    {

        [Required]
        [StringLength(25)]
        public string Username { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 6)]
        public string Password { get; set; }
    }
    public class AuthenticateResponseModel
    {
        public string Token { get; set; }
    }
}
