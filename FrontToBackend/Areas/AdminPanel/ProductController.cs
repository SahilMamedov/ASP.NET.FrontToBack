using FrontToBackend.DAL;
using FrontToBackend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBackend.Areas.AdminPanel
{
    [Area("AdminPanel")]
    public class ProductController : Controller
    {
        
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Products.Include(p=>p.category).ToList());
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Product dbProduct = await _context.Products.Include(p=>p.category).FirstOrDefaultAsync(p=>p.id==id);
            if (dbProduct == null) return NotFound();
            return View(dbProduct);
        }


        public  IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.categories.ToList(),"id","Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = _context.categories.ToList();
            if (product.Photo == null)
            {
                ModelState.AddModelError("Photo", "Bosh qoymaq olmaz");
                return View();
            }

            if(!product.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "yalniz shekil olar");
                return View();
            }

            if (!(product.Photo.Length/1024>200))
            {
                ModelState.AddModelError("Photo", "Olcu duzgun deyil");
                return View();
            }

            string filename =Guid.NewGuid().ToString()+product.Photo.FileName;

            string path = Path.Combine(_env.WebRootPath, "img",filename);
            using(FileStream stream =new FileStream(path,FileMode.Create))
            {
                product.Photo.CopyTo(stream);
            }

            Product newProduct = new Product
            {
                Price = product.Price,
                Name = product.Name,
                CategoryId = product.CategoryId,
                imgUrl = filename
            };
          await _context.Products.AddAsync(newProduct);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
