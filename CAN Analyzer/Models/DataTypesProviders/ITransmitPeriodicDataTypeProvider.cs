/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models.Databases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.DataTypesProviders
{
    public interface ITransmitPeriodicDataTypeProvider
    {
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
        IQueryable<TracePeriodicModel> TransmitModels { get; }

        /// <summary>
        /// Supported files
        /// </summary>
        string SupportedFiles { get; }


        ITransmitPeriodicDataTypeProvider SaveAs(string path, IEnumerable<TracePeriodicModel> traces);

        Task<ITransmitPeriodicDataTypeProvider> SaveAsAsync(string path, IEnumerable<TracePeriodicModel> traces);


        void Add(TracePeriodicModel entity);
        void AddRange(IEnumerable<TracePeriodicModel> entities);

        void Remove(TracePeriodicModel entity);
        void RemoveRange(IEnumerable<TracePeriodicModel> entities);
        void RemoveAll();
    }
}
