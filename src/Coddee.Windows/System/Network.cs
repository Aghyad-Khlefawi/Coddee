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
            client.Connect(IPAddress.Broadcast, 40000);
            byte[] packet = new byte[17 * 6];

            for (int i = 0; i < 6; i++)
                packet[i] = 0xFF;

            for (int i = 1; i <= 16; i++)
            for (int j = 0; j < 6; j++)
                packet[i * 6 + j] = mac[j];

            client.Send(packet, packet.Length);
        }

        /// <summary>
        /// Sends a wake on LAN magic packet to an machine
        /// </summary>
        /// <param name="mac">The machine mac address as a string</param>
        public static void WakeOnLan(string mac)
        {
            var temp = (string) mac.Clone();
            byte[] macaddress = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                var temp1 = temp.Substring(0, 2);
                macaddress[i] = byte.Parse(temp1, NumberStyles.HexNumber);
                temp = temp.Remove(0, 2);
            }
            WakeOnLan(macaddress);
        }

        /// <summary>
        /// Returns a file size from FTP server
        /// </summary>
        /// <param name="remoteLocation">file remote URL</param>
        /// <param name="userName">authentication username</param>
        /// <param name="password">authentication password</param>
        /// <returns></returns>
        public static long GetFileSize(string remoteLocation, string userName, string password)
        {
            var request = WebRequest.Create(remoteLocation);
            request.Credentials = new NetworkCredential(userName, password);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            var response = (FtpWebResponse) request.GetResponse();
            var size = response.ContentLength;
            response.Close();
            return size;
        }

        /// <summary>
        /// Check if the current machine is connected to the Internet
        /// </summary>
        /// <param name="testSite">A website to test the connection</param>
        public static bool CheckForInternetConnection(string testSite)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead(testSite))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the current machine is connected to the Internet,
        /// The function uses www.google.com to check for Internet 
        /// </summary>
        public static bool CheckForInternetConnection()
        {
            return CheckForInternetConnection("http://www.google.com");
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