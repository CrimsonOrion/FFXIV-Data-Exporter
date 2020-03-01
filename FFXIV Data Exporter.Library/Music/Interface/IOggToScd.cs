using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public interface IOggToScd
    {
        Task ConvertToScdAsync(string[] currentOggFiles, CancellationToken cancellationToken);
        async Task ConvertToScdAsync(string[] currentOggFiles) => await ConvertToScdAsync(currentOggFiles, new CancellationToken());
    }
}