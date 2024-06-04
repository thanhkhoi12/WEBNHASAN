using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEBNHASAN.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WEBNHASAN.Controllers
{
    public class ProductControlller : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hosting;
        public ProductControlller(ApplicationDbContext db, IHostingEnvironment hosting)
        {
            _db = db;
            _hosting = hosting;
        }
        public IActionResult Index()
        {
            var productList = _db.Products.Include(x => x.Category).ToList();
            return View(productList);
        }
        public IActionResult Add()
        {
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }    
     
public IActionResult Add(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid) 
            {
                if (ImageUrl != null)
                {
                    product.ImageUrl = SaveImage(ImageUrl);
                }
                _db.Products.Add(product);
                _db.SaveChanges();
                TempData["success"] = "Product inserted success";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }
        public IActionResult Update(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View(product);
        }
        [HttpPost]
        public IActionResult Update(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = _db.Products.Find(product.Id);
                if (ImageUrl != null)
                {
                    product.ImageUrl = SaveImage(ImageUrl);
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        var oldFilePath = Path.Combine(_hosting.WebRootPath,
                        existingProduct.ImageUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }
                else

                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                _db.SaveChanges();
                TempData["success"] = "Product updated success";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }
        private string SaveImage(IFormFile image)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var path = Path.Combine(_hosting.WebRootPath, @"images/products");
            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }
            return @"images/products/" + filename;
        }
        public IActionResult Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            if (!String.IsNullOrEmpty(product.ImageUrl))
            {
                var oldFilePath = Path.Combine(_hosting.WebRootPath, product.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);

                }
            }
            _db.Products.Remove(product);
            _db.SaveChanges();
            TempData["success"] = "Product deleted success";
            return RedirectToAction("Index");
        }
    }
}
