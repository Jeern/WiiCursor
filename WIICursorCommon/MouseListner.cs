using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CursorCommon
{
    /// <summary>
    /// Wraps Native Mouse Hook functionality. Remember to dispose after use since it contains Unmanaged Ressources.
    /// </summary>
    public class MouseListener : IDisposable
    {
        private IntPtr m_HookId;

        //Mouse API constants
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        private delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref MouseHookInfo lParam);

        private HookHandlerDelegate m_Proc;

        public MouseListener()
        {
            m_Proc = HookCallback;
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    m_HookId = SetWindowsHookEx(WH_MOUSE_LL, m_Proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private event EventHandler<CCMouseEventArgs> m_MouseSignal = delegate { };

        public event EventHandler<CCMouseEventArgs> MouseSignal
        {
            add { m_MouseSignal += value; }
            remove { m_MouseSignal -= value; }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, ref MouseHookInfo lParam)
        {
            if (nCode >= 0)
            {
                m_MouseSignal(this, new CCMouseEventArgs(ConvertToMouseAction(wParam),  lParam.pt)); // new Mo(lParam.pt.X, lParam.pt.Y)));
            }
            return CallNextHookEx(m_HookId, nCode, wParam, ref lParam);
        }

        private MouseActions ConvertToMouseAction(IntPtr param)
        {
            switch ((int)param)
            {
                case WM_LBUTTONDOWN:
                    return MouseActions.LeftButtonDown;
                case WM_LBUTTONUP:
                    return MouseActions.LeftButtonUp;
                case WM_MOUSEMOVE:
                    return MouseActions.MouseMove;
                case WM_MOUSEWHEEL:
                    return MouseActions.MouseWheel;
                case WM_RBUTTONDOWN:
                    return MouseActions.RightButtonDown;
                case WM_RBUTTONUP:
                    return MouseActions.RightButtonUp;
                default:
                    throw new ArgumentOutOfRangeException("param");
            }
        }

        #region DllImports

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref MouseHookInfo lParam);

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

        ~MouseListener()
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
            m_MouseSignal = delegate { };
        }

        protected virtual void FreeUnmanaged()
        {
            UnhookWindowsHookEx(m_HookId);
        }

        #endregion
    }
}
