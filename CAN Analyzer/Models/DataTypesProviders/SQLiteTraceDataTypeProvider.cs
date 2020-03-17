using CANAnalyzer.Models.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CANAnalyzer.Models.DataTypesProviders
{
    public class SQLiteTraceDataTypeProvider : ITraceDataTypeProvider
    {


        public string TargetFile
        {
            get { return _targetFile; }
            set
            {
                if (value == _targetFile)
                    return;

                if (!CanWorkWithIt(value))
                    throw new ArgumentException("Invalid file type or file does not exist");

                _targetFile = value;
                TargetFileChanged();
            }
        }
        private string _targetFile;

        private void TargetFileChanged()
        {
            context = new TraceContext(TargetFile);
        }

        private TraceContext context;


        public void CloseConnection()
        {
            context.Dispose();
            context = null;
        }

        /// <summary>
        /// Determines if the provider can work with this type of file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Return true if this provider can work with this type of file</returns>
        public bool CanWorkWithIt(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            var splittedPath = filePath.ToLower().Split('.');
            var extension = splittedPath[splittedPath.Length - 1];

            return extension == "sqlite3" || extension == "db";
        }

        /// <summary>
        /// Traces dataset
        /// </summary>
        public IQueryable<TraceModel> Traces => context?.Traces;

        /// <summary>
        /// CanHeaders dataset
        /// </summary>
        public IQueryable<CanHeaderModel> CanHeaders => context?.CanHeaders;
    }
}
