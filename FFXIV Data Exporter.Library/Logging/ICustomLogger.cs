using System.IO;

namespace FFXIV_Data_Exporter.Library.Logging
{
    public interface ICustomLogger
    {
        FileInfo LogFileInfo { get; set; }
        bool OutputToConsole { get; set; }
    }
}