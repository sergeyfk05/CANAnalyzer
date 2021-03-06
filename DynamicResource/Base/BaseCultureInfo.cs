﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicResource
{
    public abstract class BaseCultureInfo
    {
        public BaseCultureInfo(string name)
        {
            if (CulturesInfo.FirstOrDefault(x => x.Name == name) != null)
                throw new ArgumentException($"{this.ToString()} instance with {name} already created.");

            Name = name;
            CulturesInfo.Add(this);
        }

        public string Name { get; private set; }


        public static List<BaseCultureInfo> CulturesInfo => _culturesInfo ?? (_culturesInfo = new List<BaseCultureInfo>());
        private static List<BaseCultureInfo> _culturesInfo;
    }
}
