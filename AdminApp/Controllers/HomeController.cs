using AdminApp.Models;
using EConvercePorject.Core.Models;
using EConverceProject.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MerchantApplyment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult MerchantApplyment(ApplymentRequestAccountModel model)
        {
            if(ModelState.IsValid)
            {
                string endpoint = _configuration["EConverceAPI:EndPoint"];
                HttpClientService client = new HttpClientService(endpoint);

                AuthenticateRequestModel authenticateRequestModel =
                    new AuthenticateRequestModel { Username = _configuration["EConverceAPI:AdminUid"], Password = _configuration["EConverceAPI:AdminPwd"] };

              HttpClientServiceReponse<Resp<AuthenticateResponseModel>>authResponse= 
                    client.Post<AuthenticateRequestModel, Resp<AuthenticateResponseModel>>
                    ("Account/authenticate", authenticateRequestModel);
                if (authResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string token = authResponse.Data.Data.Token;

                    HttpClientServiceReponse<Resp<ApplymentAccountResponseModel>> applyResponse =
                   client.Post<ApplymentRequestAccountModel, Resp<ApplymentAccountResponseModel>>
                   ("/Account/merchnat/applyment", model,token);

                    if (applyResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewData["success"] = "satıcı başvurusu alınmıştır.";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, authResponse.ResponseContent);
                    }
                }
                else 
                {
                    ModelState.AddModelError(string.Empty, authResponse.ResponseContent);
                }

            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
