using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Exd
{
    public interface IAllExd
    {
        Task RipAsync(string paramList, CancellationToken cancellationToken);
        async Task RipAsync(CancellationToken cancellationToken) => await RipAsync(null, cancellationToken);
    }
}