/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CANAnalyzerDevice.Protocol
{
    internal class TransmitCanBData : IValidatableObject
    {
        internal byte CommandId { get; set; } = 5;
        internal byte ChannelId { get; set; }
        internal byte DLC { get; set; }
        internal UInt32 CanId { get; set; }
        internal byte[] data { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (this.CommandId != 5)
                errors.Add(new ValidationResult("Invalid CommandId field."));

            if (ChannelId > 7)
                errors.Add(new ValidationResult("Invalid ChannelId field."));

            if (DLC > 8)
                errors.Add(new ValidationResult("Invalid DLC field."));

            if (CanId > 0x1FFFFFFF)
                errors.Add(new ValidationResult("Invalid CAN ID field."));

            if ((DLC > 0) && (data != null) && (data.Length < DLC))
                errors.Add(new ValidationResult("Invalid data field."));

            return errors;
        }

        public override bool Equals(object obj)
        {
            return obj is TransmitCanBData data &&
                   CommandId == data.CommandId &&
                   ChannelId == data.ChannelId &&
                   DLC == data.DLC &&
                   CanId == data.CanId &&
                   EqualityComparer<byte[]>.Default.Equals(this.data, data.data);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CommandId, ChannelId, DLC, CanId, data);
        }

        public static explicit operator byte[](TransmitCanBData data)
        {
            if (!Validator.TryValidateObject(data, new ValidationContext(data), null, true))
            {
                throw new Exception("invalid structure");
            }

            byte[] result = new byte[8 + data.DLC];

            result[0] = data.CommandId;
            result[1] = data.ChannelId;
            result[1] |= (byte)(data.DLC << 3);
            result[4] = (byte)(data.CanId);
            result[5] = (byte)(data.CanId >> 8);
            result[6] = (byte)(data.CanId >> 16);
            result[7] = (byte)(data.CanId >> 24);

            for (int i = 0; i < data.DLC; i++)
            {
                result[8 + i] = data.data[i];
            }

            return result;
        }

        public static implicit operator TransmitCanBData(byte[] data)
        {
            if (data.Length < 6)
                throw new ArgumentException();

            TransmitCanBData result = new TransmitCanBData();

            result.CommandId = data[0];
            result.ChannelId = (byte)(data[1] & 0x07);
            result.DLC = (byte)((data[1] & 0x78) >> 3);

            result.CanId = Convert.ToUInt32((data[2]) | (data[3] << 8) | (data[4] << 16) | (data[5] << 24));

            if (result.DLC != 0)
                result.data = new byte[result.DLC];

            for (int i = 0; i < result.DLC; i++)
            {
                result.data[i] = data[6 + i];
            }

            if (!Validator.TryValidateObject(result, new ValidationContext(result), null, true))
            {
                throw new Exception("invalid structure");
            }

            return result;
        }
    }
}
