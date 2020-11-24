using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CANAnalyzerDevice.Protocol
{
    internal class TransmitCanAData : IValidatableObject
    {
        internal byte CommandId { get; set; } = 4;
        internal byte ChannelId { get; set; }
        internal byte DLC { get; set; }
        internal UInt16 CanId { get; set; }
        internal byte[] data { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (this.CommandId != 4)
                errors.Add(new ValidationResult("Invalid CommandId field."));

            if (ChannelId > 7)
                errors.Add(new ValidationResult("Invalid ChannelId field."));

            if (DLC > 8)
                errors.Add(new ValidationResult("Invalid DLC field."));

            if(CanId > 0x7FF)
                errors.Add(new ValidationResult("Invalid CAN ID field."));

            if((DLC > 0) && (data != null) && (data.Length < DLC))
                errors.Add(new ValidationResult("Invalid data field."));

            return errors;
        }

        public override bool Equals(object obj)
        {
            return obj is TransmitCanAData data &&
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

        public static explicit operator byte[](TransmitCanAData data)
        {
            if (!Validator.TryValidateObject(data, new ValidationContext(data), null, true))
            {
                throw new Exception("invalid structure");
            }

            byte[] result = new byte[4 + data.DLC];

            result[0] = data.CommandId;
            result[1] = data.ChannelId;
            result[1] |= (byte)(data.DLC << 3);
            result[2] = (byte)(data.CanId);
            result[3] = (byte)(data.CanId >> 8);

            for(int i = 0; i < data.DLC; i++)
            {
                result[4 + i] = data.data[i];
            }

            return result;
        }

        public static implicit operator TransmitCanAData(byte[] data)
        {
            if (data.Length < 4)
                throw new ArgumentException();

            TransmitCanAData result = new TransmitCanAData();

            result.CommandId = data[0];
            result.ChannelId = (byte)(data[1] & 0x07);
            result.DLC = (byte)((data[1] & 0x78) >> 3);

            result.CanId = Convert.ToUInt16((data[2]) | data[3] << 8);

            if (result.DLC != 0)
                result.data = new byte[result.DLC];

            for (int i = 0; i < result.DLC; i++)
            {
                result.data[i] = data[4 + i];
            }

            if (!Validator.TryValidateObject(result, new ValidationContext(result), null, true))
            {
                throw new Exception("invalid structure");
            }

            return result;
        }
    }
}
