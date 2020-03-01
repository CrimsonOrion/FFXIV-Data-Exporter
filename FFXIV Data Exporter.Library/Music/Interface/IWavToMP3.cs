using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public interface IWavToMP3
    {
        Task ConvertToMP3Async(IEnumerable<string> wavFiles, CancellationToken cancellationToken);
        void MP3ToWave(string mp3FileName, string waveFileName);
        void WaveToMP3(string waveFileName, string mp3FileName, int bitRate = 192);

        async Task ConvertToMP3Async(IEnumerable<string> wavFiles) => await ConvertToMP3Async(wavFiles, new CancellationToken());
    }
}