// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Runtime.InteropServices;
using System.Text;

namespace Coddee.Windows
{
    /// <summary>
    /// A windows storage drive information
    /// </summary>
    public class Drive
    {
        /// <summary>
        /// The title of the volume.
        /// </summary>
        public string VolumeName { get; set; }
        
        /// <summary>
        /// The type of the file system used in the drive.
        /// </summary>
        public string FileSystemName { get; set; }

        /// <summary>
        /// The drive serial number.
        /// </summary>
        public uint SerialNumber { get; set; }

        /// <summary>
        /// The assigned drive letter.
        /// </summary>
        public string DriveLetter { get; set; }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool GetVolumeInformation(
            string rootPathName,
            StringBuilder volumeNameBuffer,
            int volumeNameSize,
            ref uint volumeSerialNumber,
            ref uint maximumComponentLength,
            ref uint fileSystemFlags,
            StringBuilder fileSystemNameBuffer,
            int nFileSystemNameSize);

        /// <summary>
        /// Returns a storage drive information by it's letter
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns></returns>
        public static Drive GetDrive(string driveLetter)
        {
            const int VolumeNameSize = 255;
            const int FileSystemNameBufferSize = 255;
            StringBuilder volumeNameBuffer = new StringBuilder(VolumeNameSize);
            uint volumeSerialNumber = 0;
            uint maximumComponentLength = 0;
            uint fileSystemFeatures = 0;
            StringBuilder fileSystemNameBuffer = new StringBuilder(FileSystemNameBufferSize);

            if (GetVolumeInformation(
                                     string.Format("{0}:\\", driveLetter),
                                     volumeNameBuffer,
                                     VolumeNameSize,
                                     ref volumeSerialNumber,
                                     ref maximumComponentLength,
                                     ref fileSystemFeatures,
                                     fileSystemNameBuffer,
                                     FileSystemNameBufferSize))
            {
                return new Drive
                {
                    DriveLetter = driveLetter,
                    FileSystemName = fileSystemNameBuffer.ToString(),
                    VolumeName = volumeNameBuffer.ToString(),
                    SerialNumber = volumeSerialNumber
                };
            }

            return null;
        }
    }
}