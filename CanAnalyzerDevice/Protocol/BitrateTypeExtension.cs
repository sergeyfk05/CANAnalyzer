/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevice.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzerDevice.Protocol
{

	internal static class BitrateTypeExtension
	{
		public static int Value(this BitrateType data)
		{
			switch (data)
			{
				case BitrateType.undefined:
					return 0;
				case BitrateType.kpbs10:
					return 10;
				case BitrateType.kpbs20:
					return 20;
				case BitrateType.kpbs50:
					return 50;
				case BitrateType.kpbs83:
					return 83;
				case BitrateType.kpbs100:
					return 100;
				case BitrateType.kpbs125:
					return 125;
				case BitrateType.kpbs250:
					return 250;
				case BitrateType.kpbs500:
					return 500;
				case BitrateType.kpbs800:
					return 800;
				case BitrateType.kpbs1000:
					return 1000;
			}
			return 0;
		}
	}
}
