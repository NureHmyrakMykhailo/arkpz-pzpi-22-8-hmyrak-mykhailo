using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class Param
    {
        public int Id { get; set; }
        public double? TempMax { get; set; }
        public double? TempMin { get; set; }
        public double? WetMax { get; set; }
        public double? WetMin { get; set; }
    }
}
