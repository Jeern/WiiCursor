using System.Windows.Forms;
using System;

namespace CursorCommon
{
    public static class Util
    {
        /// <summary>
        /// The Width of the Primary Screen.
        /// </summary>
        public static int ScreenWidth
        {
            get { return Screen.PrimaryScreen.Bounds.Width; }
        }

        /// <summary>
        /// The Height of the primary screen.
        /// </summary>
        public static int ScreenHeight
        {
            get { return Screen.PrimaryScreen.Bounds.Height; }
        }

        public static MenuItem GetMenuItem(string name, EventHandler onClick, bool itemChecked)
        {
            var menuItem = new MenuItem(name, onClick);
            menuItem.Checked = itemChecked;
            return menuItem;
        }

        public static void UncheckSubMenuItems(MenuItem item)
        {
            if (item == null)
                return;

            if (item.MenuItems == null)
                return;

            foreach (MenuItem subItem in item.MenuItems)
            {
                subItem.Checked = false;
            }
        }
    }
}
