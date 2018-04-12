// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using System.IO.Compression;

namespace Coddee.Loggers
{
    /// <summary>
    /// <see cref="ILogger"/> implementation that writes the log to a text file.
    /// </summary>
    public class FileLogger : LoggerBase
    {
        private FileInfo _file;

        /// <summary>
        /// Initialize the logger to use.
        /// </summary>
        public void Initialize(LogRecordTypes type, string fileName, bool useFileCompression)
        {
            base.Initialize(type);

            _file = new FileInfo(fileName);

            if (useFileCompression && _file.Exists)
            {
                CompressOldLogFile(fileName);
            }

            if (!_file.Directory.Exists)
                _file.Directory.Create();
            if (!_file.Exists)
                _file.Create().Dispose();
        }

        private void CompressOldLogFile(string fileName)
        {
            bool disposed = false;
            var stream = _file.OpenRead();
            var streamReader = new StreamReader(stream);

            try
            {
                if (stream.Length <= 0)
                    return;
                var str = streamReader.ReadLine();
                if (string.IsNullOrEmpty(str))
                    return;
                var dateStr = str.Substring(2, str.IndexOf(']', 2) - 2);

                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    if (DateTime.Today != date.Date)
                    {
                        var compressedFile = Path.Combine(_file.DirectoryName, $"{Path.GetFileNameWithoutExtension(fileName)}-{date.Date:dd-MM-yyyy}.zip");
                        using (var sw = new FileStream(compressedFile, FileMode.Create))
                        {
                            using (var gz = new GZipStream(sw, CompressionLevel.Optimal))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                var buffer = new byte[stream.Length];
                                stream.Read(buffer, 0, buffer.Length);
                                gz.Write(buffer, 0, buffer.Length);
                            }
                        }
                        stream.Dispose();
                        streamReader.Dispose();
                        disposed = true;
                        File.WriteAllText(_file.FullName, string.Empty);
                    }
                }


            }
            catch
            {
                // ignored
            }
            finally
            {
                if (!disposed)
                {
                    stream.Dispose();
                    streamReader.Dispose();
                }
            }
        }

        /// <inheritdoc />
        protected override void CommitLog(LogRecord record)
        {
            lock (_file)
                using (var sw = _file.AppendText())
                {
                    sw.WriteLine(BuildEvent(record));
                }
        }
    }
}