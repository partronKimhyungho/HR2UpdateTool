using HR2UpdateTool.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HR2UpdateTool.Presentation
{
    public sealed partial class MainWindow : Window
    {

        private bool _isInitialized = false;
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = App.Services.GetRequiredService<MainViewModel>();
        }

        private void RootGrid_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (_isInitialized) return;

            this.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                try
                {
                    IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                    if (hWnd == IntPtr.Zero) return;

                    Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                    Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                    if (appWindow == null) return;

                    var scalingFactor = RootGrid.XamlRoot.RasterizationScale;
                    if (scalingFactor <= 0) scalingFactor = 1.0;

                    double targetWidthDips = 600.0;
                    int targetPhysicalWidth = (int)(targetWidthDips * scalingFactor);

                    RootGrid.UpdateLayout();
                    RootGrid.Measure(new Windows.Foundation.Size(targetWidthDips, double.PositiveInfinity));
                    double desiredHeight = RootGrid.DesiredSize.Height;

                    if (desiredHeight > 0)
                    {
                        double finalHeightDips = desiredHeight;
                        int targetPhysicalHeight = (int)(finalHeightDips * scalingFactor);

                        appWindow.ResizeClient(new Windows.Graphics.SizeInt32(targetPhysicalWidth, targetPhysicalHeight));

                        double nonClientHeightDips = (appWindow.Size.Height - appWindow.ClientSize.Height) / scalingFactor;

                        double managerTotalHeightDips = finalHeightDips + nonClientHeightDips;

                        var manager = WindowManager.Get(this);
                        if (manager != null)
                        {
                            manager.MinWidth = (int)targetWidthDips;
                            manager.MaxWidth = (int)targetWidthDips;
                            manager.MinHeight = (int)managerTotalHeightDips;
                            manager.MaxHeight = (int)managerTotalHeightDips;
                        }

                        _isInitialized = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Window Resize Error] {ex.Message}");
                }
            });
        }
    }
}
