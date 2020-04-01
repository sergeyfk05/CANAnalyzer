/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzer.Models.Databases;

namespace CANAnalyzer.Models.TraceFilters
{
    public interface ITraceFilter : INotifyPropertyChanged
    {
        bool IsActive { get; set; }

        string DisplayName { get; }

        IQueryable<TraceModel> Filter(IQueryable<TraceModel> source);
    }
}
