using CANAnalyzer.Models.Databases;
using CANAnalyzer.Models.DataTypesProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models
{
    public static class CanHackerDataTypeConverter
    {
        public static void convert(string s = @"D:\Projects\MC_projects\Машина\движение с магнитолой.trc", string destination = @"D:\Projects\MC_projects\Машина\2.traceDB")
        {
            ITraceDataTypeProvider provider = new SQLiteTraceDataTypeProvider();
            provider = provider.SaveAs(destination, null, null);

            string[] source = File.ReadAllLines(s);
            double prevtime = -1;
            double offset = 0;
            for (int i = 1; i < source.Length; i++)
            {
                var a = source[i];
                a = a.Replace("  ", " ");
                a = a.Replace("  ", " ");
                a = a.Replace("  ", " ");
                a = a.Replace("  ", " ");
                a = a.Replace("  ", " ");
                a = a.Replace("  ", " ");
                a = a.Trim();



                var paths = a.Split(' ');
                if ((Convert.ToInt32(paths[2]) + 3) != paths.Length)
                {
                    continue;
                }

                int canid = Convert.ToInt32(paths[1], 16);
                int dlc = Convert.ToInt32(paths[2]);

                if (provider.CanHeaders.Count(x => x.CanId == canid && x.DLC == dlc && x.IsExtId == false) == 0)
                {
                    provider.Add(new CanHeaderModel() { CanId = canid, DLC = dlc, IsExtId = false });
                    provider.SaveChanges();
                }

                byte[] p = new byte[dlc];
                for (int j = 0; j < dlc; j++)
                {
                    p[j] = Convert.ToByte(paths[3 + j], 16);
                }

                double time = Convert.ToDouble(paths[0]);
                if(prevtime > time)
                {
                    offset += prevtime;
                }

                prevtime = time;
                provider.Add(new TraceModel() { CanHeader = provider.CanHeaders.First(x => x.CanId == canid && x.DLC == dlc && x.IsExtId == false), Payload = p, Time = time + offset });

                if (i % 100 == 0)
                    provider.SaveChanges();
            }
            provider.SaveChanges();
            provider.CloseConnection();

        }
    }
}
