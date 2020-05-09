/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models.Databases;
using System.ComponentModel;
using System.Linq;

namespace CANAnalyzer.Models.TraceFilters
{
    public interface ITraceFilter : INotifyPropertyChanged
    {
        bool IsActive { get; set; }

        string DisplayName { get; }

        IQueryable<TraceModel> Filter(IQueryable<TraceModel> source);

        /// <summary>
        /// Filter one object
        /// </summary>
        /// <param name="source"></param>
        /// <returns>true = blocked;</returns>
        bool FilterOne(TraceModel source);
    }
}
