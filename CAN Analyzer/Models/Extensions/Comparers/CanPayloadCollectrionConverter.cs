using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Data;

namespace CANAnalyzer.Models.Extensions.Comparers
{
    public class CanPayloadCollectrionConverter : Comparer<ObservableCollection<byte>>
    {
        public override int Compare([AllowNull] ObservableCollection<byte> x, [AllowNull] ObservableCollection<byte> y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null && y != null)
                return -1;

            if (x != null && y == null)
                return 1;

            if (x.Count > y.Count)
                return 1;

            if (x.Count < y.Count)
                return -1;

            if(x.Count == y.Count)
            {
                for(int i = 0; i < x.Count; i++)
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
