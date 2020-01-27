using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public interface IOggToWav
    {
        Task ConvertToWavAsync(IEnumerable<string> oggFiles);
    }
}