using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CursorCommon
{
    /// <summary>
    /// This class handles all about Bitmaps, sadly I need to use Interop. Because .NET code is not capable of capturing
    /// the entire screen.
    /// According to the info I could find:
    /// Image.FromHbitmap references native handle
    /// bitmap.GetHbitmap exposes that reference
    /// While Imaging.CreateBitmapSourceFromHBitmap copies it.
    /// These three things are the reasons for the way cleanup is done in this class.
    /// However every time the class is used a lot of memory is added to the process. All Managed Memory as far as I
    /// can see, and thus will be GC'ed at some point.
    /// </summary>
    public class BitmapHandler : IDisposable
    {
        private const int SRCCOPY = 13369376;  //  &HCC0020;

        [DllImport("gdi32")]
        private static extern int CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, string lpInitData);
        [DllImport("gdi32")]
        private static extern int CreateCompatibleDC(int hDC);
        [DllImport("gdi32")]
        private static extern int CreateCompatibleBitmap(int hDC, int nWidth, int nHeight);
        [DllImport("gdi32")]
        private static extern int GetDeviceCaps(int hdc, int nIndex);
        [DllImport("gdi32")]
        private static extern int SelectObject(int hDC, int hObject);
        [DllImport("gdi32")]
        private static extern int BitBlt(int srchDC, int srcX, int srcY, int srcW, int srcH, int desthDC, int destX, int destY, int op);
        [DllImport("gdi32")]
        private static extern int DeleteDC(int hDC);
        [DllImport("gdi32")]
        private static extern int DeleteObject(int hObj);


        private int m_HSDC;
        private int m_HMDC;
        private int m_HBMP;
        private int m_HBMPOld;

        /// <summary>
        /// The Actual Screen Capture. Returns af System.Drawing Bitmap. Needs to be converted later, for use with
        /// WPF.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Bitmap CaptureScreen(int left, int top, int width, int height)
        {
            m_HSDC = CreateDC("DISPLAY", "", "", "");
            m_HMDC = CreateCompatibleDC(m_HSDC);
            m_HBMP = CreateCompatibleBitmap(m_HSDC, width, height);
            m_HBMPOld = SelectObject(m_HMDC, m_HBMP);
            BitBlt(m_HMDC, 0, 0, width, height, m_HSDC, left, top, SRCCOPY);
            m_HBMP = SelectObject(m_HMDC, m_HBMPOld);
            return Image.FromHbitmap(new IntPtr(m_HBMP)); 
        }

        ///// <summary>
        ///// Captures entire screen.
        ///// </summary>
        //private Bitmap CaptureScreen()
        //{
        //    return CaptureScreen(0, 0, Util.ScreenWidth, Util.ScreenHeight);
        //}
        
        /// <summary>
        /// Gets the BitmapSource for a given area of the desktop. The BitmapSource can be used by WPF
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public BitmapSource GetDesktopBitmapSource(int left, int top, int width, int height)
        {
            return ConvertBitmap(CaptureScreen(left, top, width, height));  
        }

        /// <summary>
        /// Converts a System.Drawing Bitmap to the WPF BitmapSource.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private static BitmapSource ConvertBitmap(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, 
                Int32Rect.Empty, 
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
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

        ~BitmapHandler()
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
            //Nothing
        }

        protected virtual void FreeUnmanaged()
        {
            DeleteObject(m_HBMP);
            DeleteObject(m_HBMPOld); 
            DeleteDC(m_HMDC);
            DeleteDC(m_HSDC);
        }

        #endregion


    }
}
