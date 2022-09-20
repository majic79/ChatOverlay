namespace MaJiCSoft.WinApi
{
    using System;

    [Flags]
    public enum NOTIFYICONFLAGS : uint
    {
        NIF_MESSAGE = 0x01,
        NIF_ICON = 0x02,
        NIF_TIP = 0x04,
        NIF_STATE = 0x08,
        NIF_INFO = 0x10,
        NIF_GUID = 0x20,
        NIF_REALTIME = 0x40,
        NIF_SHOWTIP = 0x80
    }
}
