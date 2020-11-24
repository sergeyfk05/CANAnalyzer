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
