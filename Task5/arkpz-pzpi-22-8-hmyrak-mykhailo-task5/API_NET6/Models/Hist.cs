using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class Hist
    {
        public int HistId { get; set; }
        public DateTime Time { get; set; }
        public int? StatusId { get; set; }
        public string? Comment { get; set; }
        public int ItemId { get; set; }
        public int? ReaderId { get; set; }

        public virtual Item Item { get; set; } = null!;
        public virtual Reader? Reader { get; set; }
        public virtual Status? Status { get; set; }
    }
}
