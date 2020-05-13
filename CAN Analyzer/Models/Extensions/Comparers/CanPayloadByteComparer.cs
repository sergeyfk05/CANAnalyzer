using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CANAnalyzer.Models.Extensions.Comparers
{
    public class CanPayloadByteComparer : Comparer<byte[]>
    {
        public override int Compare([AllowNull] byte[] x, [AllowNull] byte[] y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null && y != null)
                return -1;

            if (x != null && y == null)
                return 1;

            if (x.Length > y.Length)
                return 1;

            if (x.Length < y.Length)
                return -1;

            if (x.Length == y.Length)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] > y[i])
                        return 1;

                    if (x[i] < y[i])
                        return -1;
                }
            }

            return 0;
        }
    }
}
