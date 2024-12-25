using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class Item
    {
        public Item()
        {
            Hists = new HashSet<Hist>();
        }

        public int ItemId { get; set; }
        public int BookId { get; set; }
        public int? ReaderId { get; set; }
        public bool Available { get; set; }
        public string? Description { get; set; }

        public virtual Book Book { get; set; } = null!;
        public virtual Reader? Reader { get; set; }
        public virtual ICollection<Hist> Hists { get; set; }
    }
}
