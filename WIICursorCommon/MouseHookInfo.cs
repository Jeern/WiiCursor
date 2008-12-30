using System;
using System.Runtime.InteropServices;

namespace CursorCommon
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseHookInfo
    {
        public MousePoint pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
