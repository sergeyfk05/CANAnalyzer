using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CANAnalyzerDevices.Devices.DeviceChannels.Protocols.CanAnalyzer
{
    internal class UpdateChannelInfo
    {
        [Range(2, 2)]
        internal byte CommandId { get; set; } = 2;

        [Range(0, 7)]
        internal byte ChannelId { get; set; }

        [Range(0, 2)]
        internal CanState Status { get; set; }

        [Range(0, 10)]
        internal BitrateType BitrateType { get; set; }

        public static explicit operator byte[](UpdateChannelInfo data)
        {
            if (!Validator.TryValidateObject(data, new ValidationContext(data), null, true))
            {
                throw new Exception("invalid structure");
            }

            byte[] result = new byte[3];
            result[0] = data.CommandId;
            result[1] = data.ChannelId;
            result[1] |= (byte)((int)data.Status << 3);
            result[2] = (byte)data.BitrateType;

            return result;
        }

        public static implicit operator UpdateChannelInfo(byte[] data)
        {
            if (data.Length < 3)
                throw new ArgumentException();

            UpdateChannelInfo result = new UpdateChannelInfo();

            result.CommandId = data[0];
            result.ChannelId = (byte)(data[1] & 0x07);
            result.Status = (CanState)((data[1] & 0x38) >> 3);
            result.BitrateType = (BitrateType)data[2];

            if (!Validator.TryValidateObject(result, new ValidationContext(result), null, true))
            {
                throw new Exception("invalid structure");
            }

            return result;
        }
    }
}
