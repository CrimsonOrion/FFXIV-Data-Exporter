﻿using FFXIV_Data_Exporter.Library.Configuration;
using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Logging;

using SaintCoinach;
using SaintCoinach.Ex;
using SaintCoinach.Ex.Relational.Update;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library
{
    public class Realm : IRealm
    {
        public ICustomLogger _logger;
        public AppConfiguration _config;
        private readonly ISendMessageEvent _sendMessageEvent;

        public ARealmReversed RealmReversed { get; }

        public Realm(ICustomLogger logger, AppConfiguration config, ISendMessageEvent sendMessageEvent)
        {
            _logger = logger;
            _config = config;
            _sendMessageEvent = sendMessageEvent;

            RealmReversed = new ARealmReversed(_config.FilePaths.GamePath, GetLanguage(_config.ExportSettings.Language));
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking Realm Version...");
            var gameVer = RealmReversed.GameVersion;
            var defVer = RealmReversed.DefinitionVersion;
            var updates = $"Game Version: {gameVer}\r\nDefinition Version: {defVer}.";
            _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(updates));
            _logger.LogInformation($"Game Version: {gameVer}. Definition Version: {defVer}.");
            if (!RealmReversed.IsCurrentVersion)
            {
                _logger.LogInformation("Updating Realm.");
                IProgress<UpdateProgress> value = new ProgressReporter(_logger);
                const bool IncludeDataChanges = true;
                var updateReport = await Task.Run(() => RealmReversed.Update(IncludeDataChanges, value), cancellationToken);
                foreach (var change in updateReport.Changes)
                {
                    updates = await Task.Run(() => $"{value}\r\n{change.SheetName} {change.ChangeType}");
                    _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(updates));
                    _logger.LogInformation(updates);
                }
            }
            else
            {
                updates = "Running current version.";
                _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(updates));
                _logger.LogInformation(updates);
            }
        }

        private Language GetLanguage(string language) =>
            language.ToLower().Trim() switch
            {
                "english" => Language.English,
                "japanese" => Language.Japanese,
                "french" => Language.French,
                "german" => Language.German,
                _ => Language.None
            };
    }
}