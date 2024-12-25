using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class ClimatMonitor
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public double? Temperature { get; set; }
        public double? Wet { get; set; }
        public double? Pressure { get; set; }
    }
}
