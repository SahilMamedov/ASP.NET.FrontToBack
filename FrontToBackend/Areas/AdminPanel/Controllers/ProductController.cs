using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FrontToBackend.DAL;
using FrontToBackend.Extentions;
using FrontToBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FrontToBackend.Areas.AdminPanel.Controllers
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
            return View(_context.Products.Include(p => p.category).ToList());
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Product dbProduct = await _context.Products.Include(p => p.category).FirstOrDefaultAsync(p => p.id == id);
            if (dbProduct == null) return NotFound();
            return View(dbProduct);
        }


        public IActionResult Create()
        {
            ViewBag.Categories = _context.categories.ToList();
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

            if (!product.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "yalniz shekil olar");
                return View();
            }

            if (product.Photo.ValidSize(500))
            {
                ModelState.AddModelError("Photo", "Olcu duzgun deyil");
                return View();
            }


            Product newProduct = new Product
            {
                Price = product.Price,
                Name = product.Name,
                CategoryId = product.CategoryId,
                imgUrl = product.Photo.SaveImage(_env,"img")
            };
            await _context.Products.AddAsync(newProduct);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Product dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "img",dbProduct.imgUrl);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            _context.Products.Remove(dbProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }




        public async Task<IActionResult> Update(int? id)
        {

            ViewBag.Categories = _context.categories.ToList();

            if (id == null) return NotFound();
            Product dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return NotFound();


            return View(dbProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Product product)
        {

            ViewBag.Categories = _context.categories.ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }
            Product dbProduct = _context.Products.FirstOrDefault(c => c.id == product.id);


            if (product.Photo == null)
            {
                ModelState.AddModelError("Photo", "bosh olmaz");

                return View();
            }
           
            if (!product.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "yalniz shekil olar");
                return View();
            }
            if (product.Photo.ValidSize(500))
            {
                ModelState.AddModelError("Photo", "olcunu duzgun verin");
                return View();
            }

            string path = Path.Combine(_env.WebRootPath, "img", dbProduct.imgUrl);
            System.IO.File.Delete(path);


            dbProduct.Photo = product.Photo;
            dbProduct.Name = product.Name;
            dbProduct.imgUrl = product.Photo.SaveImage(_env, "img");
            dbProduct.CategoryId = product.CategoryId;
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
