/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System.Collections.Generic;

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
