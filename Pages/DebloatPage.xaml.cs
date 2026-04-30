using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public sealed partial class DebloatPage : Page
    {
        private ObservableCollection<BloatwareItem> _allItems = new();
        private ObservableCollection<BloatwareItem> _filteredItems = new();

        public DebloatPage()
        {
            InitializeComponent();
            BloatwareList.ItemsSource = _filteredItems;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TxtStatus.Text = "⏳ Đang quét bloatware đã cài...";
            var list = await DebloatService.GetInstalledBloatwareAsync();
            _allItems.Clear();
            foreach (var item in list)
                _allItems.Add(item);
            ApplyFilter();
            TxtStatus.Text = $"Tìm thấy {_allItems.Count} bloatware đang cài đặt";
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        private void ApplyFilter()
        {
            var keyword = TxtSearch.Text.Trim().ToLower();
            _filteredItems.Clear();
            var filtered = string.IsNullOrEmpty(keyword)
                ? _allItems
                : _allItems.Where(x => x.DisplayName.ToLower().Contains(keyword));
            foreach (var item in filtered)
                _filteredItems.Add(item);
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _filteredItems)
                item.IsSelected = true;
        }

        private void SelectNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _filteredItems)
                item.IsSelected = false;
        }

        private async void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            var selected = _filteredItems.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0)
            {
                TxtStatus.Text = "⚠ Chưa chọn bloatware nào!";
                return;
            }

            BtnUninstall.IsEnabled = false;
            int done = 0;
            int total = selected.Count;

            void UpdateStatus(string msg)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    TxtProgress.Text = msg;
                    TxtStatus.Text = msg;
                });
            }

            foreach (var item in selected)
            {
                UpdateStatus($"Đang gỡ {item.DisplayName}...");
                try
                {
                    await DebloatService.RemoveAppxAsync(item.PackageName, new Progress<string>(UpdateStatus));
                    _allItems.Remove(item);
                    done++;
                    UpdateStatus($"✅ Đã gỡ {item.DisplayName} ({done}/{total})");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"❌ Lỗi khi gỡ {item.DisplayName}: {ex.Message}");
                }
            }
            ApplyFilter();
            TxtStatus.Text = $"Hoàn tất: đã gỡ {done}/{total} bloatware";
            BtnUninstall.IsEnabled = true;
            LogService.Add($"Debloat: Đã gỡ {done} bloatware");
        }

        private async void CreateRestore_Click(object sender, RoutedEventArgs e)
        {
            BtnRestorePoint.IsEnabled = false;
            BtnUninstall.IsEnabled = false;
            void UpdateStatus(string msg)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    TxtProgress.Text = msg;
                    TxtStatus.Text = msg;
                });
            }
            var result = await DebloatService.CreateRestorePointAsync(new Progress<string>(UpdateStatus));
            UpdateStatus(result);
            BtnRestorePoint.IsEnabled = true;
            BtnUninstall.IsEnabled = true;
            LogService.Add($"Debloat: {result}");
        }

        private void OpenRestore_Click(object sender, RoutedEventArgs e)
            => DebloatService.OpenSystemRestoreUI();
    }
}