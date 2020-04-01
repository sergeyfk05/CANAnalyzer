/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CANAnalyzerDevices.Devices.DeviceChannels;

namespace CanAnalyzerDevicesTests
{
    [TestClass]
    public class CanHackerChannelTest
    {

        [TestMethod]
        public void CanHackerRecievedDataParseMethodTest1()
        {
            ReceivedData result = CanHackerChannel.ParseData("");

            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void CanHackerRecievedDataParseMethodTest2()
        {
            var sample = new ReceivedData()
            {
                Time = 50,
                IsExtId = true,
                CanId = 0x12345678,
                DLC = 7,
                Payload = new byte[] { 0x11, 0x33, 0x55, 0x77, 0x99, 0xbb, 0xdd }
            };

            var result = CanHackerChannel.ParseData("T1234567871133557799BBDDC350\r");

            Assert.AreEqual(result, sample);
        }

        [TestMethod]
        public void CanHackerRecievedDataParseMethodTest3()
        {
            var sample = new ReceivedData()
            {
                Time = 50,
                IsExtId = false,
                CanId = 0x678,
                DLC = 8,
                Payload = new byte[] { 0xFF, 0x11, 0x33, 0x55, 0x77, 0x99, 0xbb, 0xdd }
            };

            var result = CanHackerChannel.ParseData("t6788FF1133557799BBDDC350\r");

            Assert.AreEqual(result, sample);
        }
    }
}
