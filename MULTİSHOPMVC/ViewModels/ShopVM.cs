using MULTİSHOPMVC.Models;

namespace MULTİSHOPMVC.ViewModels
{
    public class ShopVM
    {
        public List<Product> Products { get; set; }
        public List<Product> LatestProducts { get; set; }
        public List<Product> BestProducts { get; set; }
        public List<Product> PopularProducts { get; set; }
    }
}
