using Shops.Models;

public class ShoppingCartDetail
{
    public int Id { get; set; }
    public int ShoppingCartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public Product Product { get; set; } // Ini adalah properti Product yang dihubungkan
}