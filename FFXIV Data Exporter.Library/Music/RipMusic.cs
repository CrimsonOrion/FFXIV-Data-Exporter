using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using SaintCoinach;
using SaintCoinach.Sound;
using SaintCoinach.Xiv;

using System;
using System.IO;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public class RipMusic : IRipMusic
    {
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;
        private readonly ARealmReversed _realm;

        public RipMusic(ICustomLogger logger, ISendMessageEvent sendMessageEvent, IRealm realm)
        {
            _logger = logger;
            _sendMessageEvent = sendMessageEvent;
            _realm = realm.RealmReversed;
        }

        public async Task GetFilesAsync()
        {
            var files = _realm.GameData.GetSheet("BGM");
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
                            var successMessage = $"{path} exported.";
                            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(successMessage));
                            _logger.LogInformation(successMessage);
                            success++;
                        }
                        else
                        {
                            var notFoundMessage = $"File {path} not found.";
                            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(notFoundMessage));
                            _logger.LogInformation(notFoundMessage);
                            fail++;
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"Could not export {path}.";
                        await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(errorMessage));
                        _logger.LogError(ex, errorMessage);
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
                            var successMessage = $"{filePath}-{name} exported.";
                            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(successMessage));
                            _logger.LogInformation(successMessage);
                            success++;
                        }
                        else
                        {
                            var notFoundMessage = $"File {filePath}-{name} not found.";
                            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(notFoundMessage));
                            _logger.LogInformation(notFoundMessage);
                            fail++;
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"Could not export {filePath}-{name}.";
                        await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(errorMessage));
                        _logger.LogError(ex, errorMessage);
                        fail++;
                    }
                }
            }

            var message = $"{success} files exported. {fail} files failed.";
            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(message));
            _logger.LogInformation(message);
        }

        private bool ExportFile(string filePath, string suffix)
        {
            var result = false;
            if (_realm.Packs.TryGetFile(filePath, out var file))
            {
                var scdFile = new ScdFile(file);
                var count = 0;

                for (var i = 0; i < scdFile.ScdHeader.EntryCount; ++i)
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

                    var fileInfo = new FileInfo(Path.Combine(_realm.GameVersion, Path.GetDirectoryName(filePath), extension.ToUpper().Replace(".", ""), fileNameWithoutExt + extension));

                    if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();
                    if (fileInfo.Exists) break;

                    File.WriteAllBytes(fileInfo.FullName, entry.GetDecoded());
                }

                result = true;
            }

            return result;
        }
    }
}