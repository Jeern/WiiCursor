using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;
using WiiCursor.WCXML;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace WiiCursor
{
    /// <summary>
    /// Main Class for connecting and disconnecting to the WiiMote
    /// </summary>
    internal class WiiMoteController
    {
        private static bool m_IsConnected;
        private Wiimote m_WiiMote;
        private static WCConfiguration m_CurrentConfiguration;
        private DispatcherTimer m_Timer;
        private int m_ConnectionRetries = 0;

        internal WiiMoteController()
        {
            m_Timer = new DispatcherTimer();
            m_Timer.Interval = new TimeSpan(0, 0, 0, 0, CurrentConfiguration.ConnectionWaitTime);
            m_Timer.Tick += (sender, eventArgs) => TryConnect();
            m_Timer.Start();
            TryConnect();       
        }

        private void TryConnect()
        {
            m_WiiMote = new Wiimote();
            m_WiiMote.WiimoteChanged += Wiimote_WiimoteChanged;
            m_ConnectionRetries++;
            Connect();
            if (m_IsConnected)
            {
                m_Timer.Stop();
            }
            else if (m_ConnectionRetries >= CurrentConfiguration.ConnectionRetries)
            {
                m_Timer.Stop();
            }
        }

        private void Connect()
        {
            try
            {
                m_WiiMote.Connect();
                m_WiiMote.SetReportType(InputReport.IRAccel, IRSensitivity.Maximum, true);
                m_WiiMote.SetLEDs(true, false, false, false);
                m_IsConnected = true;
            }
            catch
            {
                m_IsConnected = false;
            }
        }

        internal void Disconnect()
        {
            m_WiiMote.Disconnect();
            m_WiiMote.Dispose();
        }

        private void Wiimote_WiimoteChanged(object sender, WiimoteChangedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(UpdateWiimoteChanged, e);
        }


        private ButtonState m_LatestButtonState;

        private bool ButtonStateChanged(ButtonState currentState)
        {
            ButtonState latestButtonState = m_LatestButtonState;
            m_LatestButtonState = currentState;

            if (latestButtonState.A != currentState.A)
                return true;
            if (latestButtonState.B != currentState.B)
                return true;
            if (latestButtonState.Down != currentState.Down)
                return true;
            if (latestButtonState.Home != currentState.Home)
                return true;
            if (latestButtonState.Left != currentState.Left)
                return true;
            if (latestButtonState.Minus != currentState.Minus)
                return true;
            if (latestButtonState.One != currentState.One)
                return true;
            if (latestButtonState.Plus != currentState.Plus)
                return true;
            if (latestButtonState.Right != currentState.Right)
                return true;
            if (latestButtonState.Two != currentState.Two)
                return true;
            if (latestButtonState.Up != currentState.Up)
                return true;

            return false;

        }

        private void UpdateWiimoteChanged(object args)
        {
            var eventArgs = args as WiimoteChangedEventArgs;
            if (args == null)
            {
                return;
            }

            WiimoteState ws = eventArgs.WiimoteState;
            if (ws == null)
            {
                return;
            }

            var actions = new List<string>();

            //Now it is decided which actions should be taken
            if (ws.ButtonState.A)
            {
                actions.Add(CurrentConfiguration.Keys.A);
            }
            if (ws.ButtonState.B)
            {
                actions.Add(CurrentConfiguration.Keys.B);
            }
            if (ws.ButtonState.Down)
            {
                actions.Add(CurrentConfiguration.Keys.Down);
            }
            if (ws.ButtonState.Home)
            {
                actions.Add(CurrentConfiguration.Keys.Home);
            }
            if (ws.ButtonState.Left)
            {
                actions.Add(CurrentConfiguration.Keys.Left);
            }
            if (ws.ButtonState.Minus)
            {
                actions.Add(CurrentConfiguration.Keys.Minus);
            }
            if (ws.ButtonState.One)
            {
                actions.Add(CurrentConfiguration.Keys.One);
            }
            if (ws.ButtonState.Plus)
            {
                actions.Add(CurrentConfiguration.Keys.Plus);
            }
            if (ws.ButtonState.Right)
            {
                actions.Add(CurrentConfiguration.Keys.Right);
            }
            if (ws.ButtonState.Two)
            {
                actions.Add(CurrentConfiguration.Keys.Two);
            }
            if (ws.ButtonState.Up)
            {
                actions.Add(CurrentConfiguration.Keys.Up);
            }

            StringBuilder sb = new StringBuilder();
            actions.ForEach(action => { sb.Append(action); sb.Append('+'); });

            WiimoteLib.Point pos = ScreenPositionCalculator.GetPosition(eventArgs);

            bool buttonStateChanged = ButtonStateChanged(ws.ButtonState);

            using (ResponseActionController controller = new ResponseActionController(sb.ToString(), pos.X, pos.Y))
            {
                controller.Execute(buttonStateChanged);
            }
        }

        /// <summary>
        /// This is the spot where the officially current configuration of
        /// the WiiCursor is kept.
        /// </summary>
        public static WCConfiguration CurrentConfiguration
        {
            get { return m_CurrentConfiguration; }
            set { m_CurrentConfiguration = value; }
        }
    }
}
