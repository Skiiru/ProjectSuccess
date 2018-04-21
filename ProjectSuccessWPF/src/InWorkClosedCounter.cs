using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuccessWPF
{
    class InWorkClosedCounter
    {
        public int InWorkCount { get; set; }
        public int ClosedCount { get; set; }

        public InWorkClosedCounter()
        {
            InWorkCount = 0;
            ClosedCount = 0;
        }

        public void Clear()
        {
            InWorkCount = 0;
            ClosedCount = 0;
        }
    }
}
