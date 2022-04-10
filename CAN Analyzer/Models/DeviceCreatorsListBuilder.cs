using CANAnalyzerDevice;
using CANAnalyzerDevices.Devices;
using CANHackerDevice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CANAnalyzer.Models
{
    internal static class DeviceCreatorsListBuilder
    {
        internal static List<IDeviceCreator> _creators = null;
        internal static List<IDeviceCreator> BuildDeviceCreatorsList()
        {
            if(_creators != null)
            {
                return _creators;
            }

            var devicesCreators = new List<IDeviceCreator>();

            if (File.Exists("Resources/Configs/Devices.ini"))
            {
                string[] ddls = File.ReadAllLines("Resources/Configs/Devices.ini");
                foreach (var ddlName in ddls)
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(ddlName);
                        foreach (Type type in asm.GetTypes())
                        {
                            var absraction = type.GetInterface("IDeviceCreator");
                            if (absraction != null)
                            {
                                var creator = System.Activator.CreateInstance(type);
                                devicesCreators.Add((IDeviceCreator)creator);
                            }

                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            _creators = devicesCreators;
            return devicesCreators;
        }

    }
}
