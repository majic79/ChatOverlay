namespace MaJiCSoft.WinApi
{
    using MaJiCSoft.WinApi.NotifyIcon;
    using System.Runtime.InteropServices;

    public static class Shell32
    {
        private const string Shell32_Dll = "shell32.dll";

        [DllImport(Shell32_Dll, CharSet = CharSet.Unicode)]
        public static extern bool Shell_NotifyIcon(NOTIFYICONMESSAGE dwMessage, [In] ref NOTIFYICONDATA lpData);
    }
}
