using Microsoft.EntityFrameworkCore;

namespace Shops.Models
{
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Transaction> Transactions { get; set; } // Tambahkan definisi untuk Transaction

        public DbSet<TransactionDetail> TransactionDetails { get; set; } // Tambahkan definisi untuk TransactionDetail

        public DbSet<ShoppingCart> ShoppingCarts { get; set; } // Tambahkan definisi untuk ShoppingCart

        public DbSet<ShoppingCartDetail> ShoppingCartDetails { get; set; } // Tambahkan definisi untuk ShoppingCartDetail
    }
}
