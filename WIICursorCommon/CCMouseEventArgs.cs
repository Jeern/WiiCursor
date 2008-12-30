using System;

namespace CursorCommon
{
    /// <summary>
    /// The MouseEventArgs Extends EventArgs with MouseAction containing the actual action performed by the Mouse
    /// and AtPoint containing the Screen location.
    /// </summary>
    public class CCMouseEventArgs : EventArgs 
    {        
        private MouseActions m_MouseAction;
        private MousePoint m_AtPoint;

        public CCMouseEventArgs(MouseActions mouseAction, MousePoint atPoint)
        {
            m_MouseAction = mouseAction;
            m_AtPoint = atPoint;
        }

        public MouseActions MouseAction
        {
            get { return m_MouseAction; } 
        }

        public MousePoint AtPoint
        {
            get { return m_AtPoint; }
        }

    }
}
