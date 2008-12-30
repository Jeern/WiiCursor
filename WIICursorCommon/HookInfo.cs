
namespace CursorCommon
{
    /// <summary>
    /// HookInfo is part of what a hook reports when an "event" has occurred.
    /// </summary>
    public struct HookInfo
    {
        public int vkCode;
        int scanCode;
        public int flags;
        int time;
        int dwExtraInfo;
    }
}
