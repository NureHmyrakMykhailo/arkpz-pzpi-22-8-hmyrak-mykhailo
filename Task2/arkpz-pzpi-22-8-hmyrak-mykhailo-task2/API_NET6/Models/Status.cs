using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class Status
    {
        public Status()
        {
            Hists = new HashSet<Hist>();
        }

        public int StatusId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Hist> Hists { get; set; }
    }
}
