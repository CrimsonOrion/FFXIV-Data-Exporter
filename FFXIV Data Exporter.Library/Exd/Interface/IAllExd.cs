using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Exd
{
    public interface IAllExd
    {
        Task RipAsync(string paramList);
        async Task RipAsync() => await RipAsync(null);
    }
}