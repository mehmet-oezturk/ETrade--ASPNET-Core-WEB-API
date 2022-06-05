using EConverce.Api.DataAccess;
using EConverce.Api.Entities;
using EconverceProject.Core.Models;
using EConverceProject.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EConverce.Api.Controllers
{


    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class CartController : ControllerBase
    { 
        private DatabaseContext _db;// veri tabanı erişimi için
        private IConfiguration _configuration;
        public CartController(DatabaseContext databaseContext,IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;

        }
        //GetOrCreate: sepet getir yada oluştur
        //AddToCart :sepete ürün ekleme

        [HttpGet("GetOrCreate/{accountId}")]
        [ProducesResponseType(200, Type = typeof(Resp<CartModel>))]
       
        public IActionResult GetOrCreate([FromRoute] int accountId)
        {
            Resp<CartModel> response = new Resp<CartModel>();
            Cart cart = _db.Carts
                .Include(x => x.CartProducts)
                .SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);

            if (cart == null)
            {
                cart = new Cart
                {
                    AccountId = accountId,
                    Date = DateTime.Now,
                    IsClosed = false,
                    CartProducts = new List<CartProduct>()
                };
                _db.Carts.Add(cart);
                _db.SaveChanges();
            }
            CartModel data = CartToCartModel(cart);

            response.Data = data;
            return Ok(response);


        }

        private static CartModel CartToCartModel(Cart cart)
        {
            CartModel data = new CartModel
            {
                Id = cart.Id,
                AccountId = cart.AccountId,
                Date = cart.Date,
                IsClosed = cart.IsClosed,
                CartProducts = new List<CartProductModel>()

            };

            foreach (CartProduct cartProduct in cart.CartProducts)
            {
                data.CartProducts.Add(new CartProductModel
                {
                    Id = cartProduct.Id,
                    CartId = cartProduct.CartId.Value,
                    UnitPrice = cartProduct.UnitPrice,
                    DiscountedPrice = cartProduct.DiscountedPrice,
                    Quantity = cartProduct.Quantity,
                    ProductId = cartProduct.ProductId.Value


                });
            }

            return data;
        }

        [HttpPost("AddToCart/{accountId}")]
        public IActionResult AddToCart([FromRoute]int accountId,[FromBody]AddToCartModel model)
        {
            Resp<CartModel> response = new Resp<CartModel>();
            Cart cart = _db.Carts
                .Include(x => x.CartProducts)
                .SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);
            if (cart == null)
            {
                cart = new Cart
                {
                    AccountId = accountId,
                    Date = DateTime.Now,
                    IsClosed = false,
                    CartProducts = new List<CartProduct>()
                };
                _db.Carts.Add(cart);
              
            }
            Product product = _db.Products.Find(model.ProductId);

            cart.CartProducts.Add(new CartProduct
            {
                CartId = cart.Id,
                ProductId=product.Id,
                UnitPrice=product.UnitPrice,
                DiscountedPrice=product.DiscountedPrice,
                Quantity=model.Quantity

            });
            _db.SaveChanges();

            CartModel data = CartToCartModel(cart);
            response.Data = data;
            return Ok(response);

        }



    }
}
