namespace MaJiCSoft.NotifyIcon
{
    using MaJiCSoft.WinApi;
    using MaJiCSoft.WinApi.NotifyIcon;
    using System;
    using System.Drawing;
    using System.ComponentModel;

    public interface INotifyIcon
    {
        bool Visible { get; set; }
        Icon Icon { get; set; }

        string ToolTip { get; set; }

        void Info(string title, string info);
    }

    public class NotifyIcon : INotifyIcon, IDisposable
    {

        protected NOTIFYICONDATA notifyIcon;
        protected readonly NotifyMessageSink messageSink;
        private object mutex = new object();

        private bool notifyCreated = false;


        private bool visible;
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                if (visible)
                {
                    CreateNotifyIcon(messageSink.Guid);
                }
                else
                {
                    RemoveNotifyIcon();
                }
            }
        }

        private Icon icon;
        public Icon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                notifyIcon.hIcon = icon == null ? IntPtr.Zero : icon.Handle;
                notifyIcon.uFlags = NOTIFYICONFLAGS.NIF_ICON;
                if (notifyCreated)
                {
                    var result = InvokeNotify(NOTIFYICONMESSAGE.NIM_MODIFY, NOTIFYICONFLAGS.NIF_ICON);

                    if (!result)
                    {
                        throw new Win32Exception("Failed to set NotifyIcon Icon");
                    }
                }
            }
        }

        private string toolTip;
        public string ToolTip
        {
            get
            {
                return toolTip;
            }
            set
            {
                toolTip = value;
                notifyIcon.szTip = toolTip;
                if (notifyCreated)
                {
                    var result = InvokeNotify(NOTIFYICONMESSAGE.NIM_MODIFY, NOTIFYICONFLAGS.NIF_TIP);
                    if (!result)
                    {
                        throw new Win32Exception("Failed to set NotifyIcon Tooltip");
                    }
                }
            }
        }

        public Action<MouseEvent> MouseEventHandler { get ; set; }
        public Action<OpenCloseEvent> ToolTipHandler { get; set; }
        public Action<OpenCloseEvent> BalloonTipHandler { get; set; }

        public void Info(string title, string info)
        {
            notifyIcon.szInfoTitle = title;
            notifyIcon.szInfo = info;
            var result = InvokeNotify(NOTIFYICONMESSAGE.NIM_MODIFY, NOTIFYICONFLAGS.NIF_INFO);
            if (!result)
            {
                throw new Win32Exception("Failed to set NotifyIcon Info notification");
            }
        }

        private bool InvokeNotify(NOTIFYICONMESSAGE message, NOTIFYICONFLAGS flags)
        {
            lock (mutex)
            {
                notifyIcon.uFlags = flags;
                return Shell32.Shell_NotifyIcon(message, ref notifyIcon);
            }
        }

        public NotifyIcon(EventHandler<MouseEvent> mouseEventHandler, EventHandler<OpenCloseEvent> toolTipHandler, EventHandler<OpenCloseEvent> balloonTipHandler)
        {
            messageSink = new NotifyMessageSink(Guid.NewGuid());
            notifyIcon = NOTIFYICONDATA.DefaultNotifyIconData(messageSink.MessageWindowHandle, NotifyMessageSink.WM_TRAYMESSAGE, messageSink.Guid);

            messageSink.TaskbarCreated += (object sender, EventArgs e) => {
                notifyCreated = false;
                if (visible)
                {
                    CreateNotifyIcon(messageSink.Guid);
                }
            };

            messageSink.MouseEventHandler += mouseEventHandler;
            messageSink.ToolTipHandler += toolTipHandler;
            messageSink.BalloonToolTipHandler += balloonTipHandler;

            AppDomain.CurrentDomain.ProcessExit += (sender, e) => { Dispose(); };
        }

        private void CreateNotifyIcon(Guid guid)
        {
            if (notifyCreated) return;

            const NOTIFYICONFLAGS members = NOTIFYICONFLAGS.NIF_MESSAGE | NOTIFYICONFLAGS.NIF_GUID | NOTIFYICONFLAGS.NIF_ICON | NOTIFYICONFLAGS.NIF_TIP;

            if (icon == null) throw new ArgumentNullException(nameof(icon));
            notifyIcon.hIcon = icon == null ? IntPtr.Zero : icon.Handle;
            notifyIcon.guidItem = guid;
            if (string.IsNullOrEmpty(toolTip)) throw new ArgumentNullException(nameof(toolTip));
            notifyIcon.szTip = toolTip;

            var status = InvokeNotify(NOTIFYICONMESSAGE.NIM_ADD, members);
            if (!status)
            {
                throw new Win32Exception("Failed to create NotifyIcon");
            }

            status = InvokeNotify(NOTIFYICONMESSAGE.NIM_SETVERSION, members);
            if(!status)
            {
                throw new Win32Exception("Failed to set NotifyIcon version");
            }
            notifyCreated = true;
        }

        private void RemoveNotifyIcon()
        {
            if(!notifyCreated) return;

            var status = InvokeNotify(NOTIFYICONMESSAGE.NIM_DELETE, NOTIFYICONFLAGS.NIF_MESSAGE | NOTIFYICONFLAGS.NIF_GUID);
            notifyCreated = false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                RemoveNotifyIcon();
                messageSink.Dispose();
                disposedValue = true;
            }
        }

        //~NotifyIcon()
        //{
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        //}

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
