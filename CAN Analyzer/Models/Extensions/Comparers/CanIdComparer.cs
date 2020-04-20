﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
