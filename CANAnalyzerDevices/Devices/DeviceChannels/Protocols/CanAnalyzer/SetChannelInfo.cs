using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Text;

namespace CANAnalyzerDevices.Devices.DeviceChannels.Protocols.CanAnalyzer
{
    internal class SetChannelInfo
    {
        [Range(3, 3)]
        internal byte CommandId { get; set; } = 3;

        [Range(0, 7)]
        internal byte ChannelId { get; set; }

        [Range(0, 2)]
        internal CanState Status { get; set; }

        [Range(0, 10)]
        internal BitrateType BitrateType { get; set; }

        public static explicit operator byte[](SetChannelInfo data)
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

        public static implicit operator SetChannelInfo(byte[] data)
        {
            if (data.Length < 3)
                throw new ArgumentException();

            SetChannelInfo result = new SetChannelInfo();

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
