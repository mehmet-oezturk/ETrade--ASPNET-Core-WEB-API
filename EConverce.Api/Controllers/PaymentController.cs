using EConverce.Api.DataAccess;
using EConverce.Api.Entities;
using EconverceProject.Core.Models;
using EConverceProject.Core.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyServices;
using PaymentAPI.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EConverce.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class PaymentController:ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;
        public PaymentController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;

        }
        [HttpPost("Pay/{cartid}")]
        [ProducesResponseType(200, Type = typeof(Resp<PaymentModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<string>))]
        public IActionResult Pay([FromRoute] int cartid,[FromBody]PayModel model )
        {
            Resp<PaymentModel> result = new Resp<PaymentModel>();
            //sepet kapalımı  ödemesi var mı ona bakıyoruz
            Cart cart = _db.Carts.Include(x => x.CartProducts).SingleOrDefault(x => x.Id == cartid);
       
            string paymentApiEndPoint = _configuration["PaymentAPI:EndPoint"];

            if (!cart.IsClosed)
            {
                decimal totalPrice = model.TotalPriceOverride ?? cart.CartProducts.Sum(x => x.Quantity*x.DiscountedPrice);
                //TÜKETME
                HttpClientService client = new HttpClientService(paymentApiEndPoint);//domain=paymentApiEndPoint
                AutRequestModel autRequestModel = new AutRequestModel 
                { UserName = _configuration["PaymentAPI:Username"]
                , Password=_configuration["PaymentAPI:Password"] };
                HttpClientServiceReponse<AutResponseModel> authResponse= client.Post<AutRequestModel, AutResponseModel>("/Pay/authenticate", autRequestModel);

                //StringContent content = new StringContent(JsonSerializer.Serialize(autRequestModel), Encoding.UTF8, "application/json"); 
              //HttpResponseMessage authResponse=  client.PostAsync($"{paymentApiEndPoint}/Pay/authenticate", content).Result;

                if(authResponse.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    //string authJsonContent = authResponse.Content.ReadAsStringAsync().Result;
                    //AutResponseModel autResponseModel = JsonSerializer.Deserialize<AutResponseModel>(authJsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    string token = authResponse.Data.Token;

                    PaymentRequestModel paymentRequestModel = new PaymentRequestModel
                    {
                        CartNumber = model.CartNumber,
                        CardName = model.CardName,
                        ExpireDate = model.ExpireDate,
                        CVV = model.CVV

                    };
                    StringContent paymentContent =
                        new StringContent(JsonSerializer.Serialize(paymentRequestModel), Encoding.UTF8, "application/json");

                   HttpClientServiceReponse<PaymentResponseModel> paymentResponse=
                        client.Post<PaymentRequestModel, PaymentResponseModel>("/Pay/Paymnet", paymentRequestModel, token);

                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
                    //HttpResponseMessage paymentResponse=
                    //client.PostAsync($"{paymentApiEndPoint}/Pay/Payment", paymentContent).Result;

                    if (paymentResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    { //string paymnetjsonContent = paymentResponse.Content.ReadAsStringAsync().Result;
                    //    PaymentResponseModel paymentResponseModel =
                    //        JsonSerializer.Deserialize<PaymentResponseModel>(paymnetjsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (paymentResponse.Data.Result == "ok")
                        {
                            string transactionId = paymentResponse.Data.TransactionId;
                            Payment payment = new Payment
                            {
                                CartId = cartid,
                                AccountId = cart.AccountId,
                                InvoiceAddress = model.InvoiceAddress,
                                ShippeAdress = model.ShippeAdress,
                                Type = model.Type,
                                TransactionId = transactionId,
                                Date = DateTime.Now,
                                IsCompleted = true,
                                TotalPrice = totalPrice
                            };
                            cart.IsClosed = true;
                            _db.Payments.Add(payment);
                            _db.SaveChanges();
                            PaymentModel data = new PaymentModel
                            {
                                Id = payment.Id,
                                AccountId = payment.AccountId,
                                CartId = payment.CartId,
                                Date = payment.Date,
                                InvoiceAddress = payment.InvoiceAddress,
                                IsCompleted = payment.IsCompleted,
                                ShippeAdress = payment.ShippeAdress,
                                TotalPrice = payment.TotalPrice,
                                Type = payment.Type
                            };
                            result.Data = data;

                            return Ok(result);

                        }
                        else
                        {
                            Resp<string> paymentOkResult = new Resp<string>();
                            paymentOkResult.AddError("payment"," Ödeme alınamadı.");
                            return BadRequest(paymentOkResult);
                        }
                    }
                    else
                    {
                        Resp<string> paymentResult = new Resp<string>();
                        paymentResult.AddError("payment", paymentResponse.ResponseContent);
                        return BadRequest(paymentResult);
                    }
                }
                else
                {
                    Resp<string> authResult = new Resp<string>();
                    authResult.AddError("auth", authResponse.ResponseContent);
                    return BadRequest(authResult);
                }



            }
            else
            {
                Payment payment = _db.Payments.SingleOrDefault(x => x.CartId == cartid);
                if (payment == null)
                {
                    result.AddError("cart", $"Sepet Kapalı ama ödemesi yapılmamış görülmektedir. Olası sorun tespit edildi lütfen sistem sağlayıcı ile iletişime geçiniz. Cart Id:{cartid}");

                    return BadRequest(result);
                }
                PaymentModel data = new PaymentModel
                {
                    Id = payment.Id,
                    AccountId = payment.AccountId,
                    CartId = payment.CartId,
                    Date = payment.Date,
                    InvoiceAddress = payment.InvoiceAddress,
                    IsCompleted = payment.IsCompleted,
                    ShippeAdress = payment.ShippeAdress,
                    TotalPrice = payment.TotalPrice,
                    Type = payment.Type
                };
                result.Data = data;
                return Ok(result);

            }

        }
    }
}
