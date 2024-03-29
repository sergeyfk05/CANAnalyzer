﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzerDevice.Protocol
{
    internal enum BitrateType
    {
		undefined = 0,
		kpbs10 = 1,
		kpbs20 = 2,
		kpbs50 = 3,
		kpbs83 = 4,
		kpbs100 = 5,
		kpbs125 = 6,
		kpbs250 = 7,
		kpbs500 = 8,
		kpbs800 = 9,
		kpbs1000 = 10
	}

}
