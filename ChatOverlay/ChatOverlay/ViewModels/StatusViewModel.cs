using MaJiCSoft.ChatOverlay.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MaJiCSoft.ChatOverlay.ViewModels
{
    public class StatusViewModel : ViewModelBase
    {
        private const string ISODATE = "HH:mm:ss";

        private readonly IEventAggregator ea;
        private readonly StatusWriter statusWriter;

        private string status;
        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public DelegateCommand viewLoaded;
        public ICommand ViewLoaded { get => viewLoaded; }

        public StatusViewModel(IEventAggregator ea)
        {
            this.ea = ea;

            var statusEvent = this.ea.GetEvent<StatusMessage>();
            var eventSubscriber = statusEvent.Subscribe(OnStatusMessage, ThreadOption.UIThread);

            statusWriter = new StatusWriter(this);
            viewLoaded = new DelegateCommand(OnViewLoaded, (x) => true);
        }

        public void OnViewLoaded(object parameters)
        {
            ea.GetEvent<StringMessage>().Publish(new StringMessage { Message = "StatusLoaded" });
        }

        public void OnStatusMessage(StatusMessage message)
        {
            statusWriter.WriteLine($"{message.Timestamp.ToString(ISODATE)} : {message.Status}");
        }
    }
}
