namespace MaJiCSoft.NotifyIcon
{
    using MaJiCSoft.WinApi;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class NotifyMessageSink : IDisposable
    {
        public const uint WM_TRAYMESSAGE = ((uint)WINDOWMESSAGE.WM_USER) + 1;

        private WNDPROC messageHandler;
        private uint taskbarCreatedMessageId;
        private bool doubleClick;
        internal Guid Guid { get; private set; }
        internal string WindowName { get; private set; }
        internal IntPtr MessageWindowHandle { get; private set; }
        
        private WNDCLASS wc;

        public event EventHandler TaskbarCreated;
        public event EventHandler<MouseEvent> MouseEventHandler;
        public event EventHandler<OpenCloseEvent> BalloonToolTipHandler;
        public event EventHandler<OpenCloseEvent> ToolTipHandler;

        internal static NotifyMessageSink DummySink()
        {
            return new NotifyMessageSink
            {
                MessageWindowHandle = IntPtr.Zero
            };
        }

        private NotifyMessageSink()
        {
            // Empty constructor
        }

        public NotifyMessageSink(Guid newGuid)
        {
            this.Guid = newGuid;
            CreateMessageWindow();
        }

        private void CreateMessageWindow()
        {
            WindowName = $"NotifyIcon_{this.Guid} ";

            messageHandler = WindowMessageHandler;

            wc.lpfnWndProc = messageHandler;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = string.Empty;
            wc.lpszClassName = WindowName;

            var atom = User32.RegisterClass(ref wc);
            if(atom == 0)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            taskbarCreatedMessageId = User32.RegisterWindowMessage("TaskbarCreated");

            MessageWindowHandle = User32.CreateWindowEx(0, WindowName, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (MessageWindowHandle == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }

        private IntPtr WindowMessageHandler(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (uMsg == taskbarCreatedMessageId)
            {
                TaskbarCreated(this, EventArgs.Empty);
            }

            ProcessWindowMessage(uMsg, wParam, lParam);

            return User32.DefWindowProc(hWnd, uMsg, wParam, lParam);
        }

        private void ProcessWindowMessage(uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            // Main Windows Message loop processor:
            if (uMsg != WM_TRAYMESSAGE) return;

            var windowMessage = (WINDOWMESSAGE)lParam.ToInt32();

            switch(windowMessage)
            {
                case WINDOWMESSAGE.WM_CONTEXTMENU:
                    // TODO: Handle WM_CONTEXTMENU, see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                    //Debug.WriteLine("WM_CONTEXTMENU");
                    break;

                case WINDOWMESSAGE.WM_MOUSEMOVE:
                    MouseEventHandler?.Invoke(this, MouseEvent.MouseMove);
                    break;

                case WINDOWMESSAGE.WM_LBUTTONDOWN:
                    MouseEventHandler?.Invoke(this, MouseEvent.LeftMouseDown);
                    break;

                case WINDOWMESSAGE.WM_LBUTTONUP:
                    if (!doubleClick)
                    {
                        MouseEventHandler?.Invoke(this, MouseEvent.LeftMouseUp);
                    }
                    doubleClick = false;
                    break;

                case WINDOWMESSAGE.WM_LBUTTONDBLCLK:
                    doubleClick = true;
                    MouseEventHandler?.Invoke(this, MouseEvent.DoubleClick);
                    break;

                case WINDOWMESSAGE.WM_RBUTTONDOWN:
                    MouseEventHandler?.Invoke(this, MouseEvent.RightMouseDown);
                    break;

                case WINDOWMESSAGE.WM_RBUTTONUP:
                    MouseEventHandler?.Invoke(this, MouseEvent.RightMouseUp);
                    break;

                case WINDOWMESSAGE.WM_RBUTTONDBLCLK:
                    //No Action
                    break;

                case WINDOWMESSAGE.WM_MBUTTONDOWN:
                    //No Action
                    break;

                case WINDOWMESSAGE.WM_MBUTTONUP:
                    //No Action
                    break;

                case WINDOWMESSAGE.WM_MBUTTONDBLCLK:
                    //No Action
                    break;

                case WINDOWMESSAGE.NIN_BALLOONSHOW:
                    BalloonToolTipHandler?.Invoke(this, OpenCloseEvent.Opened);
                    break;

                case WINDOWMESSAGE.NIN_BALLOONHIDE:
                case WINDOWMESSAGE.NIN_BALLOONTIMEOUT:
                    BalloonToolTipHandler?.Invoke(this, OpenCloseEvent.Closed);
                    break;

                case WINDOWMESSAGE.NIN_BALLOONUSERCLICK:
                    MouseEventHandler?.Invoke(this, MouseEvent.ToolTipClicked);
                    break;

                case WINDOWMESSAGE.NIN_POPUPOPEN:
                    ToolTipHandler?.Invoke(this, OpenCloseEvent.Opened);
                    break;

                case WINDOWMESSAGE.NIN_POPUPCLOSE:
                    ToolTipHandler?.Invoke(this, OpenCloseEvent.Closed);
                    break;

                case WINDOWMESSAGE.NIN_SELECT:
                    // TODO: Handle NIN_SELECT see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                    //Debug.WriteLine("NIN_SELECT");
                    break;

                case WINDOWMESSAGE.NIN_KEYSELECT:
                    // TODO: Handle NIN_KEYSELECT see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                    //Debug.WriteLine("NIN_KEYSELECT");
                    break;

                default:
                    Debug.WriteLine($"Unhandled NotifyIcon Message: {lParam}");
                    break;
            }
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
                User32.DestroyWindow(MessageWindowHandle);
                //User32.UnregisterClass();
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        //~NotifyMessageSink()
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
