using System.Runtime.InteropServices;

namespace CursorCommon
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int X;
        public int Y;
    }
}
