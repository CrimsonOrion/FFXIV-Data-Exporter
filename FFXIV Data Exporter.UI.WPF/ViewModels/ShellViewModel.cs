using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Music;

using Microsoft.Win32;

using System.IO;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private string _status;
        private readonly IWeather _weather;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public ShellViewModel(IWeather weather) => _weather = weather;

        public async Task OggToWav()
        {
            var oFD = new OpenFileDialog() { Multiselect = true, Filter = "OGG Files | *.ogg" };

            if (oFD.ShowDialog() == false) return;

            int processed = 0, skipped = 0;
            foreach (var oggFile in oFD.FileNames)
            {
                var folder = Path.GetDirectoryName(oggFile);
                var wavFolder = Path.Combine(folder, "..", "WAV");

                if (!Directory.Exists(wavFolder)) Directory.CreateDirectory(wavFolder);

                var wavFile = Path.Combine(wavFolder, Path.GetFileName(oggFile).Replace(".ogg", ".wav"));

                if (File.Exists(wavFile))
                {
                    Status = $"{wavFile} exists. Skipping.\r\n\r\n{Status}";
                    skipped++;
                    continue;
                }

                var result = await new OggToWav().ConvertToWavAsync(oggFile, wavFile);
                processed++;
                Status = $"{result}\r\n\r\n{Status}";
            }

            Status = $"Completed WAV Conversion. {processed} converted. {skipped} skipped.\r\n\r\n{Status}";
        }

        public async Task WavToMP3()
        {
            var oFD = new OpenFileDialog() { Multiselect = true, Filter = "Wav Files | *.wav" };

            if (oFD.ShowDialog() == false) return;

            int processed = 0, skipped = 0;
            foreach (var file in oFD.FileNames)
            {
                string album, year;

                var folder = Path.GetDirectoryName(file);
                var mp3Folder = Path.Combine(folder, "..", "MP3");

                if (!Directory.Exists(mp3Folder)) Directory.CreateDirectory(mp3Folder);

                var mp3File = Path.Combine(mp3Folder, Path.GetFileName(file).Replace(".wav", ".mp3"));

                if (File.Exists(mp3File))
                {
                    Status = $"{mp3File} exists. Skipping.";
                    skipped++;
                    continue;
                }

                if (file.Contains("_EX3_")) { album = "FFXIV:ShB DAT Rip"; year = "2019"; }
                else if (file.Contains("_EX2_")) { album = "FFXIV:SB DAT Rip"; year = "2017"; }
                else if (file.Contains("_EX1_")) { album = "FFXIV:HW DAT Rip"; year = "2015"; }
                else if (file.Contains("_ORCH_")) { album = "FFXIV:ORCH DAT Rip"; year = "2019"; }
                else { album = "FFXIV:ARR DAT Rip"; year = "2013"; }

                var result = await new WavToMP3().WaveToMP3Async(file, mp3File, albumArtist: "Square Enix", album: album, year: year);
                processed++;
                Status = $"{result}\r\n\r\n{Status}";
            }

            Status = $"Completed MP3 Conversion. {processed} converted. {skipped} skipped.\r\n\r\n{Status}";
        }

        public async Task GetWeather()
        {
            var weather = await _weather.GetWeatherAsync();

            weather.ForEach(forcast => Status += $"{forcast}\r\n");
        }

        public async Task GetMoonPhase() => Status = $"{await MoonPhase.CurrentMoonPhaseAsync()}\r\n\r\n{Status}";
    }
}