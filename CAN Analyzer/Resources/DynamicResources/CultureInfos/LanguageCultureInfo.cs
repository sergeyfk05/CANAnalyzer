﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using DynamicResource;

namespace CANAnalyzer.Resources.DynamicResources
{
    public class LanguageCultureInfo : BaseCultureInfo
    {
        public LanguageCultureInfo(string name) : base(name)
        { }

        public override string ToString()
        {
            return (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource($"#{Name}");
        }
    }
}
