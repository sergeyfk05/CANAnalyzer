using System;
using System.Linq;

namespace InfluxDBChannelProxy.Extensions
{
    public static class ByteArrayHelper
    {
        public static byte[] BinaryAnd(this byte[] arr1, byte[] arr2)
        {
            if (arr1.Length == arr2.Length)
            {
                byte[] result = new byte[arr1.Length];
                for (int i = 0; i < arr1.Length; i++)
                {
                    result[i] = (byte)((int)arr1[i] & (int)arr2[i]);
                }
                return result;
            }
            else if (arr1.Length < arr2.Length)
            {
                byte[] result = new byte[arr2.Length];

                for (int i = 0; i < arr1.Length; i++)
                {
                    result[i] = (byte)((int)arr1[i] & (int)arr2[i]);
                }
                for (int i = arr1.Length; i < arr2.Length; i++)
                {
                    result[i] = arr2[i];
                }
                return result;
            }
            else if (arr2.Length < arr1.Length)
            {
                byte[] result = new byte[arr1.Length];

                for (int i = 0; i < arr2.Length; i++)
                {
                    result[i] = (byte)((int)arr1[i] & (int)arr2[i]);
                }
                for (int i = arr2.Length; i < arr1.Length; i++)
                {
                    result[i] = arr1[i];
                }
                return result;
            }
            return new byte[0];
        }

        public static bool IsEquals(this byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;

            for(int i=0;i<arr1.Length;i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }
            return true;
        }
        public static byte[] StringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
