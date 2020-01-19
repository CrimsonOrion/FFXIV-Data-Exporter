using FFXIV_Data_Exporter.Library.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public class OggToWav
    {
        public OggToWav()
        {
        }

        public async Task<string> ConvertToWavAsync(string vgmStreamPath, string oggPath)
        {
            var output = string.Empty;
            using (var process = new Process())
            {
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = vgmStreamPath;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.Arguments = $" /c .\\vgmstream\\test -o \"{oggPath.Replace(".ogg", ".wav")}\" \"{oggPath}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                output = await RunExternalProcess.LaunchAsync(process);
            }
            
            var x = oggPath.LastIndexOf(@"\") + 1;
            return $"{output}{oggPath.Substring(x).Replace(".ogg", ".wav")} created.\r\n\r\n";
        }
    }
}