// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Management;

namespace Coddee.Windows
{
    public static class SystemInfo
    {
        /// <summary>
        /// Returns the a storage drive serial number
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        public static string GetHDDSerialNumber(string drive)
        {
            return Drive.GetDrive(drive).SerialNumber.ToString();
        }

        /// <summary>
        /// Reunts the cpu unique idenetifire
        /// Requires a reference to System.Management.dll
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()
        {
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();
            foreach (var managObj in managCollec)
            {
                if (managObj.Properties["processorID"].Value != null)
                    return managObj.Properties["processorID"].Value.ToString();
            }
            return "NotSupported";
        }
    }
}