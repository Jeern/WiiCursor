using System;

namespace CursorCommon
{
    /// <summary>
    /// The KeyBoardEventArgs extends EventArgs with KeyCode (which Key was pressed) and KeyDown
    /// and KeyUp, indicating whether the Key is down or released (up)
    /// </summary>
    public class CCKeyboardEventArgs : EventArgs 
    {
        private int m_KeyCode;
        private bool m_KeyDown;

        public CCKeyboardEventArgs(int keyCode, bool keyDown)
        {
            m_KeyCode = keyCode;
            m_KeyDown = keyDown;
        }

        public int KeyCode
        {
            get { return m_KeyCode; } 
        }

        public bool KeyDown
        {
            get { return m_KeyDown; }
        }

        public bool KeyUp
        {
            get { return !m_KeyDown; }
        }
    }
}
