using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzerDevices.Devices.DeviceChannels.Protocols.CanAnalyzer
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

	internal static class BitrateTypeExtension
	{
		public static int Value(this BitrateType data)
		{
			switch(data)
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
