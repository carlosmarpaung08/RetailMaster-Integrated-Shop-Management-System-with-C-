using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using Shops.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Shops.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ProductsDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ILogger<AdminController> logger, ProductsDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminOrder()
        {
            var transactions = await _context.Transactions
                .Include(t => t.TransactionDetails)
                .ThenInclude(td => td.Product)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var orderViewModels = new List<OrderViewModel>();

            foreach (var transaction in transactions)
            {
                // Mendapatkan nama pengguna berdasarkan UserId dari transaksi
                var user = await _userManager.FindByIdAsync(transaction.UserId);
                var userName = user != null ? user.FirstName : "Unknown";

                // Membuat OrderViewModel dan menambahkannya ke daftar
                var orderViewModel = new OrderViewModel
                {
                    UserName = userName,
                    ShoppingCartDetails = transaction.TransactionDetails,
                    Total = transaction.TotalAmount,
                };
                orderViewModels.Add(orderViewModel);
            }

            return View(orderViewModels);
        }

        // Metode untuk menampilkan daftar semua pengguna
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
