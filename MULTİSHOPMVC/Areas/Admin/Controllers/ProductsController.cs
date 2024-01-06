using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MULTİSHOPMVC.Areas.Admin.ViewModels;
using MULTİSHOPMVC.DAL;
using MULTİSHOPMVC.Models;
using MULTİSHOPMVC.Utilities.Extensions;
using MULTİSHOPMVC.ViewModels;

namespace MULTİSHOPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
     
        public async Task<IActionResult> Index(int page = 1)
        {
            double count = await _context.Products.CountAsync();

            List<Product> products = await _context.Products.Skip((page - 1) * 3).Take(3)
                .Include(x => x.Category)
                .Include(p => p.ProductImages.Where(p => p.IsPrimary == true))
                .ToListAsync();

            PaginationVM<Product> paginateVM = new PaginationVM<Product>
            {
                CurrentPage = page,
                TotalPage = Math.Ceiling(count / 3),
                Items = products

            };
            return View(paginateVM);
        }
       
        public async Task<IActionResult> Create()
        {
            CreateProductVM productvm = new();
            GetList(productvm);
            return View(productvm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productvm)
        {

            if (!ModelState.IsValid)
            {
                GetList(productvm);
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Id == productvm.CategoryId);
            if (!result)
            {
                GetList(productvm);
                ModelState.AddModelError("CategoryId", "Bele Id li category movcud deyil");
                return View();
            }

            if (!productvm.MainPhoto.ValidateType("image/"))
            {
                GetList(productvm);
                ModelState.AddModelError("MainPhoto", "File tipi uygun deyil ");
                return View();
            }

            if (!productvm.MainPhoto.ValidateSize(600))
            {
                GetList(productvm);
                ModelState.AddModelError("MainPhoto", "File olcusu uygun deyil");
                return View();
            }

          

           

            ProductImage img = new ProductImage
            {
                IsPrimary = true,
                Url = await productvm.MainPhoto.CreateFile(_env.WebRootPath, "img")
            };
          


          
            Product product = new Product
            {
                Name = productvm.Name,
                Price = productvm.Price,
                Description = productvm.Description,
                Rating = productvm.Rating,
                Face=productvm.Face,
                Inst=productvm.Inst,
                Tvit=productvm.Tvit,
                Pin=productvm.Pin,
               
                CategoryId = (int)productvm.CategoryId,
              
                ProductColors = new List<ProductColor>(),
               
                ProductImages = new List<ProductImage> { img }

            };
            TempData["Message"] = "";
            
            foreach (int colorId in productvm.ColorIds)
            {
                ProductColor productcolor = new ProductColor
                {
                    ColorId = colorId
                };
                product.ProductColors.Add(productcolor);
            }
            


            foreach (var photo in productvm.Photos)
            {
                if (!photo.ValidateType("image/"))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil  </p>";
                    continue;
                }
                if (!photo.ValidateSize(600))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil </p> ";
                    continue;
                }

                product.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Url = await photo.CreateFile(_env.WebRootPath, "img")
                });
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
       
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(p => p.ProductImages)
                .Include(pc => pc.ProductColors)
                
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = existed.Name,
                Price = existed.Price,
                Description = existed.Description,
                Rating = existed.Rating,
                Face = existed.Face,
                Inst = existed.Inst,
                Tvit = existed.Tvit,
                Pin = existed.Pin,

                CategoryId = existed.CategoryId,
                Categories = await _context.Categories.ToListAsync(),
                
               
                ColorsId = existed.ProductColors.Select(pc => pc.ColorId).ToList(),
                Colors = await _context.Colors.ToListAsync(),
               
                ProductImages = existed.ProductImages
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _context.Products.Include(pi => pi.ProductImages)
                .Include(pc => pc.ProductColors)
                
                .FirstOrDefaultAsync(e => e.Id == id);
            productVM.ProductImages = existed.ProductImages;
            if (!ModelState.IsValid)
            {
                GetList(productVM);
                return View(productVM);
            }


            if (existed is null) return NotFound();


            bool result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                GetList(productVM);
                ModelState.AddModelError("Name", "Product already exists");
                return View(productVM);
            }


            bool result1 = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result1)
            {
                GetList(productVM);
                ModelState.AddModelError("CategoryId", "Category not found, choose another one.");
                return View(productVM);
            }

            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    GetList(productVM);
                    ModelState.AddModelError("MainPhoto", "Tipi uygun deyil");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(600))
                {
                    GetList(productVM);
                    ModelState.AddModelError("MainPhoto", "Olcusu uygun deyil");
                    return View(productVM);
                }

            }
            
            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "img");

                ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainImage.Url.DeleteFile(_env.WebRootPath, "img");
                _context.ProductImages.Remove(mainImage);

                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Url = fileName
                });
            }
           


            List<int> existedIds = new List<int>();

        

            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }

            List<ProductImage> removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImage pImage in removeable)
            {
                pImage.Url.DeleteFile(_env.WebRootPath, "img");
                existed.ProductImages.Remove(pImage);
            }


           


           

            foreach (ProductColor procolor in existed.ProductColors)
            {
                if (productVM.ColorsId.Exists(tId => tId == procolor.ColorId)) { _context.ProductColors.Remove(procolor); }
            }


            foreach (int colorId in productVM.ColorsId)
            {
                if (!existed.ProductColors.Any(pt => pt.ColorId == colorId))
                {
                    existed.ProductColors.Add(new ProductColor
                    {
                        ColorId = colorId
                    });
                }
            }
            


           

            TempData["Message"] = "";
            if (productVM.Photos is not null)
            {
                foreach (var photo in productVM.Photos)
                {
                    if (!photo.ValidateType("image/"))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil  </p>";
                        continue;
                    }
                    if (!photo.ValidateSize(600))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil </p> ";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = null,
                        Url = await photo.CreateFile(_env.WebRootPath, "img")
                    });
                }
            }


            existed.Name = productVM.Name;
            existed.Price = productVM.Price;
           
            existed.CategoryId = (int)productVM.CategoryId;
            existed.Description = productVM.Description;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .Include(x => x.ProductImages)
                
                .Include(x => x.ProductColors).ThenInclude(c => c.Color)
                
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound();

            return View(product);

        }
      
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .Include(x => x.ProductImages)
               
                .Include(x => x.ProductColors).ThenInclude(c => c.Color)
                
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound();

            foreach (ProductImage image in product.ProductImages)
            {
                image.Url.DeleteFile(_env.WebRootPath, "img");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private void GetList(CreateProductVM vm)
        {
            vm.Categories = _context.Categories.ToList();
           
            vm.Colors = _context.Colors.ToList();
           
        }
        private void GetList(UpdateProductVM vm)
        {
            vm.Categories = _context.Categories.ToList();
          
            vm.Colors = _context.Colors.ToList();
            
        }
    }
}
