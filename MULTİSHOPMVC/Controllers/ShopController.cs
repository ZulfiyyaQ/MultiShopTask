using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        public async Task<IActionResult> Index(int? order, int? categoryId, int page)
        {
            if (page < 0) throw new Exception("The request sent does not exist");
            double count = await _context.Products.CountAsync();
            IQueryable<Product> queryable = _context.Products
                .Include(pi => pi.ProductImages.Where(a => a.IsPrimary != null)).AsQueryable();
            switch (order)
            {
                case 1:
                    queryable = queryable.OrderBy(p => p.Name);
                    break;
                case 2:
                    queryable = queryable.OrderBy(p => p.Price);
                    break;
                case 3:
                    queryable = queryable.OrderByDescending(p => p.Id);
                    break;
                case 4:
                    queryable = queryable.OrderByDescending(p => p.Price);
                    break;
            }
            if (categoryId != null)
            {
                count = _context.Products.Where(p => p.CategoryId == categoryId).Count();
                queryable = queryable.Where(p => p.CategoryId == categoryId);
            }
            ShopVM shopVM = new ShopVM
            {
                Categories = await _context.Categories.Include(c => c.Products).ToListAsync(),
                Colors = await _context.Colors.Include(c => c.ProductColors).ThenInclude(c=>c.Product).ToListAsync(),
                Products = await queryable.Skip(page * 4).Take(4).ToListAsync(),
                Order = order,
                CategoryId = categoryId
            };

            PaginationVM<ShopVM> paginationVM = new PaginationVM<ShopVM>
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 4),
                Item = shopVM
            };
            if (paginationVM.TotalPage < page) throw new Exception("Your request was not found");

            return View(paginationVM);
        }
        public async Task<IActionResult> Details(int id)
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
                RelatedProducts = _context.Products
                .Where(p => p.Category.Id == product.CategoryId && p.Id != product.Id).Include(x => x.ProductImages.Where(x => x.IsPrimary == true)).ToList()
            };



            return View(productvm);
        }



    }
}

