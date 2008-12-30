using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using CursorCommon;
using WiiCursor.WCXML;

namespace WiiCursor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private InputSimulator m_InputSimulator;
        private System.Windows.Forms.NotifyIcon m_TrayIcon;
        private System.Windows.Forms.MenuItem m_ConfigurationMenu = null;

        /// <summary>
        /// In the constructor we check if the App is already started. It is only allowed to start once.
        /// </summary>
        public App()
        {
            bool createdNew;
            using (var mutex = new Mutex(true, "WiiCursor {3D276EDF-7306-40df-ACD3-EEA9513BC7DA}", out createdNew))
            {
                if (!createdNew)
                {
                    //Already started. Exit at once
                    Application.Current.Shutdown();
                }
            }

            AppDomain.CurrentDomain.UnhandledException +=new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);

        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.ExceptionObject as Exception);
        }

        private void HandleUnhandledException(Exception ex)
        {
            m_WiiMoteController.Disconnect();
            if (ex == null)
                return;

            MessageBox.Show("Unhandled Exception WiiCursor: " + ex.ToString());
        }

        private WiiMoteController m_WiiMoteController;

        protected override void OnStartup(StartupEventArgs e)
        {
 	         base.OnStartup(e);

            m_InputSimulator = new InputSimulator();

             //Read Configuration files 
             List<WCConfiguration> configuration = Configure();

             //TrayIcon sættes op.
             InitializeTrayIcon(configuration);

             m_WiiMoteController = new WiiMoteController();
        }

        /// <summary>
        /// Puts the Icon into the System Tray. Not yet possible in WPF
        /// </summary>
        private void InitializeTrayIcon(List<WCConfiguration> configurations)
        {
            m_TrayIcon = new System.Windows.Forms.NotifyIcon();
            m_TrayIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("WiiCursor.wc128c.ico"));
            m_TrayIcon.Visible = true;
            m_TrayIcon.Text = "WiiCursor";

            //Configuration Menuitems setup
            m_ConfigurationMenu = new System.Windows.Forms.MenuItem("Configure");

            var confMenus = from configuration in configurations
                            select Util.GetMenuItem(configuration.Name, 
                                (sender, args) => Configure((System.Windows.Forms.MenuItem)sender, configuration), 
                                configuration.Default);

            m_ConfigurationMenu.MenuItems.AddRange(confMenus.ToArray());

            var menuItems = new System.Windows.Forms.MenuItem[]
            {
                m_ConfigurationMenu,
                new System.Windows.Forms.MenuItem("-"),
                new System.Windows.Forms.MenuItem("&Exit", (sender, args) => HandleExitClick())
            };

            m_TrayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(menuItems);
        }

        private void HandleExitClick()
        {
            CloseDownEverything();
            Application.Current.Shutdown();
        }

        private void CloseDownEverything()
        {
            GetRidOfTrayIcon();
            m_InputSimulator = null;
        }

        private void GetRidOfTrayIcon()
        {
            if (m_TrayIcon != null)
            {
                m_TrayIcon.Visible = false;
                m_TrayIcon.Dispose();
            }
        }

        /// <summary>
        /// This is the initial configuration, configures the application with the XML file that has a Default Tag marked true
        /// </summary>
        /// <returns></returns>
        private List<WCConfiguration> Configure()
        {
            var xmlConfig = new XMLConfig<WCConfiguration>("WCXML/WCConfiguration.xsd");
            List<WCConfiguration> liste = xmlConfig.ReadAll("WCXML");
            Configure(liste.Find(c => c.Default));
            return liste;
        }

        private void Configure(System.Windows.Forms.MenuItem menuItem, WCConfiguration configuration)
        {
            Util.UncheckSubMenuItems(m_ConfigurationMenu);
            menuItem.Checked = true;
            Configure(configuration);
        }

        private void Configure(WCConfiguration configuration)
        {
            WiiMoteController.CurrentConfiguration = configuration;
        }



    }
}
