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
        private readonly Realm _realm;
        private string _status;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public ShellViewModel(Realm realm)
        {
            _realm = realm;
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
                if (File.Exists(file.Replace(".ogg", ".wav")))
                {
                    Status = $"{file.Replace(".ogg", ".wav")} exists. Skipping.\r\n\r\n{Status}";
                    skipped++;
                }
                else
                {
                    using var process = new Process();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "CMD";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.Arguments = $" /c .\\vgmstream\\test -o \"{file.Replace(".ogg", ".wav")}\" \"{file}\"";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    var output = await RunExternalProcess.LaunchAsync(process);
                    var x = file.LastIndexOf(@"\") + 1;
                    Status = $"{output}{file.Substring(x).Replace(".ogg", ".wav")} created.\r\n\r\n{Status}";
                    processed++;
                }
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
                if (File.Exists(file.Replace(".wav", ".mp3")))
                {
                    Status = $"{file.Replace(".wav", ".mp3")} exists. Skipping.";
                    skipped++;
                }
                else
                {
                    if (file.Contains("_EX3_")) { album = "FFXIV:ShB DAT Rip"; year = "2019"; }
                    else if (file.Contains("_EX2_")) { album = "FFXIV:SB DAT Rip"; year = "2017"; }
                    else if (file.Contains("_EX1_")) { album = "FFXIV:HW DAT Rip"; year = "2015"; }
                    else if (file.Contains("_ORCH_")) { album = "FFXIV:ORCH DAT Rip"; year = "2019"; }
                    else { album = "FFXIV:ARR DAT Rip"; year = "2013"; }

                    Status = await new WavToMP3().WaveToMP3Async(file, file.Replace(".wav", ".mp3"), albumArtist: "Square Enix", album: album, year: year) + $"\r\n\r\n{Status}";
                    processed++;
                }
            }

            Status = $"Completed MP3 Conversion. {processed} converted. {skipped} skipped.\r\n\r\n{Status}";
        }
    }
}