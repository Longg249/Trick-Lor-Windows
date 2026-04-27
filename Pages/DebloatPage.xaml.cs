using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinDeployPro.Services;

namespace WinDeployPro.Pages
{
    public partial class DebloatPage : Page
    {
        public DebloatPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TxtStatus.Text = "Sẵn sàng — hãy tạo điểm khôi phục trước khi debloat";
        }

        // ── Presets ──────────────────────────────────────────────────────────

        private void PresetBasic_Click(object sender, RoutedEventArgs e)
        {
            SetAll(false);
            ChkXboxGamingOverlay.IsChecked = true;
            ChkXboxGameOverlay.IsChecked   = true;
            ChkSolitaire.IsChecked         = true;
            ChkGetStarted.IsChecked        = true;
            ChkSkype.IsChecked             = true;
            ChkTeamsChat.IsChecked         = true;
            ChkCortana.IsChecked           = true;
            ChkFeedbackHub.IsChecked       = true;
            ChkWidgets.IsChecked           = true;
            ChkBingSearch.IsChecked        = true;
            ChkCandyCrush.IsChecked        = true;
            ChkSpotify.IsChecked           = true;
            ChkAmazon.IsChecked            = true;
            ChkFacebook.IsChecked          = true;
            ChkTikTok.IsChecked            = true;
            ChkLinkedIn.IsChecked          = true;
        }

        private void PresetAll_Click(object sender, RoutedEventArgs e) => SetAll(true);
        private void PresetNone_Click(object sender, RoutedEventArgs e) => SetAll(false);

        private void SetAll(bool value)
        {
            foreach (var chk in AllCheckboxes())
                chk.IsChecked = value;
        }

        private IEnumerable<CheckBox> AllCheckboxes() => new[]
        {
            ChkXboxApp, ChkXboxGamingOverlay, ChkXboxGameOverlay, ChkXboxIdentity, ChkXboxTCUI,
            ChkSolitaire, Chk3DViewer, ChkMixedReality, ChkZuneMusic, ChkZuneVideo, ChkGetStarted,
            ChkSkype, ChkPeople, ChkMailCalendar, ChkYourPhone, ChkTeamsChat,
            ChkOneDrive, ChkCortana, ChkOfficeHub, ChkFeedbackHub, ChkOneNote,
            ChkWidgets, ChkBingSearch, ChkMaps, ChkBingNews, ChkBingWeather,
            ChkCandyCrush, ChkSpotify, ChkAmazon, ChkFacebook, ChkTikTok, ChkLinkedIn
        };

        // ── System Restore ───────────────────────────────────────────────────

        private async void CreateRestore_Click(object sender, RoutedEventArgs e)
        {
            BtnRestorePoint.IsEnabled = false;
            BtnApply.IsEnabled = false;
            var progress = new Progress<string>(msg =>
            {
                TxtProgress.Text = msg;
                TxtStatus.Text = msg;
            });
            var result = await DebloatService.CreateRestorePointAsync(progress);
            TxtStatus.Text = result;
            TxtProgress.Text = result;
            BtnRestorePoint.IsEnabled = true;
            BtnApply.IsEnabled = true;
            LogService.Add($"Debloat: {result}");
        }

        private void OpenRestore_Click(object sender, RoutedEventArgs e)
        {
            DebloatService.OpenSystemRestoreUI();
        }

        // ── Apply Debloat ─────────────────────────────────────────────────────

        private async void ApplyDebloat_Click(object sender, RoutedEventArgs e)
        {
            BtnApply.IsEnabled = false;
            BtnRestorePoint.IsEnabled = false;

            var progress = new Progress<string>(msg =>
            {
                TxtProgress.Text = msg;
                TxtStatus.Text = msg;
            });

            try
            {
                var tasks = new List<Task>();

                // Xbox
                if (ChkXboxApp.IsChecked == true)            tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.XboxApp", progress));
                if (ChkXboxGamingOverlay.IsChecked == true)  tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.XboxGamingOverlay", progress));
                if (ChkXboxGameOverlay.IsChecked == true)    tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.XboxGameOverlay", progress));
                if (ChkXboxIdentity.IsChecked == true)       tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.XboxIdentityProvider", progress));
                if (ChkXboxTCUI.IsChecked == true)           tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.Xbox.TCUI", progress));

                // Entertainment
                if (ChkSolitaire.IsChecked == true)          tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.MicrosoftSolitaireCollection", progress));
                if (Chk3DViewer.IsChecked == true)           tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.Microsoft3DViewer", progress));
                if (ChkMixedReality.IsChecked == true)       tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.MixedReality.Portal", progress));
                if (ChkZuneMusic.IsChecked == true)          tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.ZuneMusic", progress));
                if (ChkZuneVideo.IsChecked == true)          tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.ZuneVideo", progress));
                if (ChkGetStarted.IsChecked == true)         tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.Getstarted", progress));

                // Communication
                if (ChkSkype.IsChecked == true)              tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.SkypeApp", progress));
                if (ChkPeople.IsChecked == true)             tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.People", progress));
                if (ChkMailCalendar.IsChecked == true)       tasks.Add(DebloatService.RemoveAppxAsync("microsoft.windowscommunicationsapps", progress));
                if (ChkYourPhone.IsChecked == true)          tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.YourPhone", progress));
                if (ChkTeamsChat.IsChecked == true)          tasks.Add(DebloatService.DisableChatWidgetAsync(progress));

                // Productivity
                if (ChkOneDrive.IsChecked == true)           tasks.Add(DebloatService.RemoveOneDriveAsync(progress));
                if (ChkCortana.IsChecked == true)            tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.549981C3F5F10", progress));
                if (ChkOfficeHub.IsChecked == true)          tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.MicrosoftOfficeHub", progress));
                if (ChkFeedbackHub.IsChecked == true)        tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.WindowsFeedbackHub", progress));
                if (ChkOneNote.IsChecked == true)            tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.Office.OneNote", progress));

                // Windows Features
                if (ChkWidgets.IsChecked == true)            tasks.Add(DebloatService.DisableWidgetsAsync(progress));
                if (ChkBingSearch.IsChecked == true)         tasks.Add(DebloatService.DisableBingSearchAsync(progress));
                if (ChkMaps.IsChecked == true)               tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.WindowsMaps", progress));
                if (ChkBingNews.IsChecked == true)           tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.BingNews", progress));
                if (ChkBingWeather.IsChecked == true)        tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.BingWeather", progress));

                // OEM
                if (ChkCandyCrush.IsChecked == true)         tasks.Add(DebloatService.RemoveAppxAsync("king.com.Candy*", progress));
                if (ChkSpotify.IsChecked == true)            tasks.Add(DebloatService.RemoveAppxAsync("SpotifyAB.SpotifyMusic", progress));
                if (ChkAmazon.IsChecked == true)             tasks.Add(DebloatService.RemoveAppxAsync("Amazon.com.Amazon", progress));
                if (ChkFacebook.IsChecked == true)           tasks.Add(DebloatService.RemoveAppxAsync("Facebook.Facebook", progress));
                if (ChkTikTok.IsChecked == true)             tasks.Add(DebloatService.RemoveAppxAsync("*TikTok*", progress));
                if (ChkLinkedIn.IsChecked == true)           tasks.Add(DebloatService.RemoveAppxAsync("Microsoft.LinkedIn", progress));

                if (tasks.Count == 0)
                {
                    TxtStatus.Text = "⚠  Không có mục nào được chọn";
                    return;
                }

                await Task.WhenAll(tasks);

                var msg = $"✅ Hoàn tất — đã xử lý {tasks.Count} mục";
                TxtStatus.Text = msg;
                TxtProgress.Text = msg;
                LogService.Add($"Debloat: {msg}");
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
            finally
            {
                BtnApply.IsEnabled = true;
                BtnRestorePoint.IsEnabled = true;
            }
        }
    }
}
