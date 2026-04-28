using System.Windows;
using System.Windows.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class LogPage : Page
    {
        public LogPage()
        {
            InitializeComponent();
            Loaded += (s, e) => Refresh();
        }

        private void Refresh()
        {
            LogList.ItemsSource = null;
            LogList.ItemsSource = LogService.Entries;
            TxtLogCount.Text = $"{LogService.Entries.Count} bản ghi";
            LogScroll.ScrollToBottom();
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            LogService.Clear();
            Refresh();
        }
    }
}
