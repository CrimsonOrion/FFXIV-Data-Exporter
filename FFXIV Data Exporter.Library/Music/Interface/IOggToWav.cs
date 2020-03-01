using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public interface IOggToWav
    {
        Task ConvertToWavAsync(IEnumerable<string> oggFiles, CancellationToken cancellationToken);
        async Task ConvertToWavAsync(IEnumerable<string> oggFiles) => await ConvertToWavAsync(oggFiles, new CancellationToken());
    }
}