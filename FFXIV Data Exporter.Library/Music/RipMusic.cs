using SaintCoinach;
using SaintCoinach.Sound;
using SaintCoinach.Xiv;
using System;
using System.Collections.Generic;
using System.IO;

namespace FFXIV_Data_Exporter.Library.Music
{
    public class RipMusic
    {
        private readonly ARealmReversed _realm;

        public RipMusic(Realm realm)
        {
            _realm = realm.RealmReversed;
        }

        public List<string> GetFiles()
        {
            var files = _realm.GameData.GetSheet("BGM");
            var log = new List<string>();
            int success = 0, fail = 0;

            foreach (IXivRow file in files)
            {
                var path = file["File"].ToString();

                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }
                else
                {
                    try
                    {
                        if (ExportFile(path, null))
                        {
                            success++;
                        }
                        else
                        {
                            log.Add($"File {path} not found.");
                            fail++;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Add($"Could not export {path}. Reason: {ex.Message}");
                        fail++;
                    }
                }
            }

            var orch = _realm.GameData.GetSheet("Orchestrion");
            var orchPath = _realm.GameData.GetSheet("OrchestrionPath");

            foreach (IXivRow orchInfo in orch)
            {
                var path = orchPath[orchInfo.Key];
                var name = orchInfo["Name"].ToString();
                var filePath = path["File"].ToString();

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    continue;
                }
                else
                {
                    try
                    {
                        if (ExportFile(filePath, name))
                        {
                            success++;
                        }
                        else
                        {
                            log.Add($"File {filePath} not found.");
                            fail++;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Add($"Could not export {filePath}. Reason: {ex.Message}");
                        fail++;
                    }
                }
            }

            log.Add($"{success} files exported. {fail} files failed.");

            return log;
        }

        bool ExportFile(string filePath, string suffix)
        {
            var result = false;
            if (_realm.Packs.TryGetFile(filePath, out var file))
            {
                var scdFile = new ScdFile(file);
                var count = 0;

                for (int i = 0; i < scdFile.ScdHeader.EntryCount; ++i)
                {
                    var entry = scdFile.Entries[i];

                    if (entry == null) continue;

                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);

                    if (suffix != null) fileNameWithoutExt += $"-{suffix}";

                    if (++count > 1) fileNameWithoutExt += $"-{count}";

                    foreach (var invalidChar in Path.GetInvalidFileNameChars())
                    {
                        fileNameWithoutExt = fileNameWithoutExt.Replace(invalidChar.ToString(), "");
                    }

                    var extension = entry.Header.Codec switch
                    {
                        ScdCodec.MSADPCM => ".wav",
                        ScdCodec.OGG => ".ogg",
                        _ => throw new NotSupportedException()
                    };

                    var targetPath = Path.Combine(_realm.GameVersion, Path.GetDirectoryName(filePath), extension.ToUpper(), fileNameWithoutExt + extension);

                    var fileInfo = new FileInfo(targetPath);

                    if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();

                    File.WriteAllBytes(fileInfo.FullName, entry.GetDecoded());
                }

                result = true;
            }

            return result;
        }
    }
}