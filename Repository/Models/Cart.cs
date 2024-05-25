using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class Cart
    {
        public int CartId { get; set; }
        public int? BookId { get; set; }
        public int? UsersId { get; set; }

        public virtual Book? Book { get; set; }
        public virtual User? Users { get; set; }
    }
}
