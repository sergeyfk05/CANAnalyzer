using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CANAnalyzerDevice.Protocol
{
    internal class RequestChannelInfo
    {
        [Range(2,2)]
        internal byte CommandId { get; set; } = 2;

        [Range(0,7)]
        internal byte ChannelId { get; set; }

        public static explicit operator byte[](RequestChannelInfo data)
        {
            if (!Validator.TryValidateObject(data, new ValidationContext(data), null, true))
            {
                throw new Exception("invalid structure");
            }

            byte[] result = new byte[3];
            result[0] = data.CommandId;
            result[1] = data.ChannelId;

            return result;
        }

        public static implicit operator RequestChannelInfo(byte[] data)
        {
            if (data.Length < 2)
                throw new ArgumentException();

            RequestChannelInfo result = new RequestChannelInfo();

            result.CommandId = data[0];
            result.ChannelId = (byte)(data[1] & 0x07);

            if (!Validator.TryValidateObject(result, new ValidationContext(result), null, true))
            {
                throw new Exception("invalid structure");
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is RequestChannelInfo info &&
                   CommandId == info.CommandId &&
                   ChannelId == info.ChannelId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CommandId, ChannelId);
        }
    }
}
