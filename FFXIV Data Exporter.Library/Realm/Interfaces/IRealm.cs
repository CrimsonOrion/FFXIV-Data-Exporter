using SaintCoinach;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library
{
    public interface IRealm
    {
        ARealmReversed RealmReversed { get; }

        Task UpdateAsync(CancellationToken cancellationToken);
    }
}