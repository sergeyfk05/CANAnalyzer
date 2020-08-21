/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            if (!(File.Exists(path) && (path.Split('.').Last() == "exe")))
                return false;


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
                    };
#endif

                    
                    process.Start();


                    response = "Hello world!";
                    process.StandardInput.WriteLine(response);

                    message = process.StandardOutput.ReadLine();
                    process.Kill();
                    process.Dispose();
                    
                }


                return message.TrimEnd('\0') == response;
            }
            catch
            {
                return false;
                //throw new ArgumentException("Invalide file\r\n" + e.ToString());
            }


        }

        public override string ToString()
        {
            return "ConsoleChannelProxyCreator";
        }
    }
}
