// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Diagnostics;
using Microsoft.Win32;

namespace Coddee.Windows
{
    /// <summary>
    /// Helper class for getting information related to the operation system.
    /// </summary>
    public class OS
    {
        /// <summary>
        /// Returns the windows name and it's edition
        /// E.g. Windows 10 Enterprise
        /// The function result may change between different versions of windows
        /// </summary>
        public static string GetWindowsName()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            return (string) reg.GetValue("ProductName");
        }

        /// <summary>
        /// Returns the windows release id
        /// E.g. 1607
        /// The function result may change between different versions of windows
        /// </summary>
        public static string GetWindowsVersion()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            return (string) reg.GetValue("ReleaseId");
        }

        /// <summary>
        /// Returns the windows edition
        /// E.g. Enterprise
        /// The function result may change between different versions of windows
        /// </summary>
        public static string GetWindowsEdition()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            return (string) reg.GetValue("EditionID");
        }

        /// <summary>
        /// Returns the windows Build number
        /// E.g. 10.0.14393
        /// The function result may change between different versions of windows
        /// </summary>
        public static string GetWindowsBuildNumber()
        {
            var ps = new Process
            {
                StartInfo = new ProcessStartInfo("cmd.exe", "ver")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            ps.Start();
            var ver = ps.StandardOutput.ReadLine();
            var verIndex = ver.IndexOf("Version");
            return ver.Substring(verIndex, ver.Length - (verIndex + 1)).Replace("Version ", "");
        }
    }
}