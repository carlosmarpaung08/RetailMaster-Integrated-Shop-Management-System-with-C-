namespace Shops.Models
{
    public class OrderViewModel
    {
        public string UserName { get; set; }
        public ICollection<TransactionDetail> ShoppingCartDetails { get; set; } // Mengubah tipe properti menjadi ICollection<TransactionDetail>
        public decimal Total { get; set; }
        public string PaymentProofUrl { get; set; }
    }
}
