﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.HelperClasses
{
    /// <summary>
    /// Logger used for result logging.
    /// </summary>
    public static class FileLogger
    {
        private static FileStream _file;
        private static bool _isDisposed = true;
        
        public static void Initialize(string filePath, FileMode mode)
        {
            _file = File.Open(filePath, mode);
            _isDisposed = false;
        }

        public static void Close()
        {
            if (_isDisposed)
            {
                return;
            }
            _file.Dispose();
            _isDisposed = true;
        }

        public static void WriteLine(string text)
        {
            _file.Write(Encoding.ASCII.GetBytes(text + '\n'), 0, text.Length + 1);
        }

        public static void Write(string text)
        {
            _file.Write(Encoding.ASCII.GetBytes(text), 0, text.Length);
        }
    }
}
