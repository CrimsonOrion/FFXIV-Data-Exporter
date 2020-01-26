using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public interface IRipMusic
    {
        List<string> GetFiles();
        Task<List<string>> GetFilesAsync();
    }
}