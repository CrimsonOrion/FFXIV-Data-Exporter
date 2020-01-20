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

        public async Task<string> ConvertToWavAsync(string oggFile, string wavFile)
        {
            using var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "CMD";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = $" /c .\\vgmstream\\test -o \"{wavFile}\" \"{oggFile}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            var output = await RunExternalProcess.LaunchAsync(process);
            return $"{output}{Path.GetFileName(wavFile)} created.";
        }
    }
}