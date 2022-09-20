namespace MaJiCSoft.WinApi.NotifyIcon
{
    using System;

    [Flags]
    public enum NOTIFYICONINFOFLAGS : uint
    {
        NIIF_NONE = 0x00,
        NIIF_INFO = 0x01,
        NIIF_WARNING = 0x02,
        NIIF_ERROR = 0x03,
        NIIF_USER = 0x04,
        NIIF_NOSOUND = 0x10,
        NIIF_LARGE_ICON = 0x20,
        NIIF_RESPECT_QUIET_TIME = 0x80,
        NIIF_ICON_MASK = 0x0F
    }
}
