using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MULTİSHOPMVC.DAL;
using MULTİSHOPMVC.Models;
using MULTİSHOPMVC.ViewModels;

namespace MULTİSHOPMVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _context;


        public HomeController(AppDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {


            List<Slide> slides = _context.Slides.OrderBy(p => p.Order).ToList();
            List<Product> productList = _context.Products.Include(x => x.ProductImages).Include(x=>x.Category).ToList();
            List<Category> categories = _context.Categories.Include(c => c.Products).ToList();

            HomeVM vm = new HomeVM
            {
                Products = productList,
                Categories=categories,
                Slides = slides,
                LatestProducts = productList.OrderByDescending(p => p.Id).Take(8).ToList()
            };
            return View(vm);
        }
    }
}
