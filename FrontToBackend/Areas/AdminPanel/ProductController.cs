using FrontToBackend.DAL;
using FrontToBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBackend.Areas.AdminPanel
{
    [Area("AdminPanel")]
    public class ProductController : Controller
    {
        
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
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
            ViewBag.Categories = _context.categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
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


            ViewBag.Categories = _context.categories.ToList();
            return View();
        }
    }
}
