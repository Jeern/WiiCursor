using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;
using CursorCommon;

namespace WiiCursor
{
    public static class ScreenPositionCalculator
    {

        private static PointF m_FirstSensorPos;
        private static PointF m_SecondSensorPos;
        private static PointF m_MidSensorPos;


        /// <summary>
        /// Calculates the Cursor Position on Screen by using the Midpoint of the 2 Leds in the sensor bar
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Point GetPosition(WiimoteChangedEventArgs args)
        {
            int minXPos = 0;
            int maxXPos = Util.ScreenWidth;
            int maxWidth = maxXPos - minXPos;
            int x;
            int minYPos = 0;
            int maxYPos = Util.ScreenHeight;
            int maxHeight = maxYPos - minYPos;
            int y;

            PointF relativePosition = new PointF();
            if (args.WiimoteState.IRState.IRSensors[0].Found && args.WiimoteState.IRState.IRSensors[1].Found)
            {
                relativePosition = args.WiimoteState.IRState.Midpoint;
            }
            else if (args.WiimoteState.IRState.IRSensors[0].Found)
            {
                relativePosition.X = m_MidSensorPos.X + (args.WiimoteState.IRState.IRSensors[0].Position.X - m_FirstSensorPos.X);
                relativePosition.Y = m_MidSensorPos.Y + (args.WiimoteState.IRState.IRSensors[0].Position.Y - m_FirstSensorPos.Y);
            }
            else if (args.WiimoteState.IRState.IRSensors[1].Found)
            {
                relativePosition.X = m_MidSensorPos.X + (args.WiimoteState.IRState.IRSensors[1].Position.X - m_SecondSensorPos.X);
                relativePosition.Y = m_MidSensorPos.Y + (args.WiimoteState.IRState.IRSensors[1].Position.Y - m_SecondSensorPos.Y);
            }

            //Remember for next run
            m_FirstSensorPos = args.WiimoteState.IRState.IRSensors[0].Position;
            m_SecondSensorPos = args.WiimoteState.IRState.IRSensors[1].Position;
            m_MidSensorPos = relativePosition; 

            x = Convert.ToInt32((float)maxWidth * (1.0F - relativePosition.X)) + minXPos;
            y = Convert.ToInt32((float)maxHeight * relativePosition.Y) + minYPos;
            if (x < 0)
            {
                x = 0;
            }
            else if (x > Util.ScreenWidth)
            {
                x = Util.ScreenWidth;
            }
            if (y < 0)
            {
                y = 0;
            }
            else if (y > Util.ScreenHeight)
            {
                y = Util.ScreenHeight;
            }

            Point point = new Point();
            point.X = x;
            point.Y = y;
            return point;
        }
    }
}
