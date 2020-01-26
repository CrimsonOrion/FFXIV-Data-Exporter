using SaintCoinach;

namespace FFXIV_Data_Exporter.Library
{
    public interface IRealm
    {
        ARealmReversed RealmReversed { get; }

        string Update();
    }
}