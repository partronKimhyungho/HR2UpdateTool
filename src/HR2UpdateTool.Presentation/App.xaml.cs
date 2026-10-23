using HR2UpdateTool.Presentation.Services;
using HR2UpdateTool.Presentation.ViewModels;
using HR2UpdateTool.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;


using AppLayer = global::HR2UpdateTool.Application;
using InfraLayer = global::HR2UpdateTool.Infrastructure;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HR2UpdateTool.Presentation
{
    public partial class App : global::Microsoft.UI.Xaml.Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public static Window? MainWindow { get; private set; }

        public App()
        {
            InitializeComponent();
            Services = ConfigureServices();

            this.UnhandledException += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Unhandled Exception] {e.Message}");

                System.Diagnostics.Debug.WriteLine(
                    e.Exception.ToString());
            };
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            AppLayer.DependencyInjection.AddApplicationServices(services);
            InfraLayer.DependencyInjection.AddInfrastructureServices(services);

            services.AddSingleton<IFilePickerService, FilePickerService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<SerialPortWatcher>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow?.Activate();
        }
    }
}
