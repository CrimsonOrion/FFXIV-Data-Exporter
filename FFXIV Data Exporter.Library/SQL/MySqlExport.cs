using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using SaintCoinach.Ex;
using SaintCoinach.Ex.Relational;
using SaintCoinach.Xiv;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFXIV_Data_Exporter.Library.SQL
{
    public class MySqlExport : IMySqlExport
    {
        private readonly IRealm _realm;
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;

        public MySqlExport(ICustomLogger logger, IRealm realm, ISendMessageEvent sendMessageEvent)
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
                if (sheet.Header.Variant == 1)
                {
                    sb.AppendLine($"  `_Key` INT NOT NULL,");
                }
                else
                {
                    sb.AppendLine($"  `_Key` INT NOT NULL,");
                    sb.AppendLine($"  `_SubKey` INT NOT NULL,");
                }

                // add cols
                foreach (var column in sheet.Header.Columns)
                {
                    var colName = column.Name;
                    if (string.IsNullOrEmpty(colName))
                        colName = $"unk{column.Index}";

                    sb.AppendLine($"  `{colName}` {GetSqlType(column.Reader.Type)},");
                }

                // primary key
                if (sheet.Header.Variant == 1)
                {
                    sb.AppendLine($"  PRIMARY KEY (`_Key`)");
                }
                else
                {
                    sb.AppendLine($"  PRIMARY KEY (`_Key`, `_SubKey`)");
                }

                sb.AppendLine(") COLLATE='utf8mb4_unicode_ci' ENGINE=MyISAM;");
                sb.AppendLine();

                WriteRows(sheet, sb);

                imports.Add(sb.ToString());
            }

            return imports;
        }

        private string GetSqlType(Type type) => type.Name switch
        {
            "UInt32" => "INT UNSIGNED",
            "Int32" => "INT",
            "SByte" => "TINYINT",
            "Byte" => "TINYINT UNSIGNED",
            "XivString" => "TEXT",
            "Boolean" => "BIT",
            "UInt16" => "SMALLINT UNSIGNED",
            "Int16" => "SMALLINT",
            "Single" => "FLOAT",
            "Double" => "DOUBLE",
            "Quad" => "BIGINT UNSIGNED",
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

        private string GetTableName(ISheet sheet) => $"`{sheet.Name.Replace("/", "_")}`";

        private void DoRowData(ISheet sheet, XivRow row, List<string> data, StringBuilder sb)
        {
            for (var i = 0; i < sheet.Header.ColumnCount; i++)
            {
                var o = row.GetRaw(i);

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
                    d = $"'{d.Replace("'", "\\'")}'";
                }
                data.Add(d);
            }

            sb.AppendLine($"  ( {string.Join(", ", data)} ),");
        }

        private void WriteVariant1Rows(ISheet sheet, StringBuilder sb)
        {
            var rows = sheet.Cast<XivRow>();
            var cols = new List<string>
            {
                "`_Key`"
            };

            foreach (var col in sheet.Header.Columns.Cast<RelationalColumn>())
            {
                var name = string.IsNullOrEmpty(col.Name) ? $"unk{col.Index}" : col.Name;

                cols.Add($"`{name}`");
            }

            sb.AppendLine($"INSERT INTO {GetTableName(sheet)} ({string.Join(", ", cols)}) VALUES ");

            foreach (var row in rows)
            {
                var data = new List<string>
                {
                    row.Key.ToString()
                };

                DoRowData(sheet, row, data, sb);
            }

            sb.Remove(sb.Length - 3, 3);
            sb.AppendLine(";");
        }

        private void WriteVairant2Rows(ISheet sheet, StringBuilder sb)
        {
            var rows = sheet.Cast<XivSubRow>();
            var cols = new List<string>
            {
                "`_Key`",
                "`_SubKey`"
            };

            foreach (var col in sheet.Header.Columns.Cast<RelationalColumn>())
            {
                var name = string.IsNullOrEmpty(col.Name) ? $"unk{col.Index}" : col.Name;

                cols.Add($"`{name}`");
            }

            sb.AppendLine($"INSERT INTO {GetTableName(sheet)} ({string.Join(", ", cols)}) VALUES ");

            foreach (var row in rows)
            {
                var data = new List<string>
                {
                    row.ParentRow.Key.ToString(),
                    row.Key.ToString()
                };

                DoRowData(sheet, row, data, sb);
            }

            sb.Remove(sb.Length - 3, 3);
            sb.AppendLine(";");
        }

        private void WriteRows(ISheet sheet, StringBuilder sb)
        {
            if (sheet.Header.Variant == 1)
                WriteVariant1Rows(sheet, sb);
            else
                WriteVairant2Rows(sheet, sb);
        }
    }
}