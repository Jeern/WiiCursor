using System;
using System.Windows;

namespace DrawCursor
{
    /// <summary>
    /// Interaction logic for Messages.xaml
    /// </summary>
    public partial class Messages : Window
    {
        public Messages()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            System.Threading.Thread.Sleep(1000);
            Visibility = Visibility.Hidden;
        }
    }
}
