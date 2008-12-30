using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CursorCommon;
using System.Windows;

namespace WiiCursor
{
    /// <summary>
    /// This class makes is meant for parsing and executing the ResponseActions.
    /// ResponseActions are seperated by + in the config file. If Several Actions are connected to One Wii Button,
    /// They are executed from left to right. 
    /// </summary>
    public class ResponseActionController : IDisposable
    {
        private List<ResponseAction>  m_Actions;
        private InputSimulator m_InputSimulator = new InputSimulator();
        private int m_MoveToX;
        private int m_MoveToY;
        private static bool m_Paused = false;

        public ResponseActionController(string actions, int moveToX, int moveToY)
        {
            if (!string.IsNullOrEmpty(actions))
            {
                m_Actions = (from action in actions.Split('+')
                             select ConvertToResponseAction(action)).ToList();
            }
            m_MoveToX = moveToX;
            m_MoveToY = moveToY;
        }

        private static ResponseAction ConvertToResponseAction(string action)
        {
            if (string.IsNullOrEmpty(action))
                return new ResponseAction(ResponseActions.None);

            if(Enum.GetNames(typeof(ResponseActions)).Contains(action))
            {
                return new ResponseAction((ResponseActions)Enum.Parse(typeof(ResponseActions), action));
            }

            byte keyAction;
            if (byte.TryParse(action, out keyAction))
            {
                return new ResponseAction(keyAction);
            }

            return new ResponseAction(ResponseActions.None);
        }

        private static bool pressed = false;
        public void Execute(bool buttonStateChanged)
        {

            lock (this)
            {
                bool mouseLeftButtonDownPressed = false;
                bool mouseRightButtonDownPressed = false;
                if (m_Actions != null)
                {
                    mouseLeftButtonDownPressed = m_Actions.Exists(a => a.Action == ResponseActions.MouseLeftButtonDown);
                    mouseRightButtonDownPressed = m_Actions.Exists(a => a.Action == ResponseActions.MouseRightButtonDown);
                    m_Actions.ForEach(action => Execute(buttonStateChanged, action));
                }
                if (mouseLeftButtonDownPressed && !pressed)
                {
                    pressed = true;
                    m_InputSimulator.DoLeftDown();
                }
                if (m_Paused)
                    return;

                ExecuteMouseMoveAction(mouseLeftButtonDownPressed, mouseRightButtonDownPressed);
            }
        }

        private void Execute(bool buttonStateChanged, ResponseAction action)
        {
            if (!buttonStateChanged)
                return;

            if (action.KeyAction > 0)
            {
                ExecuteKeyAction(action.KeyAction);
                return;
            }

            Execute(action.Action);

        }

        private void Execute(ResponseActions action)
        {
            if (action.ToString().StartsWith("Key"))
            {
                if (m_Paused)
                    return;
                ExecuteKeyAction(action);
                return;
            }
            if (action.ToString().StartsWith("Mouse"))
            {
                if (m_Paused)
                    return;
                ExecuteMouseAction(action);
                return;
            }

            ExecuteOtherAction(action);

        }

        private void ExecuteKeyAction(byte keyAction)
        {
            m_InputSimulator.DoSendKey(keyAction);   
        }

        private void ExecuteKeyAction(ResponseActions action)
        {
            switch (action)
            {
                case ResponseActions.KeyArrowDown:
                    ExecuteKeyAction(40);
                    break;
                case ResponseActions.KeyArrowLeft:
                    ExecuteKeyAction(37);
                    break;
                case ResponseActions.KeyArrowRight:
                    ExecuteKeyAction(39);
                    break;
                case ResponseActions.KeyArrowUp:
                    ExecuteKeyAction(38);
                    break;
                case ResponseActions.KeyDelete:
                    ExecuteKeyAction(46);
                    break;
                case ResponseActions.KeyEscape:
                    ExecuteKeyAction(27);
                    break;
                case ResponseActions.KeyF1:
                    ExecuteKeyAction(112);
                    break;
                case ResponseActions.KeyF2:
                    ExecuteKeyAction(113);
                    break;
                case ResponseActions.KeyF3:
                    ExecuteKeyAction(114);
                    break;
                case ResponseActions.KeyF4:
                    ExecuteKeyAction(115);
                    break;
                case ResponseActions.KeyF5:
                    ExecuteKeyAction(116);
                    break;
                case ResponseActions.KeyF6:
                    ExecuteKeyAction(117);
                    break;
                case ResponseActions.KeyF7:
                    ExecuteKeyAction(118);
                    break;
                case ResponseActions.KeyF8:
                    ExecuteKeyAction(119);
                    break;
                case ResponseActions.KeyF9:
                    ExecuteKeyAction(120);
                    break;
                case ResponseActions.KeyF10:
                    ExecuteKeyAction(121);
                    break;
                case ResponseActions.KeyF11:
                    ExecuteKeyAction(122);
                    break;
                case ResponseActions.KeyF12:
                    ExecuteKeyAction(123);
                    break;
                default:
                    throw new ArgumentException(string.Format("ExecuteKeyAction called with wrong argument action = {0}", action));
            }
        }

        private void ExecuteMouseAction(ResponseActions action)
        {
            switch (action)
            {
                case ResponseActions.MouseLeftButtonDown:
                    //Do Nothing - handled by MoveAction
                    break;
                case ResponseActions.MouseLeftButtonUp:
                    //Do Nothing - handled by MoveAction
                    break;
                case ResponseActions.MouseLeftClick:
                    m_InputSimulator.DoLeftClick();
                    break;
                case ResponseActions.MouseLeftDoubleClick:
                    m_InputSimulator.DoDoubleClick(); 
                    break;
                case ResponseActions.MouseRightButtonDown:
                    //Do Nothing - handled by MoveAction
                    break;
                case ResponseActions.MouseRightButtonUp:
                    //Do Nothing - handled by MoveAction
                    break;
                case ResponseActions.MouseRightClick:
                    m_InputSimulator.DoRightClick();
                    break;
                default:
                    throw new ArgumentException(string.Format("ExecuteMouseAction called with wrong argument action = {0}", action));
            }
        }

        private void ExecuteMouseMoveAction(bool leftDown, bool rightDown)
        {
            m_InputSimulator.DoMouseMove(m_MoveToX, m_MoveToY, leftDown, rightDown);
        }

        private void ExecuteOtherAction(ResponseActions action)
        {
            switch (action)
            {
                case ResponseActions.None:
                    return;
                case ResponseActions.Exit:
                    Environment.Exit(0);
                    break;
                case ResponseActions.Pause:
                    m_Paused = !m_Paused;
                    break;
                default:
                    throw new ArgumentException(string.Format("ExecuteOtherAction called with wrong argument action = {0}", action));
            }
        }

        #region DisposePattern

        private bool m_AlreadyDisposed;

        public void Dispose()
        {
            lock (this)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        ~ResponseActionController()
        {
            Dispose(false);
        }

        private void Dispose(bool isDisposing)
        {
            if (m_AlreadyDisposed)
                return;

            if (isDisposing)
            {
                FreeManaged();
            }
            FreeUnmanaged();
            m_AlreadyDisposed = true;
        }

        protected virtual void FreeManaged()
        {
            if (m_Actions != null)
            {
                m_Actions.Clear();
            }
        }

        protected virtual void FreeUnmanaged()
        {
            //Nothing
        }

        #endregion



    }
}
