using MULTİSHOPMVC.Models;

namespace MULTİSHOPMVC.ViewModels
{
    public class ShopVM
    {
        public int? Order { get; set; }
        public int? CategoryId { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Color> Colors { get; set; }
        

    }
}
