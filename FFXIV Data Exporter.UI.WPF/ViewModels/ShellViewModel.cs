using Caliburn.Micro;

using FFXIV_Data_Exporter.Library;
using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Exd;
using FFXIV_Data_Exporter.Library.Logging;
using FFXIV_Data_Exporter.Library.Music;

using Microsoft.Win32;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private readonly ICustomLogger _logger;
        private readonly ISendMessageEvent _sendMessageEvent;
        private readonly IRealm _realm;
        private readonly IAllExd _allExd;
        private readonly IWeather _weather;
        private readonly IRipMusic _ripMusic;
        private readonly IOggToScd _oggToScd;
        private readonly IOggToWav _oggToWav;
        private readonly IWavToMP3 _wavToMP3;
        private string _status;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public ShellViewModel(ICustomLogger logger, ISendMessageEvent sendMessageEvent, IRealm realm, IAllExd allExd, IWeather weather, IRipMusic ripMusic, IOggToScd oggToScd, IOggToWav oggToWav, IWavToMP3 wavToMP3)
        {
            _logger = logger;
            _realm = realm;
            _allExd = allExd;
            _weather = weather;
            _ripMusic = ripMusic;
            _oggToScd = oggToScd;
            _oggToWav = oggToWav;
            _wavToMP3 = wavToMP3;
            _sendMessageEvent = sendMessageEvent;

            _sendMessageEvent.SentMessageEvent += new OnSendMessageHandler(UpdateStatus);
        }

        public async Task UpdateRealm(CancellationToken cancellationToken)
        {
            try
            {
                await _realm.UpdateAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "Error updating");
            }
        }

        public async Task RipExd(CancellationToken cancellationToken)
        {
            try
            {
                await _allExd.RipAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "Error getting data files");
            }
        }

        public async Task GetForcast(CancellationToken cancellationToken)
        {
            try
            {
                await _weather.GetWeatherAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "Error getting forcast");
            }
        }

        public async Task GetMoonPhase(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Run(() => _weather.GetMoonPhase(), cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "Error getting moon phase");
            }
        }

        public async Task RipMusic(CancellationToken cancellationToken)
        {
            try
            {
                await _ripMusic.GetFilesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "Error getting music files");
            }
        }

        public async Task OggToSCD(CancellationToken cancellationToken)
        {
            try
            {
                var oFD = new OpenFileDialog() { Multiselect = true, Filter = "OGG Files | *.ogg" };

                if (oFD.ShowDialog() == true) await _oggToScd.ConvertToScdAsync(oFD.FileNames, cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "Error converting music files from OGG to SCD");
            }
        }

        public async Task OggToWav(CancellationToken cancellationToken)
        {
            try
            {
                var oFD = new OpenFileDialog() { Multiselect = true, Filter = "OGG Files | *.ogg" };

                if (oFD.ShowDialog() == true) await _oggToWav.ConvertToWavAsync(oFD.FileNames, cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "Error converting music files from OGG to WAV");
            }
        }

        public async Task WavToMP3(CancellationToken cancellationToken)
        {
            try
            {
                var oFD = new OpenFileDialog() { Multiselect = true, Filter = "Wav Files | *.wav" };

                if (oFD.ShowDialog() == true) await _wavToMP3.ConvertToMP3Async(oFD.FileNames, cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "Error converting music files from WAV to MP3");
            }
        }

        public void SettingsScreen()
        {
            //ActivateItem(IoC.Get<SettingsViewModel>());
        }

        public void UpdateStatus(object sender, SendMessageEventArgs e) => Status = $"{e.Message}\r\n{Status}";

        private void ErrorMessage(Exception ex, string message)
        {
            _sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs($"{message}:\r\n{ex.Message}"));
            _logger.LogError(ex, $"{message}.");
        }
    }
}