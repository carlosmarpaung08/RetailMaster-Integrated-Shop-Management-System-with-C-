using System.Collections.Generic;

namespace Shops.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ICollection<ShoppingCartDetail> ShoppingCartDetails { get; set; } // Tambahkan definisi untuk ShoppingCartDetails

        // Tambahkan constructor untuk inisialisasi koleksi
        public ShoppingCart()
        {
            ShoppingCartDetails = new List<ShoppingCartDetail>();
        }
    }
}
