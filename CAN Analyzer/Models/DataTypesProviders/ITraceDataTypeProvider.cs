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


        ITraceDataTypeProvider SaveAs(string path, IQueryable<TraceModel> traces, IQueryable<CanHeaderModel> canHeaders);
    }
}
