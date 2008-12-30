using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiCursor
{
    /// <summary>
    /// This Enum contains the various Responses that the Wii can execute when pressing a button.
    /// Apart from these the Wii can also send a Key based on an Ascii value.
    /// </summary>
    public enum ResponseActions
    {
        None,
        KeyEscape,
        KeyDelete,
        KeyF1,
        KeyF2,
        KeyF3,
        KeyF4,
        KeyF5,
        KeyF6,
        KeyF7,
        KeyF8,
        KeyF9,
        KeyF10,
        KeyF11,
        KeyF12,
        KeyArrowUp,
        KeyArrowDown,
        KeyArrowLeft,
        KeyArrowRight,
        MouseLeftButtonDown,
        MouseLeftButtonUp,
        MouseLeftClick,
        MouseLeftDoubleClick,
        MouseRightButtonDown,
        MouseRightButtonUp,
        MouseRightClick,
        Pause,
        Exit
    }
}
