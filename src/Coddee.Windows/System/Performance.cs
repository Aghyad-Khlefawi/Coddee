// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Coddee.Windows
{
    public class Performance
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        /// <summary>
        /// Returns the amount of free RAM on the machine in MB
        /// </summary>
        public static long GetAvailablePhysicalMemoryInMB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            return -1;
        }

        /// <summary>
        /// Returns the amount of installer RAM on the machine in MB
        /// </summary>
        public static long GetSystemPhysicalMemoryInMB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            return -1;
        }
        
        /// <summary>
        /// Returns the amount of RAM used by the current proccess in MB
        /// </summary>
        public static long GetCurrentProccessMemoryUsageInMB()
        {
            Process proc = Process.GetCurrentProcess();
            return proc.PrivateMemorySize64 / 1048576;
        }

        /// <summary>
        /// Return the Cpu usage percentage
        /// </summary>
        /// <returns></returns>
        public static string GetTotalCpuUsage()
        {
            PerformanceCounter cpuUsage =
                new PerformanceCounter("Processor", "% Processor Time", "_Total");

            var a = cpuUsage.NextValue() + " %";
            Thread.Sleep(200);
            return Math.Round(cpuUsage.NextValue(), 0) + " %";
        }

    }

}
