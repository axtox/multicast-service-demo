using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receiver.Model
{
    public class Statistics
    {
        public long Average { get; set; }
        public double StandardDeviation { get; set; }
        public int Mode { get; set; }
        public int Median { get; set; }
    }
}
