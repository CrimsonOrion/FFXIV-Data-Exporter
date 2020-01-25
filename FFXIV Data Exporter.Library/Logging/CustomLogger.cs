using System.IO;

namespace FFXIV_Data_Exporter.Library.Logging
{
    public class CustomLogger : ICustomLogger
    {
        public FileInfo LogFileInfo { get; set; }
        public bool OutputToConsole { get; set; }

        public CustomLogger(FileInfo logFileInfo, bool outputToConsole)
        {
            LogFileInfo = logFileInfo;
            OutputToConsole = outputToConsole;
        }
    }
}