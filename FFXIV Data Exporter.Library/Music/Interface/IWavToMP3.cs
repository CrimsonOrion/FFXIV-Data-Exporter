using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public interface IWavToMP3
    {
        Task ConvertToMP3Async(IEnumerable<string> wavFiles);
        Task MP3ToWaveAsync(string mp3FileName, string waveFileName);
        string WaveToMP3(string waveFileName, string mp3FileName, int bitRate = 192);
    }
}