using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Exd
{
    public interface IAllExd
    {
        Task RipAsync(string paramList, CancellationToken cancellationToken);
        async Task RipAsync() => await RipAsync(null, new CancellationToken());
        async Task RipAsync(string paramList) => await RipAsync(paramList, new CancellationToken());
        async Task RipAsync(CancellationToken cancellationToken) => await RipAsync(null, cancellationToken);
    }
}