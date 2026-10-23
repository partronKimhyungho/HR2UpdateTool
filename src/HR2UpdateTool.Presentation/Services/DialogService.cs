using HR2UpdateTool.Presentation.Views.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Presentation.Services
{
    public class DialogService : IDialogService
    {

        public DialogService()
        {
        }

        private XamlRoot GetXamlRoot()
        {
            var mainWindow = App.MainWindow;

            return mainWindow?.Content?.XamlRoot
                ?? throw new InvalidOperationException("XamlRoot is not ready.");
        }

        public async Task ShowAlertAsync(string message)
        {
            var dialog = new AlertDialog(GetXamlRoot(), message);
            await dialog.ShowAsync();
        }

        public async Task ShowContentAsync(string title, string content, string closeButtonText)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = GetXamlRoot(),
                Title = title,
                Content = content,
                CloseButtonText = closeButtonText
            };

            await dialog.ShowAsync();
        }
    }
}
