/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CANAnalyzerDevices.Devices.DeviceChannels
{
    public class ReceivedData : TransmitData, IValidatableObject
    {

        public double Time { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result =  new List<ValidationResult>(base.Validate(validationContext));

            if (Time < 0)
                result.Add(new ValidationResult("Invalid Time. It should be more or equals than zero."));

            return result;
        }


        public override bool Equals(object obj)
        {
            if(obj is ReceivedData data)
            {
                return ((Math.Abs(data.Time - Time) < 0.005) && (base.Equals(obj)));
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CanId, IsExtId, DLC, Payload, Time);
        }
    }
}
