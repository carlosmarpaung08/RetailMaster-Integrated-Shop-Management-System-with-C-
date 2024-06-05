using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Shops.Areas.Identity.Data;

namespace Shops.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductsDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ProductsDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Menampilkan keranjang belanja
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            return View(cart);
        }

        // Menambahkan produk ke keranjang belanja
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Login", "Account");
            }

            if (product.Stock < quantity)
            {
                // Handle insufficient stock
                ModelState.AddModelError("", "Insufficient stock.");
                return RedirectToAction("Index");
            }

            var cart = await _context.ShoppingCarts.Include(c => c.ShoppingCartDetails)
                                                   .ThenInclude(d => d.Product)
                                                   .FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (cart == null)
            {
                cart = new ShoppingCart { UserId = user.Id };
                _context.ShoppingCarts.Add(cart);
            }

            var cartItem = cart.ShoppingCartDetails.FirstOrDefault(d => d.ProductId == productId);
            if (cartItem != null)
            {
                // If item already exists in cart, increase quantity
                cartItem.Quantity += quantity;
            }
            else
            {
                // If item does not exist in cart, add new detail transaction
                cartItem = new ShoppingCartDetail { ProductId = productId, Quantity = quantity };
                cart.ShoppingCartDetails.Add(cartItem);
            }

            // Kurangi stok produk
            product.Stock -= quantity;

            await _context.SaveChangesAsync();

            return RedirectToAction("Order"); // Redirect ke halaman Order setelah menambahkan ke keranjang
        }

        // Menghapus produk dari keranjang belanja
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartDetails)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart != null)
            {
                var cartItem = cart.ShoppingCartDetails.FirstOrDefault(d => d.ProductId == productId);
                if (cartItem != null)
                {
                    // Hapus seluruh kuantitas produk dari keranjang belanja
                    _context.ShoppingCartDetails.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }
            }

            // Redirect kembali ke halaman Order setelah menghapus
            return RedirectToAction("Order");
        }

        // Checkout action untuk menyelesaikan pembelian
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || cart.ShoppingCartDetails.Count == 0)
            {
                return RedirectToAction("Index");
            }

            // Ambil file gambar dari permintaan
            var paymentProofImage = Request.Form.Files["paymentProof"];

            // Simpan gambar ke dalam byte array
            byte[] paymentProofImageData = null;
            if (paymentProofImage != null && paymentProofImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    paymentProofImage.CopyTo(memoryStream);
                    paymentProofImageData = memoryStream.ToArray();
                }
            }

            // Buat transaksi baru
            var transaction = new Transaction
            {
                UserId = user.Id,
                TotalAmount = cart.ShoppingCartDetails.Sum(d => d.Product.Price * d.Quantity),
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Pindahkan detail keranjang belanja ke detail transaksi
            foreach (var item in cart.ShoppingCartDetails)
            {
                var transactionDetail = new TransactionDetail
                {
                    TransactionId = transaction.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price,
                    TotalAmount = item.Product.Price * item.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    PaymentProofImage = paymentProofImageData // Simpan gambar ke dalam kolom PaymentProofImage
                };

                _context.TransactionDetails.Add(transactionDetail);
            }

            // Hapus keranjang belanja
            _context.ShoppingCarts.Remove(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home"); // Redirect ke halaman home setelah checkout
        }

        // Menampilkan halaman Order
        public async Task<IActionResult> Order()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new ShoppingCart { UserId = user.Id, ShoppingCartDetails = new List<ShoppingCartDetail>() };
            }

            // Tampilkan halaman Order dengan objek ShoppingCart, meskipun kosong
            return View("Order", cart);
        }

        // Menampilkan halaman pembayaran
        public IActionResult Bayar()
        {
            return View("Bayar");
        }
    }
}
