using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using PaymentAPI.Models;
using Microsoft.Extensions.Configuration;
using MyServices;
using System.Security.Claims;
using System.Collections.Generic;

namespace PaymentAPI.Controllers
{  
   
    
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private IConfiguration _configuration;
        public PayController( IConfiguration configuration)
        {
           
            _configuration = configuration;

        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(200, Type = typeof(AutResponseModel))]
        [ProducesResponseType(400, Type = typeof(string))]
        public IActionResult Authenticate([FromBody]AutRequestModel model)
        {
            string uid = _configuration["Auth:Uid"];//Auth isimili bir obje açıyoruz uid ye mehmetozturku  alacaz
            string pass = _configuration["Auth:Pass"];// buradada pass ı alacaz kodun içinde  bu veriler bu şekilde gömülü kalmmaz bu sayede appsetingsden istediğimiz zaman değişebiliriz
            if (model.UserName==uid&&model.Password==pass)
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("uid", uid));

                string token = TokenService.GenarateToken(_configuration["JwtOptions:Key"]
                    ,DateTime.Now.AddDays(30),
                    claims,
                    _configuration["JwtOptions:Issuer"],
                    _configuration["JwtOptions:Audience"]);
                return Ok(new AutResponseModel { Token=token});
            }
            else
            {
                return BadRequest("Kullanıcı ve şifre eşleşmiyor!");
            }
            
        }
        [HttpPost("payment")]
        [ProducesResponseType(200, Type = typeof(PaymentResponseModel))]
        [ProducesResponseType(400, Type = typeof(string))]
        public IActionResult Payment([FromBody] PaymentRequestModel model)
        {
            string cardno = _configuration["CardTest:No"];
            string name = _configuration["CardTest:Name"];
            string exp = _configuration["CardTest:Exp"];
            string cvv = _configuration["CardTest:CVV"];

            if (model.CartNumber==cardno&&model.CardName==name&&model.ExpireDate==exp&&model.CVV==cvv)
            {
                return Ok(new PaymentResponseModel { Result = "ok", TransactionId = Guid.NewGuid().ToString() });
            }
            else
            {
                return BadRequest("Kart Bilgileri geçersiz.Ödeme alınamadı.");
            }
        }
    }
}
