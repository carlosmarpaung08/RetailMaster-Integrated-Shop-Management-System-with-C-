namespace Shops.Models
{
    public class TransactionDetail
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] PaymentProofImage { get; set; } // Add this property for storing payment proof image data
        public Product Product { get; set; }
    }
}
