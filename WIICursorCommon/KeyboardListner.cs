using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;

namespace CursorCommon
{
    /// <summary>
    /// Wraps the Hook functionality. This is the class to use when listening to the keyboard.
    /// Use the Event KeySignal. Remember to Dispose the KeyobardListener object after use, important since
    /// unmanaged ressources are used.
    /// </summary>
    public class KeyboardListener : IDisposable
    {
        private IntPtr m_HookId;

        //Keyboard API constants
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref HookInfo lParam);

        private Dictionary<int, bool> m_KeySignals = new Dictionary<int, bool>();
        private HookHandlerDelegate m_Proc;

        public KeyboardListener()
        {
            m_Proc = HookCallback;
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    m_HookId = SetWindowsHookEx(WH_KEYBOARD_LL, m_Proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private event EventHandler<CCKeyboardEventArgs> m_KeySignal = delegate { };

        public event EventHandler<CCKeyboardEventArgs> KeySignal
        {
            add { m_KeySignal += value; }
            remove { m_KeySignal -= value; }
        }

        //private bool m_KillNextEvent =false;

        ///// <summary>
        ///// This one indicates that the next Key event should "die"
        ///// Used if only the customized keylistnening should be used. I.e no one else can catch the event.
        ///// </summary>
        //public bool KillNextEvent 
        //{ 
        //    get
        //    {
        //        bool killNextEvent = m_KillNextEvent;
        //        m_KillNextEvent = false;
        //        return killNextEvent;
        //    }
        //    set
        //    {
        //        m_KillNextEvent = value;
        //    }
        //}

        private IntPtr HookCallback(int nCode, IntPtr wParam, ref HookInfo lParam)
        {
            bool keyPressed = false;
            bool signalKey = false;
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                keyPressed = true;
                signalKey = true;
            }
            else if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
            {
                keyPressed = false;
                signalKey = true;
            }
            Debug.WriteLine("wParam = " + wParam.ToString() + " - vkCode = " + lParam.vkCode.ToString(CultureInfo.InvariantCulture) + " - flags = " + lParam.flags.ToString(CultureInfo.InvariantCulture));

            if (signalKey)
            {
                AddKeySignals(lParam.vkCode, keyPressed);
            }

            IntPtr nextHook;
            //if (KillNextEvent)
            //{
            //    nextHook = (IntPtr)1;
            //}
            //else
            //{
                nextHook = CallNextHookEx(m_HookId, nCode, wParam, ref lParam);
            //}

            //if (signalKey)
            //{
            //    AddKeySignals(lParam.vkCode, keyPressed);
            //}
            return nextHook;
        }

        private void AddKeySignals(int keyCode, bool keyPressed)
        {
            if (!m_KeySignals.ContainsKey(keyCode))
            {
                if (keyPressed)
                {
                    m_KeySignals.Add(keyCode, keyPressed);
                    m_KeySignal(this, new CCKeyboardEventArgs(keyCode, keyPressed));
                }
            }
            else
            {
                if (m_KeySignals[keyCode] != keyPressed)
                {
                    m_KeySignals[keyCode] = keyPressed;
                    m_KeySignal(this, new CCKeyboardEventArgs(keyCode, keyPressed));
                }
            }

        }

        #region DllImports

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref HookInfo lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

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

        ~KeyboardListener()
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
            //Not necessesary but whatever... FreeManaged is really only for calling Dispose on members that 
            //are Disposable .NET classes.
            m_Proc = null;
            m_KeySignal = delegate { };
            m_KeySignals = null;
        }

        protected virtual void FreeUnmanaged()
        {
            UnhookWindowsHookEx(m_HookId);
        }

        #endregion

    }
}
