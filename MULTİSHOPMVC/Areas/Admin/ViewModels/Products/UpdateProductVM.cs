using Azure;
using MULTİSHOPMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace MULTİSHOPMVC.Areas.Admin.ViewModels
{
    public class UpdateProductVM
    {
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Rating { get; set; }
        public string Inst { get; set; }
        public string Face { get; set; }
        public string Pin { get; set; }
        public string Tvit { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<int>? ImageIds { get; set; }
        public List<int> ColorsId { get; set; }
        public List<Category>? Categories { get; set; }      
        public List<Color>? Colors { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
    }
}
