using Caliburn.Micro;

using FFXIV_Data_Exporter.Library.Events;
using FFXIV_Data_Exporter.Library.Music;

using Microsoft.Win32;

using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.UI.WPF.ViewModels
{
    public class MusicViewModel : Screen
    {
        private readonly IRipMusic _ripMusic;
        private readonly IOggToScd _oggToScd;
        private readonly IOggToWav _oggToWav;
        private readonly IWavToMP3 _wavToMP3;
        private readonly ISendMessageEvent _sendMessageEvent;
        private string _status;

        public string Status { get => _status; set { _status = value; NotifyOfPropertyChange(() => Status); } }

        public MusicViewModel(ISendMessageEvent sendMessageEvent, IRipMusic ripMusic, IOggToScd oggToScd, IOggToWav oggToWav, IWavToMP3 wavToMP3)
        {
            _ripMusic = ripMusic;
            _oggToScd = oggToScd;
            _oggToWav = oggToWav;
            _wavToMP3 = wavToMP3;
            _sendMessageEvent = sendMessageEvent;

            _sendMessageEvent.SentMessageEvent += new OnSendMessageHandler(UpdateStatus);
        }

        public async Task RipMusic(CancellationToken cancellationToken) => await _ripMusic.GetFilesAsync();

        public async Task OggToSCD(CancellationToken cancellationToken)
        {
            var oFD = new OpenFileDialog() { Multiselect = true, Filter = "OGG Files | *.ogg" };

            if (oFD.ShowDialog() == true) await _oggToScd.ConvertToScdAsync(oFD.FileNames);
        }

        public async Task OggToWav(CancellationToken cancellationToken)
        {
            var oFD = new OpenFileDialog() { Multiselect = true, Filter = "OGG Files | *.ogg" };

            if (oFD.ShowDialog() == true) await _oggToWav.ConvertToWavAsync(oFD.FileNames);
        }

        public async Task WavToMP3(CancellationToken cancellationToken)
        {
            var oFD = new OpenFileDialog() { Multiselect = true, Filter = "Wav Files | *.wav" };

            if (oFD.ShowDialog() == true) await _wavToMP3.ConvertToMP3Async(oFD.FileNames);
        }

        public void UpdateStatus(object sender, SendMessageEventArgs e) => Status = $"{e.Message}\r\n{Status}";
    }
}