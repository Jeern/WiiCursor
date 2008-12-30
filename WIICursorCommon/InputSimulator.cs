using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows;
using System.Threading;
using System.Windows.Forms;

namespace CursorCommon
{
    /// <summary>
    /// Input simulator can simulate Mouse moves & clicks + keypresses
    /// </summary>
    public class InputSimulator
    {
        private static bool m_LeftWasDown = false;
        private static bool m_RightWasDown = false;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint flags, int x, int y, int buttons, int extraInfo);

        [DllImport("user32.dll")]
        private static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern int GetCursorPos(ref MousePoint point);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, [In] INPUT pInput, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal ushort wVk;
            internal ushort wScan;
            internal uint dwFlags;
            internal uint time;
            internal IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct INPUT
        {
            [FieldOffset(0)]
            internal int type;
            [FieldOffset(4)] //*
            internal KEYBDINPUT ki;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        const uint XBUTTON1 = 0x0001;
        const uint XBUTTON2 = 0x0002;
        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_XDOWN = 0x0080;
        const uint MOUSEEVENTF_XUP = 0x0100;
        const uint MOUSEEVENTF_WHEEL = 0x0800;
        const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        public static MousePoint GetCurrentPosition() 
        { 
            MousePoint point = new MousePoint();
            GetCursorPos(ref point);
            return point;
        }

        public void DoDoubleClick()
        {
            DoLeftClick();        
            DoLeftClick();        
        }
        
        public void DoLeftClick()
        {
            DoLeftDown();        
            DoLeftUp();        
        }

        public void DoRightClick()
        {
            DoRightDown();        
            DoRightUp();        
        }

        public void DoMiddleClick()
        {
            DoMiddleDown();        
            DoMiddleUp();        
        }

        public void DoLeftDown()
        {
            MouseEvent(MOUSEEVENTF_LEFTDOWN);  
        }

        public void DoRightDown()
        {
            MouseEvent(MOUSEEVENTF_RIGHTDOWN);  
        }

        public void DoLeftUp()
        {
            MouseEvent(MOUSEEVENTF_LEFTUP);  
        }

        public void DoRightUp()
        {
            MouseEvent(MOUSEEVENTF_RIGHTUP);  
        }

        public void DoMiddleDown()
        {
            MouseEvent(MOUSEEVENTF_MIDDLEDOWN);  
        }

        public void DoMiddleUp()
        {
            MouseEvent(MOUSEEVENTF_MIDDLEUP);  
        }

        public void DoMouseMove(int xPos, int yPos, bool leftDown, bool rightDown)
        {
            uint flags = MOUSEEVENTF_ABSOLUTE;
            if (leftDown)
            {
                m_LeftWasDown = true;
                flags = flags | MOUSEEVENTF_LEFTDOWN;
            }
            else if(m_LeftWasDown)
            {
                m_LeftWasDown = false;
                flags = flags | MOUSEEVENTF_LEFTUP;
            }
            if (rightDown)
            {
                m_RightWasDown = true;
                flags = flags | MOUSEEVENTF_RIGHTDOWN;
            }
            else if(m_RightWasDown)
            {
                m_RightWasDown = false;
                flags = flags | MOUSEEVENTF_RIGHTUP;
            }
            SetCursorPos(xPos, yPos);
            MouseEvent(flags, xPos, yPos);
        }

        private void MouseEvent(uint flags)
        {
            MouseEvent(flags, 0,0);
        }

        private void MouseEvent(uint flags, int x, int y)
        {
            mouse_event(flags, x, y, 0, 0);
        }

        public void DoSendKey(byte keyCode)
        {
            string sendThis = string.Empty;

            switch (keyCode)
            {
                case 40: //KeyArrowDown
                    sendThis = "{DOWN}";
                    break;
                case 37: //KeyArrowLeft
                    sendThis = "{LEFT}";
                    break;
                case 39: //KeyArrowRight:
                    sendThis = "{RIGHT}";
                    break;
                case 38: //KeyArrowUp:
                    sendThis = "{UP}";
                    break;
                case 46: //KeyDelete:
                    sendThis = "{DELETE}";
                    break;
                case 17: //KeyEscape:
                    sendThis = "{ESC}";
                    break;
                case 112: //KeyF1:
                    sendThis = "{F1}";
                    break;
                case 113: //KeyF2:
                    sendThis = "{F2}";
                    break;
                case 114: //KeyF3:
                    sendThis = "{F3}";
                    break;
                case 115: //KeyF4:
                    sendThis = "{F4}";
                    break;
                case 116: //KeyF5:
                    sendThis = "{F5}";
                    break;
                case 117: //KeyF6:
                    sendThis = "{F6}";
                    break;
                case 118: //KeyF7:
                    sendThis = "{F7}";
                    break;
                case 119: //KeyF8:
                    sendThis = "{F8}";
                    break;
                case 120: //KeyF9:
                    sendThis = "{F9}";
                    break;
                case 121: //KeyF10:
                    sendThis = "{F10}";
                    break;
                case 122: //KeyF11:
                    sendThis = "{F11}";
                    break;
                case 123: //KeyF12:
                    sendThis = "{F12}";
                    break;
                default:
                    sendThis = Convert.ToChar(keyCode).ToString();
                    break;
            }

            SendKeys.SendWait(sendThis);
        }
    }
}
