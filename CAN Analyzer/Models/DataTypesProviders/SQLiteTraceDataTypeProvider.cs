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

        public async void SaveChanges()
        {
            if (!File.Exists(TargetFile))
                throw new ArgumentException("TargetFile does not exist.");
            
            await context.SaveChangesAsync();
        }

        public string TargetFile
        {
            get { return _targetFile; }
            set
            {
                if (value == _targetFile)
                    return;

                if (!File.Exists(value))
                    throw new ArgumentException("File does not exist.");

                if (!CanWorkWithIt(value))
                    throw new ArgumentException("Invalid file type.");

                _targetFile = value;
                TargetFileChanged();
            }
        }
        private string _targetFile;

        private void TargetFileChanged()
        {
            context = new TraceContext(TargetFile);
        }


        public void CloseConnection()
        {
            context?.Dispose();
            context = null;
        }

        /// <summary>
        /// Determines if the provider can work with this type of file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Return true if this provider can work with this type of file</returns>
        public bool CanWorkWithIt(string filePath)
        {
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


        private TraceContext context;
    }
}
