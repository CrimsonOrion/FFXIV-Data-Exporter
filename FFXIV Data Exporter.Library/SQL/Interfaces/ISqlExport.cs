using System.Collections.Generic;

namespace FFXIV_Data_Exporter.Library.SQL
{
    public interface ISqlExport
    {
        List<string> ExportSchema();
    }
}