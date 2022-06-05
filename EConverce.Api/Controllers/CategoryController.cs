using EConverce.Api.DataAccess;
using EConverce.Api.Entities;
using EconverceProject.Core.Models;
using EConverceProject.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace EConverce.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles ="Admin")]//kategori yönetimi sadece adminde
    public class CategoryController : ControllerBase
    {
        private DatabaseContext _db;// veri tabanı erişimi için
        private IConfiguration _configuration;
        public CategoryController(DatabaseContext databaseContext, IConfiguration configuration)
        {//bagımlılık enjeksiyonu
            _db = databaseContext;
            _configuration = configuration;

        }
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(Resp<List<CategoryModel>>))]
        public IActionResult List()
        {
            Resp<List<CategoryModel>> response = new Resp<List<CategoryModel>>();//burada response oluşturuyoruz
            //select ile verileri çekiyoruz ve hepsini category modele dönüştürüyoruz
            List<CategoryModel> list=_db.Categories.Select(x => new
            CategoryModel
            { Id = x.Id, Name = x.Name, Description = x.Description }).ToList();//burda list category modeli elde ettik
          
            response.Data = list;
            return Ok(response);
        }
        [HttpGet("get/{id}")]
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
        public IActionResult GetById([FromRoute] int id)
        {
            Resp<CategoryModel> response = new Resp<CategoryModel>();//burada response oluşturuyoruz
            //select ile verileri çekiyoruz ve hepsini category modele dönüştürüyoruz
            Category category = _db.Categories.SingleOrDefault(x => x.Id == id);
            CategoryModel data = null;
            if (category == null) { return NotFound(response); }
            
                data = new CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

            

            response.Data = data;
            return Ok(response);
        }

        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<CategoryModel>))]
        public IActionResult Create([FromBody] CategoryCreateModel model)
        {   Resp<CategoryModel> response = new Resp<CategoryModel>();
            string categoryName = model.Name?.Trim().ToLower();

            if (_db.Categories.Any(x=>x.Name.ToLower()==categoryName))
            {
                response.AddError(nameof(model.Name), "Bu kategori adı zaten mevcuttur.");
                return BadRequest(response);
            }
            else
            {
                Category category = new Category
                {
                    Name = model.Name,
                    Description = model.Description
                };
                _db.Categories.Add(category);
                _db.SaveChanges();

                CategoryModel data = new CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };
                response.Data = data;
                return Ok(response);
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
        public IActionResult Update([FromRoute] int id, [FromBody] CategoryUpdateModel model)
        {
            Resp<CategoryModel> response = new Resp<CategoryModel>();
            Category category = _db.Categories.Find(id);//kategoriyi bulduk bunu singleordefault la da yapabilirdik Category category = _db.Categories.SingleOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound(response);

            string categoryName = model.Name?.Trim().ToLower();

            if (_db.Categories.Any(x => x.Name.ToLower() == categoryName && x.Id != id))//burada farklı id de aynı isim var mı kontrolünü sağlıyoruz 
            {
                response.AddError(nameof(model.Name), "Bu kategori adı zaten mevcuttur.");
                return BadRequest(response);
            }
            category.Name = model.Name;
            category.Description = model.Description;
            _db.SaveChanges();


            CategoryModel data = new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };



            response.Data = data;
            return Ok(response);


        }
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
 
        [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
        public IActionResult Delete([FromRoute]int id)
        {
            Resp<object> response = new Resp<object>();
            Category category = _db.Categories.Find(id);
            if (category == null)
                return NotFound(response);

            _db.Remove(category);
            _db.SaveChanges();

            response.Data = true;
            return Ok(response);

        }

    }
}
