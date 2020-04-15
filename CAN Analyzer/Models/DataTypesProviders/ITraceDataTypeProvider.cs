/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzer.Models.Databases;

namespace CANAnalyzer.Models.DataTypesProviders
{
    public interface ITraceDataTypeProvider
    {
        void GenerateFile(string file);
        void CloseConnection();

        string TargetFile { get; set; }


        void SaveChanges();


        /// <summary>
        /// Determines if the provider can work with this type of file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Return true if this provider can work with this type of file</returns>
        bool CanWorkWithIt(string filePath);


        /// <summary>
        /// Traces dataset
        /// </summary>
        IQueryable<TraceModel> Traces { get; }

        /// <summary>
        /// CanHeaders dataset
        /// </summary>
        IQueryable<CanHeaderModel> CanHeaders { get; }

        /// <summary>
        /// Supported files
        /// </summary>
        string SupportedFiles { get; }


        ITraceDataTypeProvider SaveAs(string path, IEnumerable<TraceModel> traces, IEnumerable<CanHeaderModel> canHeaders);

        Task<ITraceDataTypeProvider> SaveAsAsync(string path, IEnumerable<TraceModel> traces, IEnumerable<CanHeaderModel> canHeaders);

        
        void Add(TraceModel entity);
        void Add(CanHeaderModel entity);


        void AddRange(IEnumerable<TraceModel> entities);
        void AddRange(IEnumerable<CanHeaderModel> entities);

        void Remove(TraceModel entity);
        void Remove(CanHeaderModel entity);


        void RemoveRange(IEnumerable<TraceModel> entities);
        void RemoveRange(IEnumerable<CanHeaderModel> entities);
    }
}
