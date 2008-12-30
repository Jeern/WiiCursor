using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using CursorCommon;
using DrawCursor.XML;

namespace DrawCursor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The DrawWindow is an InkCanvas, by utilizing WPF we easily get the desired draw on screen functionality
        /// </summary>
        private DrawWindow m_DrawWindow = new DrawWindow();

        private KeyboardListener m_KeyboardListener = null;
        private System.Windows.Forms.NotifyIcon m_TrayIcon;
        private System.Windows.Forms.MenuItem m_ConfigurationMenu = null;

        /// <summary>
        /// In the constructor we check if the App is already started. It is only allowed to start once.
        /// </summary>
        public App()
        {
            bool createdNew;
            using (var mutex = new Mutex(true, "DrawCursor {048C724A-8444-4ad3-A28F-8579E771F9AD}", out createdNew))
            {
                if (!createdNew)
                {
                    //Already started. Exit at once
                    Application.Current.Shutdown();
                }
            }
        }

        /// <summary>
        /// Listener for Keyboard is set up.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            m_KeyboardListener = new KeyboardListener();
            m_KeyboardListener.KeySignal += new EventHandler<CCKeyboardEventArgs>(KeySignaled);

            //Read Configuration files 
            List<DCConfiguration> configuration = Configure();

            //TrayIcon sættes op.
            InitializeTrayIcon(configuration);
        }

        /// <summary>
        /// This is the initial configuration, configures the application with the XML file that has a Default Tag marked true
        /// </summary>
        /// <returns></returns>
        private List<DCConfiguration> Configure()
        {
            var xmlConfig = new XMLConfig<DCConfiguration>("DCXML/DCConfiguration.xsd");
            List<DCConfiguration> liste = xmlConfig.ReadAll("DCXML");            
            Configure(liste.Find(c => c.Default));
            return liste;
        }

        private void Configure(System.Windows.Forms.MenuItem menuItem, DCConfiguration configuration)
        {
            Util.UncheckSubMenuItems(m_ConfigurationMenu);
            menuItem.Checked = true;
            Configure(configuration);
        }

        private void Configure(DCConfiguration configuration)
        {
            m_DrawWindow.Configure(configuration);
        }


        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            CloseDownEverything();
            base.OnSessionEnding(e);
        }

        /// <summary>
        /// Puts the Icon into the System Tray. Not yet possible in WPF
        /// </summary>
        private void InitializeTrayIcon(List<DCConfiguration> configurations)
        {
            m_TrayIcon = new System.Windows.Forms.NotifyIcon();
            m_TrayIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("DrawCursor.dc128c.ico"));
            m_TrayIcon.Visible = true;
            m_TrayIcon.Text = "DrawCursor";

            //Configuration Menuitems setup. Only WinForms MenuItems can go to Tray
            //I guess MS will probably extend WPF in the future. This is quite an important feature to lack
            m_ConfigurationMenu = new System.Windows.Forms.MenuItem("Configure");

            var confMenus = from configuration in configurations
                        select Util.GetMenuItem(
                            configuration.Name,
                            (sender, args) => Configure((System.Windows.Forms.MenuItem)sender, configuration),
                            configuration.Default);

            m_ConfigurationMenu.MenuItems.AddRange(confMenus.ToArray()); 

            var menuItems = new System.Windows.Forms.MenuItem[]
            {
                m_ConfigurationMenu,
                new System.Windows.Forms.MenuItem("-"),
                new System.Windows.Forms.MenuItem("&Exit", (sender, args) => HandleExitClick())
            };

            var contextMenu = m_TrayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(menuItems);
        }

        private void HandleExitClick()
        {
            CloseDownEverything();
            Application.Current.Shutdown();
        }

        private void CloseDownEverything()
        {
            m_DrawWindow.Close();
            GetRidOfTrayIcon();
            if (m_KeyboardListener != null)
            {
                m_KeyboardListener.Dispose();
            }
        }

        private void GetRidOfTrayIcon()
        {
            if (m_TrayIcon != null)
            {
                m_TrayIcon.Visible = false;
                m_TrayIcon.Dispose();
            }
        }


        private void KeySignaled(object sender, CCKeyboardEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (int)CheckKeys.F7:
                    if (e.KeyUp)
                    {
                        m_DrawWindow.CurrentState = NextState(m_DrawWindow.CurrentState);
                        m_DrawWindow.ShowState();
                    }
                    break;
                case (int)CheckKeys.Delete:
                    if (e.KeyUp && m_DrawWindow != null)
                    {
                        m_DrawWindow.ChangeEditMode();
                    }
                    break;
                case (int)CheckKeys.Escape:
                    if (e.KeyUp) // Means Key Released
                        HandleExitClick();
                    break;
                default:
                    break;
            }
        }

        private DrawState NextState(DrawState state)
        {
            switch (state)
            {
                case DrawState.TransparentCanDrawNoDesktopInteraction:
                    return DrawState.SemitransparentDrawingFixed;
                case DrawState.SemitransparentDrawingFixed:
                    return DrawState.TransparentNoDrawing;
                case DrawState.TransparentNoDrawing:
                    return DrawState.TransparentCanDrawNoDesktopInteraction;
                default:
                    throw new ArgumentOutOfRangeException("state");
            }
        }
    }
}
