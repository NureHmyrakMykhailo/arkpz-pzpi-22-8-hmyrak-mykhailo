using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class BooksPerson
    {
        public int BookId { get; set; }
        public int PersonId { get; set; }
        public int RoleId { get; set; }

        public virtual Book Book { get; set; } = null!;
        public virtual Person Person { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
