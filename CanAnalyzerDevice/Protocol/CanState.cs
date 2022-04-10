/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzerDevice.Protocol
{
    internal enum CanState
    {
        Closed = 0,
        OpenedNormal = 1,
        OpenedListenOnly = 2
    }
}
