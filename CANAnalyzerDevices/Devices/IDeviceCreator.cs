/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Finder;

namespace CANAnalyzerDevices.Devices
{
    /// <summary>
    /// Interface for creation Devices. Before creation checks compatibility.
    /// </summary>
    public interface IDeviceCreator
    {
        /// <summary>
        /// Check on compatability device.
        /// </summary>
        /// <param name="info">Information needed for check compatability.</param>
        /// <returns>Return true if device is compatability, else return false.</returns>
        bool IsCanWorkWith(HardwareDeviceInfo info);

        /// <summary>
        /// Create new IDevice.
        /// </summary>
        /// <param name="info">Information needed for creation IDevice</param>
        /// <returns>Return IDevice. Before creation checks compatibility. If not compatible, then throw exception.</returns>
        IDevice CreateInstance(HardwareDeviceInfo info);

        /// <summary>
        /// Create new IDevice.
        /// </summary>
        /// <param name="info">Information needed for creation IDevice</param>
        /// <returns>Return IDevice. Before creation checks compatibility. If not compatible, then returne null.</returns>
        IDevice CreateInstanceDefault(HardwareDeviceInfo info);
    }
}
