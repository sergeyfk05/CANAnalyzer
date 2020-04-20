/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.ChannelsProxy.Creators
{
    public interface IChannelProxyCreator
    {
        /// <summary>
        /// Check on file compatability.
        /// </summary>
        /// <param name="path">Information needed for check compatability.</param>
        /// <returns>Return true if device is compatability, else return false.</returns>
        bool IsCanWorkWith(string path);

        /// <summary>
        /// Create new IChannelProxy.
        /// </summary>
        /// <param name="path">Information needed for creation IChannelProxy</param>
        /// <returns>Return IChannelProxy. Before creation checks compatibility. If not compatible, then throw exception.</returns>
        IChannelProxy CreateInstance(string path);

        /// <summary>
        /// Create new IChannelProxy.
        /// </summary>
        /// <param name="path">Information needed for creation IChannelProxy</param>
        /// <returns>Return IChannelProxy. Before creation checks compatibility. If not compatible, then returne null.</returns>
        IChannelProxy CreateInstanceDefault(string path);

        string SupportedFiles { get; }
    }
}
