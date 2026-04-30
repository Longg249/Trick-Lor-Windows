using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using TrickLor.Models;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public sealed partial class DeployPage : Page
    {
        public DeployPage() => InitializeComponent();

        // ===== PHẦN DANH SÁCH CỨNG =====
        private void SelectAll_Click(object sender, RoutedEventArgs e) => SetAllChecks(true);
        private void DeselectAll_Click(object sender, RoutedEventArgs e) => SetAllChecks(false);

        private void SetAllChecks(bool value)
        {
            foreach (var chk in GetAllCheckBoxes())
                chk.IsChecked = value;
        }

        private IEnumerable<CheckBox> GetAllCheckBoxes()
        {
            yield return Chk7Zip; yield return ChkNotepad; yield return ChkPowerToys;
            yield return ChkKeePass; yield return ChkTreeSize; yield return ChkCCleaner;
            yield return ChkNaps2; yield return ChkFoxitReader; yield return ChkChrome;
            yield return ChkFirefox; yield return ChkBrave; yield return ChkVSCode;
            yield return ChkGit; yield return ChkPython; yield return ChkAnyDesk;
            yield return ChkTeamViewer; yield return ChkPuTTY; yield return ChkWPS;
            yield return ChkLibreOffice; yield return ChkOnlyOffice; yield return ChkAdobeReader;
            yield return ChkSumatraPDF; yield return ChkPDF24; yield return ChkZoom;
            yield return ChkTeams; yield return ChkSkype; yield return ChkSlack;
            yield return ChkTelegram; yield return ChkThunderbird; yield return ChkGoogleDrive;
            yield return ChkDropbox; yield return ChkOneDrive; yield return ChkVLC;
            yield return ChkShareX;
        }

        private async void InstallSelected_Click(object sender, RoutedEventArgs e)
        {
            var selected = new List<(string Name, string WingetId)>();

            if (Chk7Zip.IsChecked == true) selected.Add(("7-Zip", "7zip.7zip"));
            if (ChkNotepad.IsChecked == true) selected.Add(("Notepad++", "Notepad++.Notepad++"));
            if (ChkPowerToys.IsChecked == true) selected.Add(("PowerToys", "Microsoft.PowerToys"));
            if (ChkKeePass.IsChecked == true) selected.Add(("KeePass", "DominikReichl.KeePass"));
            if (ChkTreeSize.IsChecked == true) selected.Add(("TreeSize", "JAMSoftware.TreeSizeFree"));
            if (ChkCCleaner.IsChecked == true) selected.Add(("CCleaner", "Piriform.CCleaner"));
            if (ChkNaps2.IsChecked == true) selected.Add(("NAPS2", "NAPS2.NAPS2"));
            if (ChkFoxitReader.IsChecked == true) selected.Add(("Foxit Reader", "Foxit.FoxitReader"));
            if (ChkChrome.IsChecked == true) selected.Add(("Chrome", "Google.Chrome"));
            if (ChkFirefox.IsChecked == true) selected.Add(("Firefox", "Mozilla.Firefox"));
            if (ChkBrave.IsChecked == true) selected.Add(("Brave", "Brave.Brave"));
            if (ChkVSCode.IsChecked == true) selected.Add(("VS Code", "Microsoft.VisualStudioCode"));
            if (ChkGit.IsChecked == true) selected.Add(("Git", "Git.Git"));
            if (ChkPython.IsChecked == true) selected.Add(("Python 3", "Python.Python.3"));
            if (ChkAnyDesk.IsChecked == true) selected.Add(("AnyDesk", "AnyDeskSoftwareGmbH.AnyDesk"));
            if (ChkTeamViewer.IsChecked == true) selected.Add(("TeamViewer", "TeamViewer.TeamViewer"));
            if (ChkPuTTY.IsChecked == true) selected.Add(("PuTTY", "PuTTY.PuTTY"));
            if (ChkWPS.IsChecked == true) selected.Add(("WPS Office", "Kingsoft.WPSOffice"));
            if (ChkLibreOffice.IsChecked == true) selected.Add(("LibreOffice", "TheDocumentFoundation.LibreOffice"));
            if (ChkOnlyOffice.IsChecked == true) selected.Add(("OnlyOffice", "ONLYOFFICE.DesktopEditors"));
            if (ChkAdobeReader.IsChecked == true) selected.Add(("Adobe Reader", "Adobe.Acrobat.Reader.64-bit"));
            if (ChkSumatraPDF.IsChecked == true) selected.Add(("Sumatra PDF", "SumatraPDF.SumatraPDF"));
            if (ChkPDF24.IsChecked == true) selected.Add(("PDF24", "geeksoftwareGmbH.PDF24Creator"));
            if (ChkZoom.IsChecked == true) selected.Add(("Zoom", "Zoom.Zoom"));
            if (ChkTeams.IsChecked == true) selected.Add(("Microsoft Teams", "Microsoft.Teams"));
            if (ChkSkype.IsChecked == true) selected.Add(("Skype", "Microsoft.Skype"));
            if (ChkSlack.IsChecked == true) selected.Add(("Slack", "SlackTechnologies.Slack"));
            if (ChkTelegram.IsChecked == true) selected.Add(("Telegram", "Telegram.TelegramDesktop"));
            if (ChkThunderbird.IsChecked == true) selected.Add(("Thunderbird", "Mozilla.Thunderbird"));
            if (ChkGoogleDrive.IsChecked == true) selected.Add(("Google Drive", "Google.GoogleDrive"));
            if (ChkDropbox.IsChecked == true) selected.Add(("Dropbox", "Dropbox.Dropbox"));
            if (ChkOneDrive.IsChecked == true) selected.Add(("OneDrive", "Microsoft.OneDrive"));
            if (ChkVLC.IsChecked == true) selected.Add(("VLC", "VideoLAN.VLC"));
            if (ChkShareX.IsChecked == true) selected.Add(("ShareX", "ShareX.ShareX"));

            if (selected.Count == 0)
            {
                TxtStatus.Text = "⚠ Chưa chọn phần mềm nào!";
                return;
            }

            BtnInstall.IsEnabled = false;
            ProgressInstall.Value = 0;
            TxtCount.Text = $"0 / {selected.Count}";

            int done = 0;
            var progress = new Progress<string>(msg =>
            {
                TxtStatus.Text = msg;
                done++;
                ProgressInstall.Value = (double)done / selected.Count * 100;
                TxtCount.Text = $"{done} / {selected.Count}";
            });

            await InstallerService.InstallListAsync(selected, progress);

            TxtStatus.Text = "✅ Hoàn tất cài đặt!";
            ProgressInstall.Value = 100;
            BtnInstall.IsEnabled = true;

            LogService.Add($"Deploy hoàn tất: {selected.Count} phần mềm");
        }

        // ===== PHẦN TÌM KIẾM WINGET =====
        private List<WingetPackage> _wingetResults = new();

        private async void WingetSearch_Click(object sender, RoutedEventArgs e)
        {
            await PerformWingetSearch();
        }

        private async void WingetSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                await PerformWingetSearch();
        }

        private async System.Threading.Tasks.Task PerformWingetSearch()
        {
            var query = TxtWingetSearch.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                TxtWingetStatus.Text = "⚠ Vui lòng nhập từ khóa.";
                return;
            }

            BtnWingetSearch.IsEnabled = false;
            TxtWingetStatus.Text = "⏳ Đang tìm kiếm...";
            WingetResultsList.ItemsSource = null;
            BtnInstallSelected.IsEnabled = false;

            try
            {
                _wingetResults = await WingetService.SearchAsync(query);
                WingetResultsList.ItemsSource = _wingetResults;
                TxtWingetStatus.Text = $"🔍 Tìm thấy {_wingetResults.Count} gói.";
                BtnInstallSelected.IsEnabled = _wingetResults.Count > 0;
            }
            catch (Exception ex)
            {
                TxtWingetStatus.Text = $"❌ Lỗi: {ex.Message}";
                _wingetResults.Clear();
                WingetResultsList.ItemsSource = null;
            }
            finally
            {
                BtnWingetSearch.IsEnabled = true;
            }
        }

        private async void InstallSelectedWinget_Click(object sender, RoutedEventArgs e)
        {
            var selected = _wingetResults.Where(p => p.IsSelected).ToList();
            if (selected.Count == 0)
            {
                TxtWingetStatus.Text = "⚠ Chưa chọn gói nào.";
                return;
            }

            BtnInstallSelected.IsEnabled = false;
            var packages = selected.Select(p => (p.Name, p.Id)).ToList();
            var progress = new Progress<string>(msg => TxtWingetStatus.Text = msg);

            await InstallerService.InstallListAsync(packages, progress);
            TxtWingetStatus.Text = "✅ Cài đặt hoàn tất!";
            BtnInstallSelected.IsEnabled = true;
            LogService.Add($"Winget: cài {selected.Count} gói");
        }
    }
}