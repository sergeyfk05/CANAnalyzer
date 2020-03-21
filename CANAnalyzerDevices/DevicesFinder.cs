using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Win32;
using System.Management;

namespace CANAnalyzerDevices
{
    public class DevicesFinder
    {

        public static void find()
        {

            string query = "SELECT * FROM Win32_SerialPort";

            string[] ModemObjects = new string[250];
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            byte k = 0;
            foreach (ManagementObject obj in searcher.Get())
            {


                ModemObjects[k] = obj["Name"].ToString() + "(" + obj["AttachedTo"].ToString() + ")";
                k += 1;

            }



            RegistryKey USBDevices = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\USB");


            //foreach (String s3 in rk2.GetSubKeyNames())
            //{
            //    RegistryKey rk3 = rk2.OpenSubKey(s3);
            //    var k = rk3.GetSubKeyNames();
            //    foreach (String s in rk3.GetSubKeyNames())
            //    {
            //        //if (_rx.Match(s).Success)
            //        //{
            //        //    RegistryKey rk4 = rk3.OpenSubKey(s);
            //        //    foreach (String s2 in rk4.GetSubKeyNames())
            //        //    {
            //        //        RegistryKey rk5 = rk4.OpenSubKey(s2);
            //        //        RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
            //        //        comports.Add((string)rk6.GetValue("PortName"));
            //        //    }
            //        //}
            //    }
            //}
        }
    }
}
