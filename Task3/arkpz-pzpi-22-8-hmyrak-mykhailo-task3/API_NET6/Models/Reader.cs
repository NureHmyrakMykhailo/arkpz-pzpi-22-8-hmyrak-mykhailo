using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class Reader
    {
        public Reader()
        {
            Hists = new HashSet<Hist>();
            Items = new HashSet<Item>();
        }

        public int ReaderId { get; set; }
        public string Name { get; set; } = null!;
        public int? Class { get; set; }
        public string? StudentCard { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public virtual ICollection<Hist> Hists { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
