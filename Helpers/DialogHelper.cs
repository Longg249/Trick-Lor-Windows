using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace TrickLor.Helpers
{
    public static class DialogHelper
    {
        /// <summary>Hiện hộp thoại xác nhận Yes/No. Trả về true nếu chọn Yes.</summary>
        public static async Task<bool> ConfirmAsync(string title, string message, XamlRoot xamlRoot)
        {
            var dialog = new ContentDialog
            {
                Title             = title,
                Content           = message,
                PrimaryButtonText = "Có",
                CloseButtonText   = "Không",
                XamlRoot          = xamlRoot
            };
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        /// <summary>Mở hộp thoại lưu file JSON. Trả về đường dẫn hoặc null.</summary>
        public static async Task<string?> SaveJsonAsync(string suggestedName)
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName      = suggestedName
            };
            picker.FileTypeChoices.Add("JSON Files", new[] { ".json" });
            InitPicker(picker);
            StorageFile? file = await picker.PickSaveFileAsync();
            return file?.Path;
        }

        /// <summary>Mở hộp thoại lưu file TXT. Trả về đường dẫn hoặc null.</summary>
        public static async Task<string?> SaveTxtAsync(string suggestedName)
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName      = suggestedName
            };
            picker.FileTypeChoices.Add("Text Files", new[] { ".txt" });
            InitPicker(picker);
            StorageFile? file = await picker.PickSaveFileAsync();
            return file?.Path;
        }

        /// <summary>Mở hộp thoại chọn file JSON. Trả về đường dẫn hoặc null.</summary>
        public static async Task<string?> OpenJsonAsync()
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".json");
            InitPicker(picker);
            StorageFile? file = await picker.PickSingleFileAsync();
            return file?.Path;
        }

        private static void InitPicker(object picker)
        {
            var hwnd = WindowNative.GetWindowHandle(App.MainWindow!);
            InitializeWithWindow.Initialize(picker, hwnd);
        }
    }
}
