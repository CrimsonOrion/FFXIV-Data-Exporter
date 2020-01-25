using FFXIV_Data_Exporter.Library.Logging;

using SaintCoinach.Ex.Relational.Update;

using System;

namespace FFXIV_Data_Exporter.Library
{
    public class ProgressReporter : IProgress<UpdateProgress>
    {
        private readonly ICustomLogger _logger;

        public ProgressReporter(ICustomLogger logger) => _logger = logger;

        public void Report(UpdateProgress value) => _logger.LogInformation(value.ToString());
    }
}