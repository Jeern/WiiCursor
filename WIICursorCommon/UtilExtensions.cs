using System.Windows;

namespace CursorCommon
{
    public static class UtilExtensions
    {
        /// <summary>
        /// Brings the window to the absolute Front
        /// </summary>
        /// <param name="?"></param>
        public static void BringToFront(this Window window)
        {
            //Far out, but at this point Topmost might be true, but the windows isn't necessarily the Topmost of Topmost windows.
            //By setting Topmost to false, and then to true. The window becomes the Topmost'est of all.
            window.Topmost = false;
            window.Topmost = true;
        }

    }
}
