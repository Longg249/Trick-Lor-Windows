using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public sealed partial class OfficeInstallPage : Page
    {
        public OfficeInstallPage() => InitializeComponent();

        private async void Install_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị popup cảnh báo bản quyền
            bool agreed = await ShowCopyrightWarningAsync();
            if (!agreed) return;

            BtnInstall.IsEnabled = false;
            ProgressBar.Value = 0;
            TxtProgress.Text = "";
            TxtStatus.Text = "⏳ Đang bắt đầu...";

            var progress = new Progress<string>(msg =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    TxtProgress.Text = msg;
                    if (ProgressBar.Value < 90) ProgressBar.Value += 10;
                });
            });

            try
            {
                await OfficeInstallService.RunAsync(progress);
                TxtStatus.Text = "✅ Hoàn tất!";
                ProgressBar.Value = 100;
                LogService.Add("Office: Cài đặt từ installoffice.org thành công");
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
            finally
            {
                BtnInstall.IsEnabled = true;
            }
        }

        private async Task<bool> ShowCopyrightWarningAsync()
        {
            var stack = new StackPanel();

            // Tiêu đề cảnh báo (không dùng hình ảnh)
            stack.Children.Add(new TextBlock
            {
                Text = "⚠️ COPYRIGHT WARNING / CẢNH BÁO BẢN QUYỀN ⚠️",
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.OrangeRed),
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            });

            // Nội dung tiếng Anh
            stack.Children.Add(new TextBlock
            {
                Text = "This software is for educational and research purposes only.\n" +
                       "Any unauthorized reproduction or distribution of copyrighted material is illegal.\n" +
                       "Please ensure you have a valid license for Microsoft Office.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10),
                Foreground = new SolidColorBrush(Colors.LightGray)
            });

            // Nội dung tiếng Việt
            stack.Children.Add(new TextBlock
            {
                Text = "Phần mềm này chỉ dành cho mục đích học tập và nghiên cứu.\n" +
                       "Mọi hành vi sao chép hoặc phân phối trái phép tài liệu có bản quyền là bất hợp pháp.\n" +
                       "Vui lòng đảm bảo bạn có giấy phép hợp lệ cho Microsoft Office.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 15),
                Foreground = new SolidColorBrush(Colors.LightGray)
            });

            // Dòng đếm ngược
            var countdownText = new TextBlock
            {
                Text = "Continue button will be enabled in 5 seconds...\nNút tiếp tục sẽ được kích hoạt sau 5 giây...",
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.Yellow),
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 8, 0, 0)
            };
            stack.Children.Add(countdownText);

            var dialog = new ContentDialog
            {
                Title = "⚠️ Copyright Notice / Thông báo bản quyền",
                Content = stack,
                PrimaryButtonText = "Continue / Tiếp tục",
                SecondaryButtonText = "Cancel / Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
                IsPrimaryButtonEnabled = false   // Vô hiệu hóa nút Continue lúc đầu
            };

            int remaining = 5;
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, _) =>
            {
                remaining--;
                if (remaining > 0)
                {
                    countdownText.Text = $"Continue button will be enabled in {remaining} seconds...\nNút tiếp tục sẽ được kích hoạt sau {remaining} giây...";
                }
                else
                {
                    timer.Stop();
                    dialog.IsPrimaryButtonEnabled = true;
                    countdownText.Text = "✓ You may now continue.\n✓ Bạn có thể tiếp tục.";
                }
            };
            timer.Start();

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}