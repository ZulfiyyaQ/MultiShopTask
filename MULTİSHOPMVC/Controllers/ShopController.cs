using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MULTİSHOPMVC.DAL;
using MULTİSHOPMVC.Models;
using MULTİSHOPMVC.ViewModels;

namespace MULTİSHOPMVC.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;


        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> productList = _context.Products.Include(x => x.ProductImages).ToList();
            ShopVM vm = new ShopVM
            {
                Products = productList, 
                LatestProducts = productList.OrderByDescending(p => p.Id).Take(8).ToList(),
                BestProducts=productList.OrderBy(p=>p.Name).ToList(),
                PopularProducts=productList.OrderBy(p=>p.Id).ToList()
                
            };
            return View(vm);
            
        }


        public async Task< IActionResult> Details(int id)
        {
            if (id <= 0) throw new Exception("Gonderilen sorgu yalnisdir");

            Product product = _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductColors).ThenInclude(pt => pt.Color)
                .FirstOrDefault(x => x.Id == id);

            if (product is null) throw new Exception("Bele bir mehsul tapilmadi");



            ProductVM productvm = new ProductVM
            {
                Product = product,
                RelatedProducts = _context.Products.Where(p => p.Category.Id == product.CategoryId && p.Id != product.Id).Include(x => x.ProductImages).ToList(),
            };



            return View(productvm);
        }
    }
}
