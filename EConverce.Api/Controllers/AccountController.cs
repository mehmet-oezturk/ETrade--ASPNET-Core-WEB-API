using EConverce.Api.DataAccess;
using EConverce.Api.Entities;
using EConverce.Api.Services;
using EConvercePorject.Core.Models;
using EConverceProject.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EConverce.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles ="Admin")]//İZİN VERME Sadece admin bu alanda işlem yapabilirdaha fazla kullanıcı için , ile ekleme yapıyoruz
    public class AccountController : ControllerBase
    {
        //Applyment :Satıcı Başvuru 
        //Register :üye kaydı
        //Authenticate :Kimlik doğrulaması yapılacak

        private DatabaseContext _db;
        private IConfiguration _configuration;
        public AccountController(DatabaseContext databaseContext,IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
           
        }

        [HttpPost("merchnat/applyment")]
        [ProducesResponseType(200, Type = typeof(Resp<ApplymentAccountResponseModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<ApplymentAccountResponseModel>))]
        public IActionResult Applyment([FromBody] ApplymentRequestAccountModel model)
        {
            Resp<ApplymentAccountResponseModel> response = new Resp<ApplymentAccountResponseModel>();
            /*if(ModelState.IsValid)*///gelen modeli kontrol ediyoruz
            {
                model.Username = model.Username?.Trim().ToLower();
                if (_db.Accounts.Any(x => x.Username.ToLower() == model.Username))//kayıttan önce kullanıcı adı var mı kontrolü yapıyoruz
                {
                    response.AddError(nameof(model.Username), "bu kullanıcı adı zaten var");
                    return BadRequest(response);
                }
                else
                {
                    Account account = new Account
                    {
                        Username = model.Username,
                        Password = model.Password,
                        CompanyName = model.CompanyName,
                        ContactEmail = model.ContactEmail,
                        ContactName = model.ContactName,
                        Type = AccountType.Merchant,//enum kullanmıştık 
                        IsApplyment = true
                    };
                    _db.Accounts.Add(account);
                    _db.SaveChanges();

                    ApplymentAccountResponseModel applymentAccountResponseModel = new ApplymentAccountResponseModel
                    {
                        Id = account.Id,
                        Username = account.Username,
                        ContactName = account.ContactName,
                        CompanyName = account.CompanyName,
                        ContactEmail = account.ContactEmail

                    };
                    //Hata ekleyeceksen .AddError Eklemeyeceksek .Data


                    response.Data = applymentAccountResponseModel;

                    return Ok(response);
                }

            }
            //select y deki string dizi oluşturuyoruz tek bir diziye çıkarmak için selecetmany yi kullanıyoruz  eğer  select kullansakdık sonucu bana list içinde list string döndürecek
            //List<string> errors = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToList();
            //return BadRequest(errors);
        }

        //üye kayıt metodumuz burası 
        //burası için Econverceproject.core un accountmodel kısmına RegisterRequestModel adında bir class oluşturduk
        //RegisterResponseModel adında bir class daha oluşturuyoruz
        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(Resp<RegisterResponseModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<RegisterResponseModel>))]
        public IActionResult Register([FromBody]RegisterRequestModel model)
        {
            Resp<RegisterResponseModel> response = new Resp<RegisterResponseModel>();
            //kullanıcı username uygun mu kontrolünü yapıyoruz
            model.Username = model.Username?.Trim().ToLower();
            if(_db.Accounts.Any(x=>x.Username.ToLower()==model.Username))
            {
                response.AddError(nameof(model.Username), "bu kullanıcı adı zaten var");
                return BadRequest(response);
            }
            else
            {
                Account account = new Account
                {
                    Username = model.Username,
                    Password = model.Password,
                    Type = AccountType.Member

                };
                _db.Accounts.Add(account);
                _db.SaveChanges();

                RegisterResponseModel data = new RegisterResponseModel
                {
                     Id= account.Id,
                    Username = account.Username
                };
                response.Data = data;
                return Ok(response);
            }
        }

        [AllowAnonymous]//DİKKAT [Authorize(Roles ="Admin")] işlemi ile kısıtladığımızı bu işlemi yaparak Herkesin kullana bilmesini sağlıyoruz
        [HttpPost("authenticate")]
        [ProducesResponseType(200, Type = typeof(Resp<AuthenticateRequestModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<AuthenticateRequestModel>))]
        public IActionResult Authenticate([FromBody] AuthenticateRequestModel model)
        {
            Resp<AuthenticateResponseModel> response = new Resp<AuthenticateResponseModel>();
            model.Username = model.Username?.Trim().ToLower();

            Account account = _db.Accounts.SingleOrDefault(
                x => x.Username.ToLower() == model.Username && x.Password == model.Password);
            if(account!=null)
            {//token oluşturmalıyız
                //Response a token ı yüklemeliyiz
                if(account.IsApplyment)//başvuru daha onaylanmadıysa
                {
                    response.AddError("*", "henüz satıcı başvurusu onaylanmamıştır");
                    return BadRequest(response);
                }
                else
                {   //token oluşturuyoruz response a token ı yüklemeliyiz
                    //appsetting de 
                    //"JwtOptions": {
                    //    "Key": "07D9E0B85D694F649F13593B4BDFBF40" oluşturduk
                    //startup da services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer( oluşturdurk
                    //BURADA MYSERVİCE DEN REFERE EDEREK TOKENSERVİCE İ KULLANDIK!!!!
                    string key = _configuration["JwtOptions:Key"];

                    List<Claim> claims = new List<Claim>
                            {
                                new Claim("Id",account.Id.ToString()),
                                new Claim("type",((int)account.Type).ToString()),
                                new Claim(ClaimTypes.Name,account.Username),

                                new Claim(ClaimTypes.Role,account.Type.ToString())

                            };
                     
                    string token = TokenService.GenarateToken(key,DateTime.Now.AddDays(30),claims);
                    AuthenticateResponseModel data = new AuthenticateResponseModel { Token = token };
                    response.Data = data;

                    return Ok(response);
                }
            }
            else
            {//* yerine nameof da verebiliriz
                response.AddError("*", "kullanıcı adı ya da şifre eşleşmiyor.");
                return BadRequest(response);
            }
        }

       
    }
}
