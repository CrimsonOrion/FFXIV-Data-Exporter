using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using System;
using System.IO;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public class OggToScd : IOggToScd
    {
        private const int scdHeaderSize = 0x540;
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;

        public OggToScd(ICustomLogger logger, ISendMessageEvent sendMessageEvent)
        {
            _logger = logger;
            _sendMessageEvent = sendMessageEvent;
        }

        public async Task ConvertToScdAsync(string[] currentOggFiles)
        {
            var processed = 0;
            var failed = 0;

            foreach (var file in currentOggFiles)
            {
                try
                {
                    await Task.Run(() => Convert(file));
                    processed++;
                    var scdFile = file.Contains(".scd.ogg") ? file.Replace(".ogg", "") : file.Replace(".ogg", ".scd");
                    await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"{new FileInfo(scdFile).Name} created"));
                }
                catch (Exception ex)
                {
                    var message = $"There was a problem converting {file}.";
                    await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"{message}\r\n{ex.Message}."));
                    _logger.LogError(ex, message);
                    failed++;
                }
            }
            var result = $"Completed SCD Conversion. {processed} converted. {failed} failed.";
            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"{result}"));
            _logger.LogInformation(result);
        }

        private static byte[] ReadContentIntoByteArray(string file)
        {
            var bFile = new byte[new FileInfo(file).Length];

            using (var fileInputStream = new FileStream(file, FileMode.Open))
            {
                fileInputStream.Read(bFile, 0, bFile.Length);
            }
            return bFile;
        }

        private static void Convert(string oggPath)
        {
            var filePath = new FileInfo(oggPath);

            // Create Path
            var scdFolder = Path.Combine(Directory.GetParent(Directory.GetParent(filePath.FullName).FullName).FullName, "SCD");
            if (!Directory.Exists(scdFolder)) Directory.CreateDirectory(scdFolder);
            var scdFile = filePath.Name.Contains(".scd.ogg") ? filePath.Name.Replace(".ogg", "") : filePath.Name.Replace(".ogg", ".scd");
            var scdPath = Path.Combine(scdFolder, scdFile);

            // If the file already exists, return.
            if (File.Exists(scdPath)) return;

            // Write out SCD
            var ogg = ReadContentIntoByteArray(filePath.FullName);

            var volume = 1.0f;
            var numChannels = 2;
            var sampleRate = 44100;
            var loopStart = 0;
            var loopEnd = ogg.Length;

            // Create Header
            var header = CreateSCDHeader(ogg.Length, volume, numChannels, sampleRate, loopStart, loopEnd);

            using var output = new BufferedStream(new FileStream(scdPath, FileMode.OpenOrCreate));
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