using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using SaintCoinach.Ex;
using SaintCoinach.Ex.Relational;
using SaintCoinach.Xiv;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFXIV_Data_Exporter.Library.Exporting.SQL
{
    public class MSSqlExport : IMSSqlExport
    {
        private readonly IRealm _realm;
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;

        public MSSqlExport(ICustomLogger logger, IRealm realm, ISendMessageEvent sendMessageEvent)
        {
            _logger = logger;
            _realm = realm;
            _sendMessageEvent = sendMessageEvent;
        }

        public List<string> ExportSchema()
        {
            var imports = new List<string>();

            // .Where(n => !n.Contains("quest/") && !n.Contains("custom/"))
            foreach (var name in _realm.RealmReversed.GameData.AvailableSheets)
            {
                var sheet = _realm.RealmReversed.GameData.GetSheet(name);
                var variant = sheet.Header.Variant;
                var sheet2 = sheet as XivSheet2<XivSubRow>;

                _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs($"Sheet: {name}, variant: {variant}"));
                _logger.LogInformation($"Sheet: {name}, variant: {variant}");

                if (sheet.Count == 0)
                    continue;

                var sb = new StringBuilder();

                sb.AppendLine($"CREATE TABLE {GetTableName(sheet)} (");

                // key meme
                sb.AppendLine($"\t[_Key] INT NOT NULL,");
                if (sheet.Header.Variant != 1)
                {
                    sb.AppendLine($"\t[_SubKey] INT NOT NULL,");
                }

                // add cols
                foreach (var column in sheet.Header.Columns)
                {
                    var colName = column.Name;
                    if (string.IsNullOrEmpty(colName))
                        colName = $"unk{column.Index}";

                    sb.AppendLine($"\t[{colName.Replace("[", "{").Replace("]", "}")}] {GetSqlType(column.Reader.Type)} NULL,");
                }

                // primary key
                sb.AppendLine($"PRIMARY KEY CLUSTERED (");
                sb.AppendLine(sheet.Header.Variant == 1 ? $"\t[_Key] ASC" : $"\t[_Key] ASC, [_SubKey] ASC");
                sb.AppendLine("))");

                sb.AppendLine("GO");
                sb.AppendLine();

                // Write data
                WriteRows(sheet, sb);

                sb.AppendLine("GO");
                sb.AppendLine();

                imports.Add(sb.ToString());
            }

            return imports;
        }

        private string GetSqlType(Type type) => type.Name switch
        {
            "UInt32" => "BIGINT",
            "Int32" => "INT",
            "SByte" => "INT",
            "Byte" => "TINYINT",
            "XivString" => "NVARCHAR(MAX)",
            "Boolean" => "BIT",
            "UInt16" => "INT",
            "Int16" => "SMALLINT",
            "Single" => "FLOAT",
            "Double" => "FLOAT",
            "Quad" => "NVARCHAR(MAX)", // BIGINT UNSIGNED doesn't have a direct corrilation in MSSQL
            _ => throw new NotImplementedException($"The type {type.Name} doesn't have an SQL type mapping")
        };

        private static bool IsUnescaped(object self) => (self is bool
                    || self is byte
                    || self is sbyte
                    || self is short
                    || self is int
                    || self is long
                    || self is ushort
                    || self is uint
                    || self is ulong
                    || self is float
                    || self is double);

        private string GetTableName(ISheet sheet) => $"[{sheet.Name.Replace("/", "_")}]";

        private void DoRowData(ISheet sheet, XivRow row, List<string> data, StringBuilder sb)
        {
            for (var i = 0; i < sheet.Header.ColumnCount; i++)
            {
                var o = row.GetRaw(i);

                // Change bool to 1 and 0
                if (o is bool b)
                {
                    data.Add(b == true ? "1" : "0");
                    continue;
                }

                if (o is Quad q)
                {
                    data.Add(q.ToInt64().ToString());
                    continue;
                }

                if (IsUnescaped(o))
                {
                    data.Add(o.ToString());
                    continue;
                }

                var d = o.ToString();
                if (string.IsNullOrEmpty(d))
                {
                    d = "NULL";
                }
                else
                {
                    d = $"'{d.Replace("'", "''")}'";
                }
                data.Add(d);
            }

            sb.AppendLine($"\t( {string.Join(", ", data)} ),");
        }

        private void WriteVariantRows(ISheet sheet, StringBuilder sb, bool isVariant)
        {
            if (isVariant)
            {
                var rows = sheet.Cast<XivRow>();
                var cols = new List<string> { "[_Key]" };
                WriteData(rows, cols, sheet, sb, isVariant);
            }
            else
            {
                var rows = sheet.Cast<XivSubRow>();
                var cols = new List<string> { "[_Key], [_SubKey]" };
                WriteData(rows, cols, sheet, sb, isVariant);
            }


        }

        private List<string> InitiateData(XivRow row) => new List<string> { row.Key.ToString() };
        private List<string> InitiateData(XivSubRow row) => new List<string> { row.ParentRow.Key.ToString(), row.Key.ToString() };

        private void WriteData(IEnumerable<XivRow> rows, List<string> cols, ISheet sheet, StringBuilder sb, bool isVariant)
        {
            foreach (var col in sheet.Header.Columns.Cast<RelationalColumn>())
            {
                var name = string.IsNullOrEmpty(col.Name) ? $"unk{col.Index}" : col.Name.Replace("[", "{").Replace("]", "}");

                cols.Add($"[{name}]");
            }

            var insertIndex = 0;
            var currentRow = 0;
            foreach (var row in rows)
            {
                currentRow++;
                if (insertIndex == 0)
                {
                    sb.AppendLine($"INSERT INTO {GetTableName(sheet)} ({string.Join(", ", cols)}) VALUES ");
                    insertIndex++;
                }

                var data = isVariant ? InitiateData(row) : InitiateData((XivSubRow)row);

                DoRowData(sheet, row, data, sb);

                if (insertIndex == 1000 && currentRow != rows.Count())
                {
                    sb.Remove(sb.Length - 3, 3);
                    sb.AppendLine(";");
                    sb.AppendLine("GO");
                    sb.AppendLine();
                    insertIndex = 0;
                }
                else
                {
                    insertIndex++;
                }
            }

            sb.Remove(sb.Length - 3, 3);
            sb.AppendLine(";");
        }

        private void WriteRows(ISheet sheet, StringBuilder sb)
        {
            if (sheet.Header.Variant == 1)
                WriteVariantRows(sheet, sb, true);
            else
                WriteVariantRows(sheet, sb, false);
        }
    }
}