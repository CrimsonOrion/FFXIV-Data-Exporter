using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public interface IRipMusic
    {
        Task GetFilesAsync(CancellationToken cancellationToken);
        async Task GetFilesAsync() => await GetFilesAsync(new CancellationToken());
    }
}