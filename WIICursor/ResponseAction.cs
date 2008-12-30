using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiCursor
{
    public class ResponseAction
    {
        private ResponseActions m_Action;
        private byte m_KeyAction;

        public ResponseActions Action
        {
            get { return m_Action; }
        }

        public byte KeyAction 
        {
            get { return m_KeyAction; }
        }

        public ResponseAction(ResponseActions action)
        {
            m_Action = action;
        }
        
        public ResponseAction(byte keyAction)
        {
            m_KeyAction = keyAction;
        }
    }
}
