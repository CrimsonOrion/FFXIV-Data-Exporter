using FFXIV_Data_Exporter.Library.Configuration;
using FFXIV_Data_Exporter.Library.Logging;

using SaintCoinach;
using SaintCoinach.Ex;
using SaintCoinach.Ex.Relational.Update;

using System;

namespace FFXIV_Data_Exporter.Library
{
    public class Realm : IRealm
    {
        public ARealmReversed RealmReversed { get; }
        public ICustomLogger _logger;
        public FilePathsModel _config;

        public Realm(ICustomLogger logger, FilePathsModel config)
        {
            _logger = logger;
            _config = config;
            RealmReversed = new ARealmReversed(_config.GamePath, GetLanguage("english"));
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

        public string Update()
        {
            _logger.LogInformation("Checking Realm Version...");
            var gameVer = RealmReversed.GameVersion;
            var defVer = RealmReversed.DefinitionVersion;
            var updates = string.Empty;
            updates += $"Game Version: {gameVer}\r\nDefinition Version: {defVer}.\r\n\r\n";
            _logger.LogInformation($"Game Version: {gameVer}. Definition Version: {defVer}.");
            if (!RealmReversed.IsCurrentVersion)
            {
                _logger.LogInformation("Updating Realm.");
                IProgress<UpdateProgress> value = new ProgressReporter(_logger);
                const bool IncludeDataChanges = true;
                var updateReport = RealmReversed.Update(IncludeDataChanges, value);
                foreach (var change in updateReport.Changes)
                {
                    updates += $"{change.SheetName} {change.ChangeType}\r\n";
                    _logger.LogInformation($"{value}\r\n{change.SheetName} {change.ChangeType}");
                }
                return updates;
            }
            else
            {
                updates += "Running current version.";
                _logger.LogInformation("Running current version.");
                return updates;
            }
        }
    }
}