using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using CursorCommon;
using DrawCursor.XML;
using System.Windows.Ink;

namespace DrawCursor
{
    /// <summary>
    /// Interaction logic for DrawWindow.xaml
    /// </summary>
    public partial class DrawWindow : Window
    {
        internal DrawWindow()
        {
            InitializeComponent();

            CurrentState = DrawState.TransparentNoDrawing;

            //Size of DrawWindow is read from config file
            Width = Util.ScreenWidth; // Config.AnnotationSquareSize;
            Height = Util.ScreenHeight; // Config.AnnotationSquareSize;

            //DrawWindow skal tegnes lidt forskudt af Cursor, da den er i nederste højre hjørne af sit window
            Top = 0; // TopOfCursor + 100 - (Height / 2);
            Left = 0; // LeftOfCursor + 75 - (Width / 2);

            var xmlConfig = new XMLConfig<DCConfiguration>("DCXML/DCConfiguration.xsd");
            var liste = xmlConfig.ReadAll("DCXML");

            Configure(liste.Find(c => c.Default));
        }

        /// <summary>
        /// This is were the real configuration takes places. The Values of the XML file is actually put to use.
        /// </summary>
        /// <param name="configuration"></param>
        internal void Configure(DCConfiguration configuration)
        {
            DrawArea.DefaultDrawingAttributes.Width = configuration.MarkerSize;
            DrawArea.DefaultDrawingAttributes.Height = configuration.MarkerSize;
            DrawArea.DefaultDrawingAttributes.Color = configuration.MarkerColor;
        }

        /// <summary>
        /// The Current State of the application. Toggles between 3 DrawStates by userinput. Usually Crtl key.
        /// </summary>
        internal DrawState CurrentState
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the Drawstate of the window. Can the user draw or interact with desktop and/or see what was drawn earlier.
        /// </summary>
        internal void ShowState()
        {
            ShowState(CurrentState);
        }


        /// <summary>
        /// Sets the Drawstate of the window. Can the user draw or interact with desktop and/or see what was drawn earlier.
        /// </summary>
        /// <param name="state"></param>
        internal void ShowState(DrawState state)
        {
            switch (state)
            {
                case DrawState.TransparentCanDrawNoDesktopInteraction:
                    //This first state is not really transparent, a bitmap of the desktop is used as
                    //background. Necessary to use the InkCanvas to draw. InkCanvas cannot be used if the
                    //window is truly transparent
                    DrawArea.EditingMode = InkCanvasEditingMode.Ink;
                    MakeOpaqueToEvents();
                    IsHitTestVisible = true;
                    Opacity = 1;
                    using (BitmapHandler handler = new BitmapHandler())
                    {
                        BackGroundImage.Source = handler.GetDesktopBitmapSource((int)Left, (int)Top, (int)Width, (int)Height);
                    }
                    ClearValue(Window.BackgroundProperty);
                    Show();
                    CurrentState = state;
                    break;
                case DrawState.SemitransparentDrawingFixed:
                    //Changes to a semitransparent state where the drawing is fixed. But it is possible to interact
                    //with the desktop.
                    DrawArea.EditingMode = InkCanvasEditingMode.Ink;
                    MakeTransparentToEvents();
                    IsHitTestVisible = false;
                    Opacity = 0.5;
                    BackGroundImage.ClearValue(Image.SourceProperty);
                    Background = Brushes.White;
                    Show();
                    CurrentState = state;
                    break;
                case DrawState.TransparentNoDrawing:
                    DrawArea.EditingMode = InkCanvasEditingMode.Ink;
                    MakeOpaqueToEvents();
                    IsHitTestVisible = true;
                    Opacity = 0;
                    BackGroundImage.ClearValue(Image.SourceProperty);
                    Background = Brushes.Transparent;
                    Show();
                    CurrentState = state;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("state");
            }
        }

        /// <summary>
        /// Changes Edit mode between drawing and deleting. But only if the state is TransparentCanDrawNoDesktopInteraction
        /// </summary>
        internal void ChangeEditMode()
        {
            if (CurrentState != DrawState.TransparentCanDrawNoDesktopInteraction)
                return;

            if (DrawArea.EditingMode == InkCanvasEditingMode.Ink)
            {
                DrawArea.EditingMode = InkCanvasEditingMode.EraseByPoint;
                DrawArea.EraserShape = new EllipseStylusShape(30.0, 30.0);
            }
            else
            {
                DrawArea.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        //The Event transparency below makes the window transparent to MouseClick which makes the user able to interact with stuff
        //beneath the window. This is relevant for the Semi-Transparent state (SemiTransparentDrawingFixed) only

        #region Event Transparency

        private int m_ExtendedStyle = 0;

        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private void MakeTransparentToEvents()
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Change the extended window style to include WS_EX_TRANSPARENT
            m_ExtendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, m_ExtendedStyle | WS_EX_TRANSPARENT);
        }

        private void MakeOpaqueToEvents()
        {
            if (m_ExtendedStyle == 0)
                return;

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Change Style back
            SetWindowLong(hwnd, GWL_EXSTYLE, m_ExtendedStyle);
            m_ExtendedStyle = 0;
        }

        #endregion


    }
}
