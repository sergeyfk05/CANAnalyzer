/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CANAnalyzerDataModels
{
    internal static class ObservableCollectionExtension
    {
        internal static byte[] ToByteArray(this ObservableCollection<byte> collection)
        {
            byte[] result = new byte[collection.Count];

            int i = 0;
            foreach(var el in collection)
            {
                result[i++] = el;
            }

            return result;
        }

        internal static ObservableCollection<byte> ToObservableCollection(this byte[] array)
        {
            ObservableCollection<byte> result = new ObservableCollection<byte>();
            foreach(var el in array)
            {
                result.Add(el);
            }

            return result;
        }

        internal static ObservableCollection<byte> CreateEmpty(uint c)
        {
            ObservableCollection<byte> result = new ObservableCollection<byte>();
            for (uint i = 0; i < c; i++)
            {
                result.Add(0);
            }

            return result;
        }

        internal static int RemoveAll<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }
    }
}
