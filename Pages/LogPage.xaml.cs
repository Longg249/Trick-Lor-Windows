using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public sealed partial class LogPage : Page
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
            LogScroll.ChangeView(null, double.MaxValue, null);
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            LogService.Clear();
            Refresh();
        }
    }
}
