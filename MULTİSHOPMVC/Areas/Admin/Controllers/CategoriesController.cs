using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MULTİSHOPMVC.Areas.Admin.ViewModels;
using MULTİSHOPMVC.Areas.Admin.ViewModels;
using MULTİSHOPMVC.DAL;
using MULTİSHOPMVC.Models;
using MULTİSHOPMVC.Utilities.Extensions;
using MULTİSHOPMVC.ViewModels;

namespace MULTİSHOPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoriesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            double count = await _context.Categories.CountAsync();


            List<Category> categories = await _context.Categories.Skip((page - 1) * 3).Take(3)
                .Include(c => c.Products).ToListAsync();

            PaginationVM<Category> paginateVM = new PaginationVM<Category>
            {
                CurrentPage = page,
                TotalPage = Math.Ceiling(count / 3),
                Items = categories

            };
            return View(paginateVM);
        }



       
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Create(CreateCategoriesVM categoryvm)
        {
            if (!ModelState.IsValid) return View();

            bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == categoryvm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bele category artiq movcutdur");
                return View();
            }
            if (!categoryvm.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "Sekil file secmeyiniz mutleqdir");
                return View();
            }
            if (!categoryvm.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "Sekil olcusu 2 mb dan artiq olmamalidir");
                return View();
            }


            string filename = await categoryvm.Photo.CreateFile(_env.WebRootPath, "img");
            Category category = new Category
            {
                Name = categoryvm.Name,
                ImageUrl = filename
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
       
        public async Task<IActionResult> Update(int id)
        {
            Category existed = await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateCategoriesVM vm = new()
            {
                Name = existed.Name,
                ImageUrl=existed.ImageUrl

            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoriesVM categoryvm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == categoryvm.Name.ToLower().Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele category artiq movcutdur");
                return View();
            }
            if (categoryvm.Photo is not null)
            {
                if (!categoryvm.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "Sekil file secmeyiniz mutleqdir");
                    return View(existed);
                }
                if (!categoryvm.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "Sekil olcusu 2 mb dan artiq olmamalidir");
                    return View(existed);
                }
                string newimage = await categoryvm.Photo.CreateFile(_env.WebRootPath, "img");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "img");
                existed.ImageUrl = newimage;
            }
            existed.Name = categoryvm.Name;
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            _context.Categories.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }
        public async Task<IActionResult> Detail(int id)
        {
            var category = await _context.Categories.Include(c => c.Products).ThenInclude(p => p.ProductImages).FirstOrDefaultAsync(pi => pi.Id == id);
            if (category is null) return NotFound();
            return View(category);
        }
    }
}
