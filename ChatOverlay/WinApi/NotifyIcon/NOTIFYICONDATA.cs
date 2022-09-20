namespace MaJiCSoft.WinApi.NotifyIcon
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NOTIFYICONDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uID;
        public NOTIFYICONFLAGS uFlags;
        public uint uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string szTip;
        public NOTIFYICONSTATE dwState;
        public NOTIFYICONSTATE dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string szInfo;
        public uint uVersion;   // Unionized with uTimeout, however deprecated in Vista and later
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string szInfoTitle;
        public NOTIFYICONINFOFLAGS dwInfoFlags;
        public Guid guidItem;
        public IntPtr CustomBalloonIconHandle;

        public static NOTIFYICONDATA DefaultNotifyIconData(IntPtr handle, uint messageId, Guid guid)
        {
            var data = new NOTIFYICONDATA();
            if (Environment.OSVersion.Version.Major < 6)
                throw new NotSupportedException("Operating System version is not supported");

            data.cbSize = (uint)Marshal.SizeOf(data);

            data.hWnd = handle;
            data.uID = 0x0;
            data.uCallbackMessage = messageId;
            data.uVersion = 0x04; // Vista or later
            data.hIcon = IntPtr.Zero;
            data.dwState = NOTIFYICONSTATE.NIS_HIDDEN;
            data.dwStateMask = NOTIFYICONSTATE.NIS_HIDDEN;
            data.uFlags = NOTIFYICONFLAGS.NIF_MESSAGE | NOTIFYICONFLAGS.NIF_GUID;
            data.dwInfoFlags = NOTIFYICONINFOFLAGS.NIIF_NONE;
            data.guidItem = guid;
            data.szTip = data.szInfo = data.szInfoTitle = string.Empty;

            return data;
        }
    }
}
