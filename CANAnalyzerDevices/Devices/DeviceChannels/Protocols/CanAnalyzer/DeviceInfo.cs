﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;

namespace CANAnalyzerDevices.Devices.DeviceChannels.Protocols.CanAnalyzer
{
    internal class DeviceInfo
	{ 
		public byte CommandId;

		[MinLength(3), MaxLength(3)]
		public byte[] UID;

		[Range(0, 7)]
		public byte channels;

		public bool isSupportCanB;

		public static explicit operator byte[](DeviceInfo data)
		{
			byte[] result = new byte[5];

			if (!Validator.TryValidateObject(data, new ValidationContext(data), null, true))
			{
				throw new Exception("invalid structure");
			}

			result[0] = data.CommandId;
			result[1] = data.UID[0];
			result[2] = data.UID[1];
			result[3] = data.UID[2];

			result[4] = data.channels;
			result[4] |= (byte)(data.isSupportCanB ? 1 : 0 << 4);

			return result;
		}

		public static implicit operator DeviceInfo(byte[] data)
		{
			if (data.Length < 5)
				throw new ArgumentException();

			DeviceInfo result = new DeviceInfo();
			result.CommandId = data[0];
			
			result.UID = new byte[3];
			result.UID[0] = data[1];
			result.UID[1] = data[2];
			result.UID[2] = data[3];

			result.channels = (byte)(data[4] & 0x03);
			result.isSupportCanB = Convert.ToBoolean((data[4] & 0x08) >> 3);

			if (!Validator.TryValidateObject(result, new ValidationContext(result), null, true))
			{
				throw new Exception("invalid structure");
			}

			return result;
		}

	}
}
