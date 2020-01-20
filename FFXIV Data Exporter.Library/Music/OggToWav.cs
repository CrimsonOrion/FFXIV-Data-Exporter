using FFXIV_Data_Exporter.Library.Helpers;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public class OggToWav
    {
        public OggToWav()
        {
            
        }

        public async Task<string> ConvertToWavAsync(string file)
        {
            var status = string.Empty;
            var folder = Path.GetDirectoryName(file);
            var wavFolder = Path.Combine(folder, "..", "WAV");
            if (!Directory.Exists(wavFolder))
            {
                Directory.CreateDirectory(wavFolder);
            }

            var wavFile = Path.Combine(wavFolder, Path.GetFileName(file).Replace(".ogg", ".wav"));

            if (File.Exists(wavFile))
            {
                status = $"{wavFile} exists. Skipping.";
            }
            else
            {
                using var process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "CMD";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.Arguments = $" /c .\\vgmstream\\test -o \"{wavFile}\" \"{file}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                var output = await RunExternalProcess.LaunchAsync(process);
                status = $"{output}{Path.GetFileName(wavFile)} created.";
            }

            return status;
        }
    }
}