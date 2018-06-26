// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Coddee.Windows
{
    /// <summary>
    /// Helper class for getting information related to the Network.
    /// </summary>
    public class Network
    {
        /// <summary>
        /// Sends a wake on LAN magic packet to an machine
        /// </summary>
        /// <param name="mac">The machine mac address as a bytes array</param>
        public static void WakeOnLan(byte[] mac)
        {
            UdpClient client = new UdpClient();
            client.Client.Connect(IPAddress.Broadcast, 40000);
            byte[] packet = new byte[17 * 6];

            for (int i = 0; i < 6; i++)
                packet[i] = 0xFF;

            for (int i = 1; i <= 16; i++)
            for (int j = 0; j < 6; j++)
                packet[i * 6 + j] = mac[j];

            client.Client.Send(packet);
        }

        /// <summary>
        /// Sends a wake on LAN magic packet to an machine
        /// </summary>
        /// <param name="mac">The machine mac address as a string</param>
        public static void WakeOnLan(string mac)
        {
            byte[] macaddress = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                var temp1 = mac.Substring(0, 2);
                macaddress[i] = byte.Parse(temp1, NumberStyles.HexNumber);
                mac = mac.Remove(0, 2);
            }
            WakeOnLan(macaddress);
        }

        /// <summary>
        /// Returns the machine MAC address, only return MAC Address from first network card  
        /// </summary>
        /// <returns></returns>
        public static string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                return adapter.GetPhysicalAddress().ToString();
            }
            return null;
        }

        /// <summary>
        /// Returns a network adapter MAC address
        /// </summary>
        /// <returns></returns>
        public static string GetMACAddress(string adapterName)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.Name == adapterName)
                    return adapter.GetPhysicalAddress().ToString();
            }
            return null;
        }
    }
}