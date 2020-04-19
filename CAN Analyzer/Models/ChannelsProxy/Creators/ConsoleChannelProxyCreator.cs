using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.ChannelsProxy.Creators
{
    public class ConsoleChannelProxyCreator : IChannelProxyCreator
    {
        public string SupportedFiles => "*.exe";

        public IChannelProxy CreateInstance(string path)
        { 
            return IsCanWorkWith(path) ? new ConsoleChannelProxy(path) : throw new ArgumentException("this device cannot work with this hardware device.");
        }

        public IChannelProxy CreateInstanceDefault(string path)
        {
            return IsCanWorkWith(path) ? new ConsoleChannelProxy(path) : null;
        }

        public bool IsCanWorkWith(string path)
        {
            try
            {
                string message, response;
                using (Process process = new Process())
                {
#if DEBUG
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = false
                    };
#else
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true
                    }
#endif

                    
                    process.Start();


                    response = "Hello world!";
                    process.StandardInput.WriteLine(response);

                    message = process.StandardOutput.ReadLine();
                    process.Kill();
                    
                }


                return message.TrimEnd('\0') == response;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalide file\r\n" + e.ToString());
            }


        }

        public override string ToString()
        {
            return "ConsoleChannelProxyCreator";
        }
    }
}
