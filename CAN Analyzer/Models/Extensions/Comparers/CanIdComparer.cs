using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.Extensions.Comparers
{
    public class CanIdComparer : Comparer<int>
    {

        /// <inheritdoc />
        public override int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }
}
