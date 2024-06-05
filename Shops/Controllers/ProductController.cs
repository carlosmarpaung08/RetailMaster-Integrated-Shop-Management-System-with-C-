using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Shops.Services
{

    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductsDbContext _context;

        public ProductController(ProductsDbContext context)
        {
            _context = context;
        }

        // Index action
        public async Task<ActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // Create action
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    product.ImageUrl = await SaveImageAsync(imageFile);
                }

                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }
            return View(product);
        }

        // Edit action
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Jika ada file gambar baru dipilih, simpan dan gunakan URL gambar baru
                if (imageFile != null && imageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        DeleteImage(existingProduct.ImageUrl);
                    }
                    existingProduct.ImageUrl = await SaveImageAsync(imageFile);
                }

                // Jika tidak ada file gambar baru dipilih, gunakan URL gambar saat ini
                if (imageFile == null)
                {
                    existingProduct.ImageUrl = product.ImageUrl;
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.UpdatedAt = DateTime.Now;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Admin");
            }
            return View(product);
        }


        // Delete action
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }

        // Method untuk menyimpan gambar dan mengembalikan URL-nya
        private async Task<string> SaveImageAsync(IFormFile imageFile, string currentImageUrl = null)
        {
            string uniqueFileName;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsDirectory);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                // Jika tidak ada file baru, gunakan URL gambar saat ini
                uniqueFileName = currentImageUrl?.Split('/').LastOrDefault();
            }

            return Path.Combine("/uploads", uniqueFileName);
        }

        // Method untuk menghapus gambar
        private void DeleteImage(string imageUrl)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }
    }
}