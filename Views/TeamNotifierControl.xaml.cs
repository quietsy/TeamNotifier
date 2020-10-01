using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using TeamNotifier.ViewModels;
using System.Globalization;
using System.Windows;

namespace TeamNotifier.Views
{
    /// <summary>
    /// Interaction logic for TeamNotifierControl.xaml
    /// </summary>
    public partial class TeamNotifierControl : UserControl
    {
        public TeamNotifierControl()
        {
            InitializeComponent();

            Loaded += (s, e) => { ((TeamNotifierViewModel)DataContext).IsInitialized = true; };
        }
        
        public void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            Keyboard.ClearFocus();
        }

        private void ScheduledNotificationsTextboxMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            var result = dlg.ShowDialog();

            if (result == true)
            {
                var vm = ((TeamNotifierViewModel)textBox.DataContext);
                vm.Model.ScheduledNotificationsFile = dlg.FileName;
                textBox.Text = System.IO.Path.GetFileName(dlg.FileName);
            }
        }
    }

    public class PathToFilenameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = null;

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                result = System.IO.Path.GetFileName(value.ToString());

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
