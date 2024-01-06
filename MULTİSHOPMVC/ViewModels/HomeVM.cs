using MULTİSHOPMVC.Models;

namespace MULTİSHOPMVC.ViewModels
{
    public class HomeVM
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Slide> Slides { get; set; }
        public List<Product> LatestProducts { get; set; }
    }
}
