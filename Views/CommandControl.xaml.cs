using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TeamNotifier.Views
{
    /// <summary>
    /// Interaction logic for CommandControl.xaml
    /// </summary>
    public partial class CommandControl : UserControl
    {
        public CommandControl()
        {
            InitializeComponent();
        }

        private string savedText;
        
        private void TimeReminderGotFocus(object sender, RoutedEventArgs e)
        {
            MainWindow.IsCommandFocused = true;
            var textBox = sender as MaskedTextBox;

            savedText = textBox.Text;
            textBox.Text = string.Empty;
        }

        private void TimeReminderLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            MainWindow.IsCommandFocused = false;
            var textBox = sender as MaskedTextBox;
            TimeSpan time;
            bool valid = TimeSpan.TryParseExact(textBox.Text, "g",
                       CultureInfo.CurrentCulture, out time);
            if (textBox.Text == "__:__:__" && savedText != null)
            {
                textBox.Text = savedText;
                savedText = null;
            }
            else if (!textBox.IsMaskFull || !valid)
                textBox.Text = TimeSpan.Zero.ToString();

        }

        private void TimeReminderKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as MaskedTextBox;

            if (textBox.Text == "__:__:__")
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;
            }
        }

        private void Hotkey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var modifiers = Keyboard.Modifiers;
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            if (modifiers == ModifierKeys.None && 
                (key == Key.Delete || key == Key.Escape || key == Key.Back))
            {
                return;
            }

            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                HotkeyTextBox.Text = "";
                return;
            }

            var str = new StringBuilder();

            if (modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");

            str.Append(key);

            HotkeyTextBox.Text = str.ToString();
        }

        private void HotkeyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MainWindow.IsCommandFocused = true;
        }

        private void HotkeyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            MainWindow.IsCommandFocused = false;
        }
    }

    public class DelayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TimeSpan)value != null && (TimeSpan)value != TimeSpan.Zero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
