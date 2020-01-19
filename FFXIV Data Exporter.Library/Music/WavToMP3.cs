using NAudio.Lame;
using NAudio.Wave;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WavFile;

namespace FFXIV_Data_Exporter.Library.Music
{
    public class WavToMP3
    {
        public WavToMP3()
        {
        }

        public string WaveToMP3(string waveFileName, string mp3FileName, int bitRate = 192)
        {
            using var reader = new AudioFileReader(waveFileName);
            using var writer = new LameMP3FileWriter(mp3FileName, reader.WaveFormat, bitRate);
            reader.CopyTo(writer);
            return $"{mp3FileName} created.";
        }

        public string MP3ToWave(string mp3FileName, string waveFileName)
        {
            using var reader = new Mp3FileReader(mp3FileName);
            using var writer = new WaveFileWriter(waveFileName, reader.WaveFormat);
            reader.CopyTo(writer);
            return $"{waveFileName} created.";
        }

        public async Task<string> WaveToMP3Async(
            string waveFileName,
            string mp3FileName,
            int bitRate = 192,
            string title = "",
            string subtitle = "",
            string comment = "",
            string artist = "",
            string albumArtist = "",
            string album = "",
            string year = "",
            string track = "",
            string genre = "",
            byte[] albumArt = null)
        {
            try
            {
                var tag = new ID3TagData
                {
                    Title = title,
                    Artist = artist,
                    Album = album,
                    Year = year,
                    Comment = comment,
                    Genre = genre.Length == 0 ? LameMP3FileWriter.Genres[36] : genre, // 36 is game.  Full list @ http://ecmc.rochester.edu/ecmc/docs/lame/id3.html
                    Subtitle = subtitle,
                    AlbumArt = albumArt,
                    AlbumArtist = albumArtist,
                    Track = track
                };

                var reader = new AudioFileReader(waveFileName);
                if (reader.WaveFormat.Channels <= 2)
                {
                    using (reader)
                    using (var writer = new LameMP3FileWriter(mp3FileName, reader.WaveFormat, bitRate, tag))
                    {
                        await reader.CopyToAsync(writer);
                    }
                }
                else if (reader.WaveFormat.Channels == 4 || reader.WaveFormat.Channels == 6)
                {
                    reader.Dispose();
                    mp3FileName = string.Empty;
                    SplitWav(waveFileName);
                    var fileNames = MixSixChannel(waveFileName);
                    foreach (var fileName in fileNames)
                    {
                        using (reader = new AudioFileReader(fileName))
                        {
                            using (var writer = new LameMP3FileWriter(fileName.Replace(".wav", ".mp3"), reader.WaveFormat, bitRate: bitRate, id3: tag))
                            {
                                await reader.CopyToAsync(writer);
                            }
                            mp3FileName += fileName.Replace(".wav", ".mp3") + " ";
                        }
                    }
                }
                else
                {
                    throw new Exception($"Could not convert {mp3FileName}: It has {reader.WaveFormat.Channels} channels.");
                }

                var x = mp3FileName.LastIndexOf(@"\") + 1;

                return $"{mp3FileName.Substring(x)} created";
            }
            catch (Exception ex)
            {
                return $"{waveFileName} failed due to error: {ex.Message}";
            }
        }

        private string[] MixSixChannel(string fileName)
        {
            fileName = fileName.Replace(".wav", "");
            var fileNames = new string[2];

            using (var input1 = new WaveFileReader($"{fileName}.CH01.wav"))
            {
                using var input2 = new WaveFileReader($"{fileName}.CH02.wav");
                var waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { input1, input2 }, 2);
                waveProvider.ConnectInputToOutput(0, 0);
                waveProvider.ConnectInputToOutput(1, 1);
                WaveFileWriter.CreateWaveFile($"{fileName}.Dungeon.wav", waveProvider);
                fileNames[0] = $"{fileName}.Dungeon.wav";
            }

            using (var input1 = new WaveFileReader($"{fileName}.CH03.wav"))
            {
                using var input2 = new WaveFileReader($"{fileName}.CH04.wav");
                var waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { input1, input2 }, 2);
                waveProvider.ConnectInputToOutput(0, 0);
                waveProvider.ConnectInputToOutput(1, 1);
                WaveFileWriter.CreateWaveFile($"{fileName}.Battle.wav", waveProvider);
                fileNames[1] = $"{fileName}.Battle.wav";
            }

            return fileNames;

            /* TO PLAY IT OUTRIGHT:
            WasapiOut outDevice = new WasapiOut();
            outDevice.Init(waveProvider);
            outDevice.Play(); */
        }

        private void SplitWav(string fileName)
        {
            var outputPath = fileName.Remove(fileName.Length - Path.GetFileName(fileName).Length);

            try
            {
                long bytesTotal = 0;
                var splitter = new WavFileSplitter(value => Console.WriteLine(string.Format("\rProgress: {0:0.0}%", value)));
                var sw = Stopwatch.StartNew();
                bytesTotal = splitter.SplitWavFile(fileName, outputPath, CancellationToken.None);
                sw.Stop();
                Console.Write(Environment.NewLine);
                Console.WriteLine(
                    string.Format(
                        "Data bytes processed: {0} ({1} MB)",
                        bytesTotal, Math.Round((double)bytesTotal / (1024 * 1024), 1)));
                Console.WriteLine(string.Format("Elapsed time: {0}", sw.Elapsed));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}