namespace MaJiCSoft.WinApi
{
    using System;
    using System.Runtime.InteropServices;

    public delegate IntPtr WNDPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    public static class User32
    {
        public const string User32_Dll = "user32.dll";

        public const uint HWND_BROADCAST = 0xFFFF;

        [DllImport(User32_Dll, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport(User32_Dll, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            int dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            int dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
            );


        [DllImport(User32_Dll, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern short RegisterClass(ref WNDCLASS lpWndClass);

        [DllImport(User32_Dll, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [DllImport(User32_Dll, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32_Dll, ExactSpelling = true, SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport(User32_Dll, ExactSpelling = true, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(User32_Dll, ExactSpelling = true, SetLastError = true)]
        public static extern bool GetPhysicalCursorPos(ref Point lpPoint);


        [DllImport(User32_Dll, SetLastError = true)]
        public static extern bool GetCursorPos(ref Point lpPoint);
    }
}
