using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CANAnalyzer.Models.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static byte[] ToByteArray(this ObservableCollection<byte> collection)
        {
            byte[] result = new byte[collection.Count];

            int i = 0;
            foreach(var el in collection)
            {
                result[i++] = el;
            }

            return result;
        }

        public static ObservableCollection<byte> ToObservableCollection(this byte[] array)
        {
            ObservableCollection<byte> result = new ObservableCollection<byte>();
            foreach(var el in array)
            {
                result.Add(el);
            }

            return result;
        }
    }
}
