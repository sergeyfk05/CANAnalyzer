using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzerDevices.Devices.DeviceChannels
{
    public class TransmitData : IValidatableObject
    {
        public int CanId { get; set; }

        public bool IsExtId { get; set; } = false;

        public int DLC { get; set; } = 8;

        public byte[] Payload { get; set; }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if(DLC <= 0 || DLC > 8)
                errors.Add(new ValidationResult("Invalid DLC. It should be between 1 and 8."));

            if (Payload.Length < DLC)
                errors.Add(new ValidationResult("Invalid payload. Payload array length less than DLC or invalid DLC."));

            if(CanId < 0)
                errors.Add(new ValidationResult("Не указано имя"));

            if (IsExtId)
            {
                if(CanId > 0x1FFFFFFF)
                    errors.Add(new ValidationResult("Invalid CanId. It should be between 0 and 0x1FFF FFFF."));
            }
            else
            {
                if(CanId > 0x7FF)
                    errors.Add(new ValidationResult("Invalid CanId. It should be between 0 and 0x7FF."));
            }


            return errors;

        }

        public override bool Equals(object obj)
        {
            if(obj is TransmitData data)
            {
                if ((data.CanId != CanId) || (data.IsExtId != IsExtId) || (data.DLC != DLC))
                    return false;

                if (Payload == null || data.Payload == null)
                {
                    if (Payload != data.Payload)
                        return false;
                }
                else
                {
                    if (Payload.Length != data.Payload.Length)
                        return false;


                    for (int i = 0; i < Payload.Length; i++)
                        if (data.Payload[i] != Payload[i]) return false;
                }


                return true;
            }
            else { return false; }
        }
    }
}
