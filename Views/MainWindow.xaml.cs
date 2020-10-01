using System.Windows;
using System.Windows.Input;

namespace TeamNotifier.Views
{
    public partial class MainWindow : Window
    {
        public static bool IsCommandFocused { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            Keyboard.ClearFocus();
        }
    }
}
