using EConverce.Api.DataAccess;
using EConverce.Api.Entities;
using EconverceProject.Core.Models;
using EConverceProject.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EConverce.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin,Merchant")]// yönetimi  admin ve merchant
    public class ProductController : ControllerBase
    {
        private DatabaseContext _db;// veri tabanı erişimi için
        private IConfiguration _configuration;
        public ProductController(DatabaseContext databaseContext, IConfiguration configuration)
        {//bagımlılık enjeksiyonu
            _db = databaseContext;
            _configuration = configuration;

        }
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(Resp<List<ProductModel>>))]
        public IActionResult List()//bu metot bütün ürünleri listeleyecek 
        {
            Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();//burada response oluşturuyoruz
            //select ile verileri çekiyoruz ve hepsini category modele dönüştürüyoruz

           /* int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);*/// burada tokendan id inin bilgisini alarak int e çevirdik
            List<ProductModel> list = _db.Products
                .Include(x=>x.Category)
                .Include(x=>x.Account)
                //.Where(x=>x.AccountId==accountId)
                .Select(x => new
              ProductModel
            { Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    DiscountedPrice = x.DiscountedPrice,
                    Discontinued = x.Discontinued,
                    CategoryId = x.CategoryId,
                    AccountId = x.AccountId,
                    CategoryName = x.Category.Name,
                    AccountCompanyName = x.Account.CompanyName
                }).ToList();//burda list category modeli elde ettik

            response.Data = list;
            return Ok(response);
        }

        [HttpGet("list/{accountId}")]
        [ProducesResponseType(200, Type = typeof(Resp<List<ProductModel>>))]
        public IActionResult ListByAccountId([FromRoute]int accountId)//baccount ıd ye göre ürünleri listeleme metodu
        {
            Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();//burada response oluşturuyoruz
            //select ile verileri çekiyoruz ve hepsini category modele dönüştürüyoruz

           /* int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);*/// burada tokendan id inin bilgisini alarak int e çevirdik
            List<ProductModel> list = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Account)
                .Where(x=>x.AccountId==accountId)
                .Select(x => new
              ProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    DiscountedPrice = x.DiscountedPrice,
                    Discontinued = x.Discontinued,
                    CategoryId = x.CategoryId,
                    AccountId = x.AccountId,
                    CategoryName = x.Category.Name,
                    AccountCompanyName = x.Account.CompanyName
                }).ToList();//burda list category modeli elde ettik

            response.Data = list;
            return Ok(response);
        }

        [HttpGet("get/{productId}")]
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
        public IActionResult GetById([FromRoute] int productId)// ID ile tekil bir ürünü getiren metot
        {
            Resp<ProductModel> response = new Resp<ProductModel>();//burada response oluşturuyoruz
                                                                   //select ile verileri çekiyoruz ve hepsini category modele dönüştürüyoruz
            //int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);//bunu istersek koyabiliriz
            Product product = _db.Products
                .Include(x=>x.Category)
                .Include(x=>x.Account)
                .SingleOrDefault(x => x.Id == productId);
        
            
            if (product == null) { return NotFound(response); }

          ProductModel data = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                DiscountedPrice = product.DiscountedPrice,
                Discontinued = product.Discontinued,
                CategoryId = product.CategoryId,
                AccountId = product.AccountId,
                CategoryName = product.Category.Name,
                AccountCompanyName = product.Account.CompanyName
          };



            response.Data = data;
            return Ok(response);
        }

        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(Resp<ProductModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<ProductModel>))]
        public IActionResult Create([FromBody] ProductCreateModel model)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();
            string productName = model.Name?.Trim().ToLower();

            if (_db.Products.Any(x => x.Name.ToLower() == productName))
            {
                response.AddError(nameof(model.Name), "Bu ürün adı zaten mevcuttur.");
                return BadRequest(response);
            }
            else
            { int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);// burada tokendan id inin bilgisini alarak int e çevirdik
                Product product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    UnitPrice=model.UnitPrice,
                    DiscountedPrice=model.DiscountedPrice,
                    Discontinued=model.Discontinued,
                    CategoryId=model.CategoryId,
                    AccountId=accountId    //accounnt ıd yı yukarıda tokendan okuduk 


                };
                _db.Products.Add(product);
                _db.SaveChanges();
                //ürün nesnelerini çekiyoruz Include için ef.core ekliyoruz
                //lazy loading varsa eğer  ınclude yapmaya gereke kalmıyor
               product= _db.Products.Include(x => x.Category).Include(x => x.Account).SingleOrDefault(x => x.Id == product.Id);//Include ile ID eşleşen verilere  category ve Account bilgilerini dahil ediyoruz

                ProductModel data = new ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    DiscountedPrice = product.DiscountedPrice,
                    Discontinued = product.Discontinued,
                    CategoryId = product.CategoryId,
                    AccountId = product.AccountId,
                    CategoryName=product.Category.Name,
                    AccountCompanyName=product.Account.CompanyName
                    
                };
                response.Data = data;
                return Ok(response);
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(200, Type = typeof(Resp<ProductModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<ProductModel>))]
        [ProducesResponseType(404, Type = typeof(Resp<ProductModel>))]
        public IActionResult Update([FromRoute] int id, [FromBody] ProductUpdateModel model)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();
           
            int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);
            string role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            Product product = _db.Products.SingleOrDefault(x => x.Id == id && (role == "Admin" || (role != "Admin" && x.AccountId == accountId)));//productı bulduk bunu singleordefault la da yapabilirdik Category category = _db.Categories.SingleOrDefault(x => x.Id == id);
            if (product == null)
                return NotFound(response);

            string productName = model.Name?.Trim().ToLower();

            if (_db.Products.Any(x => x.Name.ToLower() == productName && x.Id!=id && (role!="Admin"&& x.AccountId == accountId)))//burada farklı id de aynı isim var mı kontrolünü sağlıyoruz 
            {
                response.AddError(nameof(model.Name), "Bu ürün adı zaten mevcuttur.");
                return BadRequest(response);
            }
            product.Name = model.Name;
            product.Description = model.Description;
            product.UnitPrice = model.UnitPrice;
            product.DiscountedPrice = model.DiscountedPrice;
            product.Discontinued = model.Discontinued;
            product.CategoryId = model.CategoryId;

            _db.SaveChanges();

            product = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Account)
                .SingleOrDefault(x => x.Id == product.Id);

            ProductModel data= new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                DiscountedPrice = product.DiscountedPrice,
                Discontinued = product.Discontinued,
                CategoryId = product.CategoryId,
                AccountId = product.AccountId,
                CategoryName = product.Category.Name,
                AccountCompanyName = product.Account.CompanyName

            };
            response.Data = data;
            return Ok(response);





        }
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]

        [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
        public IActionResult Delete([FromRoute] int id)
        {
            Resp<object> response = new Resp<object>();
            int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);
            string role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            Product product = _db.Products.SingleOrDefault(x => x.Id == id && (role == "Admin" || (role != "Admin" && x.AccountId == accountId)));//productı bulduk bunu singleordefault la da yapabilirdik Category category = _db.Categories.SingleOrDefault(x => x.Id == id);
            if (product == null)
                return NotFound(response);

            _db.Remove(product);
            _db.SaveChanges();

            response.Data = true;
            return Ok(response);

        }

    }
}
