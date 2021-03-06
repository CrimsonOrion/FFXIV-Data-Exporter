﻿using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Helpers;
using FFXIV_Data_Exporter.Library.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library.Music
{
    public class OggToWav : IOggToWav
    {
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;

        public OggToWav(ICustomLogger logger, ISendMessageEvent sendMessageEvent)
        {
            _logger = logger;
            _sendMessageEvent = sendMessageEvent;
        }

        public async Task ConvertToWavAsync(IEnumerable<string> oggFiles, CancellationToken cancellationToken)
        {
            int processed = 0, skipped = 0, failed = 0;
            foreach (var oggFile in oggFiles)
            {
                var folder = Path.GetDirectoryName(oggFile);
                var parentFolder = Directory.GetParent(folder);
                var wavFolder = Path.Combine(parentFolder.FullName, "WAV");

                if (!Directory.Exists(wavFolder)) Directory.CreateDirectory(wavFolder);

                var wavFile = Path.Combine(wavFolder, Path.GetFileName(oggFile).Replace(".ogg", ".wav"));

                if (File.Exists(wavFile))
                {
                    var skipMessage = await Task.Run(() => $"{wavFile} exists. Skipping.");
                    _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(skipMessage));
                    _logger.LogInformation(skipMessage);
                    skipped++;
                    continue;
                }

                try
                {
                    await ConvertAsync(oggFile, wavFile);
                    processed++;
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Unable to convert {oggFile}";
                    _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(errorMessage));
                    _logger.LogError(ex, errorMessage);
                    failed++;
                }
            }
            var message = $"Completed WAV Conversion. {processed} converted. {skipped} skipped. {failed} failed.";
            _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(message));
            _logger.LogInformation(message);
        }

        private async Task ConvertAsync(string oggFile, string wavFile)
        {
            using var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "CMD";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = $" /c .\\vgmstream\\test -o \"{wavFile}\" \"{oggFile}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            var output = await RunExternalProcess.LaunchAsync(process);
            var message = $"{Path.GetFileName(wavFile)} created.{output}";
            _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(message));
            _logger.LogInformation(message);
        }
    }
}