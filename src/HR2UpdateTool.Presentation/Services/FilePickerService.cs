using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using WinRT.Interop;

namespace HR2UpdateTool.Presentation.Services
{
    public sealed class FilePickerService : IFilePickerService
    {
        public async Task<StorageFile?> PickFileAsync(Window window, IEnumerable<string> extensions)
        {
            ArgumentNullException.ThrowIfNull(window);
            ArgumentNullException.ThrowIfNull(extensions);

            var picker = new FileOpenPicker();

            foreach (var exteion in extensions)
            {
                if (string.IsNullOrWhiteSpace(exteion))
                    continue;

                var normalizedExtension = exteion.StartsWith('.')
                    ? exteion
                    : $".{exteion}";

                picker.FileTypeFilter.Add(normalizedExtension);
            }

            if (picker.FileTypeFilter.Count == 0)
            {
                throw new ArgumentException("하나 이상의 유효한 파일 확장자가 필요합니다.", nameof(extensions));
            }

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);

            return await picker.PickSingleFileAsync();
        }
    }
}
