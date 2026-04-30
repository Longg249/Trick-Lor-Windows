using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public sealed partial class DriverPage : Page
    {
        private List<DriverEntry> _all = new();

        public DriverPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadAsync();
        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadAsync();

        private async System.Threading.Tasks.Task LoadAsync()
        {
            TxtLoading.Visibility = Visibility.Visible;
            DgDrivers.Visibility  = Visibility.Collapsed;
            TxtStatus.Text        = "⏳ Đang truy vấn WMI...";

            _all = await DriverService.GetDriversAsync();
            ApplyFilter();

            TxtLoading.Visibility = Visibility.Collapsed;
            DgDrivers.Visibility  = Visibility.Visible;
            int unsigned = _all.Count(d => !d.IsSigned);
            TxtStatus.Text = $"Tổng: {_all.Count} driver  |  ⚠️ Chưa ký: {unsigned}";
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e) => ApplyFilter();

        private void ApplyFilter()
        {
            var q = TxtFilter.Text.Trim().ToLowerInvariant();
            DgDrivers.ItemsSource = string.IsNullOrEmpty(q)
                ? _all
                : _all.Where(d => d.Name.ToLowerInvariant().Contains(q)
                               || d.Manufacturer.ToLowerInvariant().Contains(q)).ToList();
        }
    }
}
