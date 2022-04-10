/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CANAnalyzerDevices
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="timeout">timeout in ms</param>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static int SafeRead(this SerialPort port, uint timeout, ref byte[] buf)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (port.BytesToRead == 0)
            {
                if(watch.ElapsedMilliseconds > timeout)
                {
                    watch.Stop();
                    return 0;
                }
            }
            watch.Stop();

            byte[] readbuf = new byte[buf.Length];
            int size = port.Read(readbuf, 0, readbuf.Length);
            readbuf.CopyTo(buf, 0);
            return size;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="timeout">timeout in ms</param>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static int SafeRead(this SerialPort port, uint timeout, ref char[] buf)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (port.BytesToRead == 0)
            {
                if (watch.ElapsedMilliseconds > timeout)
                {
                    watch.Stop();
                    return 0;
                }
            }
            watch.Stop();

            char[] readbuf = new char[buf.Length];
            int size = port.Read(readbuf, 0, readbuf.Length);
            readbuf.CopyTo(buf, 0);
            return size;
        }
    }
}
