using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconverceProject.Core.Models
{
    public class CategoryModel//bu model 3 farklı kategoride kullanılacak
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
      
        //public string Description { get; internal set; }
    }
   public class CategoryCreateModel
    {
        
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
    public class CategoryUpdateModel
    {

        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        }
    }
