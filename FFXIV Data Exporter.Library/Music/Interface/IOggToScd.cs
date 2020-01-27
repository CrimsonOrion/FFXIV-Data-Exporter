using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public interface IOggToScd
    {
        Task ConvertToScdAsync(string[] currentOggFiles);
    }
}