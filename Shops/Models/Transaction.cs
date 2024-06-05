using System;
using System.Collections.Generic;

namespace Shops.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<TransactionDetail> TransactionDetails { get; set; }

        public Transaction()
        {
            TransactionDetails = new List<TransactionDetail>();
        }
    }
}
