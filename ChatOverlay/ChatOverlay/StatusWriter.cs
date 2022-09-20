using MaJiCSoft.ChatOverlay.ViewModels;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace MaJiCSoft.ChatOverlay
{

    internal class StatusWriter : TextWriter
    {
        private readonly StatusViewModel statusViewModel;

        public StatusWriter(StatusViewModel statusViewModel)
        {
            this.statusViewModel = statusViewModel;
        }

        public override void Write(char value)
        {
            statusViewModel.Status += value;
        }

        public override void Write(string value)
        {
            statusViewModel.Status += value;
        }

        public override Encoding Encoding {
            get {
                return Encoding.Default; // Or does this need to be UTF8?
            }
        }
    }
}
