namespace MaJiCSoft.WinApi.NotifyIcon
{
    using System;

    [Flags]
    public enum NOTIFYICONSTATE : uint
    {
        NIS_VISIBLE = 0x00,
        NIS_HIDDEN = 0x01,
        NIS_SHARED = 0x02
    }
}
