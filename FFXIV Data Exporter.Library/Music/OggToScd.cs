using System;
using System.IO;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public class OggToScd
    {
        private const int scdHeaderSize = 0x540;

        public OggToScd()
        {
        }

        public async Task<string> ConvertToSCDAsync(string[] currentOggFiles)
        {
            var numberOfFilesSucceeded = 0;
            var numberOfFilesFailed = 0;
            var result = "";

            foreach (var file in currentOggFiles)
            {
                try
                {
                    await Task.Run(() => Convert(file));
                    //RenameFileExtension(file, "ogg");
                    numberOfFilesSucceeded++;
                }
                catch (Exception e)
                {
                    result += $"There was a problem converting {file}.\r\n{e.Message}\r\n\r\n";
                    numberOfFilesFailed++;
                }
            }
            return result += $"Finished converting OGG files.\r\nNumber of files converted:{numberOfFilesSucceeded}\r\nNumber of files failed:{numberOfFilesFailed}\r\n\r\n";
        }

        private static byte[] ReadContentIntoByteArray(string file)
        {
            //FileStream fileInputStream = null;
            var bFile = new byte[new FileInfo(file).Length];

            using (var fileInputStream = new FileStream(file, FileMode.Open))
            {
                fileInputStream.Read(bFile, 0, bFile.Length);
            }
            return bFile;
        }

        private static void Convert(string oggPath)
        {
            var ogg = ReadContentIntoByteArray(oggPath);

            var volume = 1.0f;
            var numChannels = 2;
            var sampleRate = 44100;
            var loopStart = 0;
            var loopEnd = ogg.Length;

            // Create Header
            var header = CreateSCDHeader(ogg.Length, volume, numChannels, sampleRate, loopStart, loopEnd);

            //Write out scd
            if (oggPath.Contains(".scd.ogg"))
            {
                oggPath = oggPath.Replace(".ogg", "");
            }
            else
            {
                oggPath = oggPath.Replace(".ogg", ".scd");
            }

            using var output = new BufferedStream(new FileStream(oggPath, FileMode.OpenOrCreate));
            output.Write(header, 0, header.Length);
            output.Write(ogg, 0, ogg.Length);
        }

        private static byte[] CreateSCDHeader(int oggLength, float volume, int numChannels, int sampleRate, int loopStart, int loopEnd)
        {
            var scdHeader = new byte[scdHeaderSize];
            using (var inStream = new FileStream("scd_header.bin", FileMode.Open))
            {
                try
                {
                    inStream.Read(scdHeader, 0, scdHeader.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            using (var memory = new MemoryStream(scdHeader))
            {
                using var writer = new BinaryWriter(memory);
                memory.Position = 0x10;
                writer.Write(scdHeader.Length + oggLength);
                memory.Position = 0x1B0;
                writer.Write(oggLength - 0x10);
                memory.Position = 0xA8;
                writer.Write(volume);
                memory.Position = 0x1B4;
                writer.Write(numChannels);
                memory.Position = 0x1B8;
                writer.Write(sampleRate);
                memory.Position = 0x1C0;
                writer.Write(loopStart);
                memory.Position = 0x1C4;
                writer.Write(loopEnd);
            }
            return scdHeader;
        }
    }
}