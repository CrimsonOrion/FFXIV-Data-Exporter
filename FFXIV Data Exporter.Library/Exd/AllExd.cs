using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Helpers;
using FFXIV_Data_Exporter.Library.Logging;

using SaintCoinach.Ex;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Exd
{
    public class AllExd : IAllExd
    {
        private readonly IRealm _realm;
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;

        public AllExd(ICustomLogger logger, IRealm realm, ISendMessageEvent sendMessageEvent)
        {
            _logger = logger;
            _realm = realm;
            _sendMessageEvent = sendMessageEvent;
        }

        public async Task RipAsync(string paramList, CancellationToken cancellationToken)
        {
            const string csvFileFormat = "exd-all/{0}{1}.csv";
            var arr = _realm.RealmReversed;

            IEnumerable<string> filesToExport;

            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"Getting sheet list..."));
            _logger.LogInformation("Getting sheet list...");
            if (string.IsNullOrWhiteSpace(paramList))
                filesToExport = arr.GameData.AvailableSheets;
            else
                filesToExport = paramList.Split(' ').Select(_ => arr.GameData.FixName(_));

            var successCount = 0;
            var failCount = 0;
            foreach (var name in filesToExport)
            {
                var sheet = arr.GameData.GetSheet(name);
                foreach (var lang in sheet.Header.AvailableLanguages)
                {
                    var code = lang.GetCode();
                    if (code.Length > 0)
                        code = "." + code;
                    var target = new FileInfo(Path.Combine(arr.GameVersion, string.Format(csvFileFormat, name, code)));
                    try
                    {

                        if (!target.Directory.Exists)
                            target.Directory.Create();

                        ExdHelper.SaveAsCsv(sheet, lang, target.FullName, false);
                        await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"{target.Name}"));
                        _logger.LogInformation($"Exported {target.Name}.");
                        ++successCount;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Export of {target.Name} failed.");
                        try
                        {
                            if (target.Exists) target.Delete();
                        }
                        catch { }
                        ++failCount;
                    }
                }

            }
            var returnValue = $"{successCount} files exported, {failCount} failed";
            _logger.LogInformation(returnValue);
            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs(returnValue));
        }
    }
}