using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class Token
    {
        public int TokenId { get; set; }
        public string Token1 { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
