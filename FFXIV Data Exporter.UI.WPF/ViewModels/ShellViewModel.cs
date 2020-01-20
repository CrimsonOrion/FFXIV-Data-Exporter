using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Helpers;
using FFXIV_Data_Exporter.Library.Music;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private string _status;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public ShellViewModel()
        {
            //var w = new Weather(realm);
        }

        public async Task OggToWav()
        {
            int processed = 0, skipped = 0;
            var files = new List<string>();
            var oFD = new OpenFileDialog() { Multiselect = true, Filter = "OGG Files | *.ogg" };
            if (oFD.ShowDialog() == true)
            {
                files.AddRange(oFD.FileNames);
            }
            else
            {
                return;
            }

            foreach (var file in files)
            {
                var result = await new OggToWav().ConvertToWavAsync(file);
                if (result.Contains("Skipped"))
                    skipped++;
                else
                    processed++;
                Status = $"{result}\r\n\r\n{Status}";
            }

            Status = $"Completed WAV Conversion. {processed} converted. {skipped} skipped.\r\n\r\n{Status}";
        }

        public async Task WavToMP3()
        {
            var files = new List<string>();
            string album, year;
            var oFD = new OpenFileDialog() { Multiselect = true, Filter = "Wav Files | *.wav" };
            int processed = 0, skipped = 0;
            if (oFD.ShowDialog() == true)
            {
                files.AddRange(oFD.FileNames);
            }
            else
            {
                return;
            }

            foreach (var file in files)
            {
                var folder = Path.GetDirectoryName(file);
                var mp3Folder = Path.Combine(folder, "..", "MP3");
                if (!Directory.Exists(mp3Folder))
                {
                    Directory.CreateDirectory(mp3Folder);
                }

                var mp3File = Path.Combine(mp3Folder, Path.GetFileName(file).Replace(".wav", ".mp3"));


                if (File.Exists(mp3File))
                {
                    Status = $"{mp3File} exists. Skipping.";
                    skipped++;
                }
                else
                {
                    if (file.Contains("_EX3_")) { album = "FFXIV:ShB DAT Rip"; year = "2019"; }
                    else if (file.Contains("_EX2_")) { album = "FFXIV:SB DAT Rip"; year = "2017"; }
                    else if (file.Contains("_EX1_")) { album = "FFXIV:HW DAT Rip"; year = "2015"; }
                    else if (file.Contains("_ORCH_")) { album = "FFXIV:ORCH DAT Rip"; year = "2019"; }
                    else { album = "FFXIV:ARR DAT Rip"; year = "2013"; }

                    Status = await new WavToMP3().WaveToMP3Async(file, mp3File, albumArtist: "Square Enix", album: album, year: year) + $"\r\n\r\n{Status}";
                    processed++;
                }
            }

            Status = $"Completed MP3 Conversion. {processed} converted. {skipped} skipped.\r\n\r\n{Status}";
        }
    }
}