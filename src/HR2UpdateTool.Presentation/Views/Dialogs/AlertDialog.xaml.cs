using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HR2UpdateTool.Presentation.Views.Dialogs
{
    public sealed partial class AlertDialog : ContentDialog
    {
        public AlertDialog(XamlRoot xamlRoot, string message)
        {
            this.InitializeComponent();

            this.XamlRoot = xamlRoot;
            this.MessageTextBlock.Text = message;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
