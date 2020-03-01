﻿using SaintCoinach.Ex;
using SaintCoinach.Xiv;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Ex = SaintCoinach.Ex;

namespace FFXIV_Data_Exporter.Library.Helpers
{

    /// <summary>
    /// Pretty much Ctrl+C, Ctrl+V from SaintCoinach.Cmd
    /// </summary>
    internal static class ExdHelper
    {
        public static CultureInfo _culture = new CultureInfo("en-US", false);

        public static void SaveAsCsv(Ex.Relational.IRelationalSheet sheet, Language language, string path, bool writeRaw)
        {
            using var s = new StreamWriter(path, false, Encoding.UTF8);
            var indexLine = new StringBuilder("key");
            var nameLine = new StringBuilder("#");
            var typeLine = new StringBuilder("int32");

            var colIndices = new List<int>();
            foreach (var col in sheet.Header.Columns)
            {
                indexLine.AppendFormat(",{0}", col.Index);
                nameLine.AppendFormat(",{0}", col.Name);
                typeLine.AppendFormat(",{0}", col.ValueType);

                colIndices.Add(col.Index);
            }

            s.WriteLine(indexLine);
            s.WriteLine(nameLine);
            s.WriteLine(typeLine);

            WriteRows(s, sheet, language, colIndices, writeRaw);
        }

        public static void WriteRows(StreamWriter s, ISheet sheet, Language language, IEnumerable<int> colIndices, bool writeRaw)
        {
            if (sheet.Header.Variant == 1)
            {
                WriteRowsCore(s, sheet.Cast<Ex.IRow>(), language, colIndices, writeRaw, WriteRowKey);
            }
            else
            {
                var rows = sheet.Cast<XivRow>().Select(_ => (Ex.Variant2.DataRow)_.SourceRow);
                foreach (var parentRow in rows.OrderBy(_ => _.Key))
                    WriteRowsCore(s, parentRow.SubRows, language, colIndices, writeRaw, WriteSubRowKey);
            }
        }

        private static void WriteRowsCore(StreamWriter s, IEnumerable<Ex.IRow> rows, Language language, IEnumerable<int> colIndices, bool writeRaw, Action<StreamWriter, Ex.IRow> writeKey)
        {
            foreach (var row in rows.OrderBy(_ => _.Key))
            {
                var useRow = row;

                if (useRow is IXivRow)
                    useRow = ((IXivRow)row).SourceRow;
                var multiRow = useRow as IMultiRow;

                writeKey(s, useRow);
                foreach (var col in colIndices)
                {
                    object v;

                    if (language == Language.None || multiRow == null)
                        v = writeRaw ? useRow.GetRaw(col) : useRow[col];
                    else
                        v = writeRaw ? multiRow.GetRaw(col, language) : multiRow[col, language];

                    s.Write(",");
                    if (v == null)
                        continue;
                    else if (IsUnescaped(v))
                        s.Write(string.Format(_culture, "{0}", v));
                    else
                        s.Write("\"{0}\"", v.ToString().Replace("\"", "\"\""));
                }
                s.WriteLine();

                s.Flush();
            }
        }

        private static void WriteRowKey(StreamWriter s, Ex.IRow row) => s.Write(row.Key);

        private static void WriteSubRowKey(StreamWriter s, Ex.IRow row)
        {
            var subRow = (Ex.Variant2.SubRow)row;
            s.Write(subRow.FullKey);
        }

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
    }
}