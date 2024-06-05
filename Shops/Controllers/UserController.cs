using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shops.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Shops.Areas.Identity.Data;

namespace Shops.Controllers
{
    [Authorize(Roles = null)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProductsDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager, ProductsDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Mendapatkan pengguna yang sedang login
            var user = await _userManager.GetUserAsync(User);

            // Handle jika pengguna tidak ditemukan
            if (user == null)
            {
                return NotFound();
            }

            // Memuat daftar produk dari database
            var products = await _context.Products.ToListAsync();

            // Meneruskan daftar produk ke tampilan
            return View(products);
        }

        public async Task<IActionResult> Transactions()
        {
            // Mendapatkan pengguna yang sedang login
            var user = await _userManager.GetUserAsync(User);

            // Handle jika pengguna tidak ditemukan
            if (user == null)
            {
                return NotFound();
            }

            // Mendapatkan ID pengguna yang sedang login
            var userId = user.Id;

            // Memuat daftar transaksi pengguna dari database
            var transactions = await _context.Transactions
                .Include(t => t.TransactionDetails)
                .ThenInclude(td => td.Product)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Meneruskan daftar transaksi ke tampilan
            return View(transactions);
        }
    }
}
