﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AliceCLI.Java
{
    internal partial class Java
    {
        string[] versions = { "8", "11", "16", "17" };

        bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        bool isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        bool isOSX = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        JRE jdk;

        public Java()
        {
            Exists();
        }

        private void Exists()
        {
            foreach (var item in versions)
            {
                string path = $"{Environment.CurrentDirectory}/runtime/windows/jre_{item}";

                if (!Directory.Exists(path))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Java {item} doesn't exist! Attempting to download.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Download(new JRE(item, path));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Java {item} has been found!");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

        private void Download(JRE jre)
        {
            if (isWindows)
            {
                jre.OS = "windows";
            }
            else if (isLinux)
            {
                jre.OS = "linux";
            }
            else if (isOSX)
            {
                jre.OS = "mac";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Unsupported platform.");
                Environment.Exit(1);
            }

            jre.Download();
        }
    }
}