using System;
using System.ComponentModel.DataAnnotations;

namespace Shops.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Set nilai awal dengan waktu UTC saat ini

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Set nilai awal dengan waktu UTC saat ini
        public string? ImageUrl { get; set; }
    }
}
