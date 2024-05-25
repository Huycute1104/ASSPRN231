using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }

        public virtual Book Book { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
