﻿using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliceCLI
{
    internal class Helper
    {
        /// <summary>
        /// https://stackoverflow.com/questions/8863875/decompress-tar-files-using-c-sharp
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="outputDir"></param>
        public static void ExtractTarGz(string filename, string outputDir)
        {
            using (var stream = File.OpenRead(filename))
                ExtractTarGz(stream, outputDir);
        }

        public static void ExtractTarGz(Stream stream, string outputDir)
        {
            // A GZipStream is not seekable, so copy it first to a MemoryStream
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            {
                const int chunk = 4096;
                using (var memStr = new MemoryStream())
                {
                    int read;
                    var buffer = new byte[chunk];
                    do
                    {
                        read = gzip.Read(buffer, 0, chunk);
                        memStr.Write(buffer, 0, read);
                    } while (read == chunk);

                    memStr.Seek(0, SeekOrigin.Begin);
                    ExtractTar(memStr, outputDir);
                }
            }
        }

        public static void ExtractTar(string filename, string outputDir)
        {
            using (var stream = File.OpenRead(filename))
                ExtractTar(stream, outputDir);
        }
        public static void ExtractTar(Stream stream, string outputDir)
        {
            var buffer = new byte[100];
            while (true)
            {
                stream.Read(buffer, 0, 100);
                var name = Encoding.ASCII.GetString(buffer).Trim('\0');
                if (String.IsNullOrWhiteSpace(name))
                    break;
                stream.Seek(24, SeekOrigin.Current);
                stream.Read(buffer, 0, 12);
                var size = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 0, 12).Trim(), 8);

                stream.Seek(376L, SeekOrigin.Current);

                var output = Path.Combine(outputDir, name);
                if (!Directory.Exists(Path.GetDirectoryName(output)))
                    Directory.CreateDirectory(Path.GetDirectoryName(output));
                using (var str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    var buf = new byte[size];
                    stream.Read(buf, 0, buf.Length);
                    str.Write(buf, 0, buf.Length);
                }

                var pos = stream.Position;

                var offset = 512 - (pos % 512);
                if (offset == 512)
                    offset = 0;

                stream.Seek(offset, SeekOrigin.Current);
            }
        }
        public static void ExtractZip(string filename, string outputDir)
        {
            ZipFile.ExtractToDirectory(filename, outputDir);
        }
    }
}
