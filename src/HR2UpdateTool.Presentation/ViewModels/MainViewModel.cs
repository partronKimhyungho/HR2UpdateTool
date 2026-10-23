using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HR2UpdateTool.Application.Common;
using HR2UpdateTool.Application.Interface;
using HR2UpdateTool.Application.UseCases;
using HR2UpdateTool.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Presentation.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly ISerialService _serialService;
        private readonly SerialPortWatcher _serialPortWatcher;

        private readonly FirmwareUpdateUseCase _firmwareUpdateUseCase;
        private readonly IFilePickerService _filePickerService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        public partial ObservableCollection<string> AvailablePorts { get; set; } = new();

        [ObservableProperty]
        public partial string? SelectedPort { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotConnected))]
        [NotifyPropertyChangedFor(nameof(ConnectButtonText))]
        public partial bool IsConnected { get; set; }
        public bool IsNotConnected => !IsConnected;

        public string ConnectButtonText => IsConnected ? "해제" : "연결";


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UpdateEnable))]
        [NotifyPropertyChangedFor(nameof(ClearButtonVisible))]
        public partial string? FilePath { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressGridLength))]
        public partial int UpdateProgress { get; set; }

        public string ProgressGridLength => $"{UpdateProgress}*,{100 - UpdateProgress}*";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UpdateEnable))]
        public partial bool IsLoading { get; set; }

        public bool UpdateEnable => !IsLoading && (FilePath != null);

        public bool ClearButtonVisible => FilePath != null;

        public MainViewModel(ISerialService serialService, FirmwareUpdateUseCase firmwareUpdateUseCase, IFilePickerService filePickerService, IDialogService dialogService, SerialPortWatcher serialPortWatcher)
        {
            _serialService = serialService;
            _firmwareUpdateUseCase = firmwareUpdateUseCase;
            _filePickerService = filePickerService;
            _dialogService = dialogService;
            _serialPortWatcher = serialPortWatcher;

            _serialPortWatcher.PortsChanged += OnPortsChanged;

            RefreshPorts();
        }

        [RelayCommand]
        private void RefreshPorts()
        {
            var latestPorts = _serialService.GetAvailablePorts().ToList();

            for (int i = AvailablePorts.Count - 1; i >= 0; i--)
            {
                var port = AvailablePorts[i];

                if (!latestPorts.Contains(port))
                {
                    if (SelectedPort == port)
                    {
                        Debug.WriteLine(port);
                        Debug.WriteLine(SelectedPort);
                        _serialService.Disconnect();
                        IsConnected = false;
                    }

                    AvailablePorts.RemoveAt(i);
                }
            }

            foreach (var port in latestPorts)
            {
                if (!AvailablePorts.Contains(port))
                {
                    AvailablePorts.Add(port);
                }
            }
        }

        [RelayCommand]
        private async Task ToggleConnectAsync()
        {
            if (IsConnected)
            {
                _serialService.Disconnect();
                IsConnected = false;
            }
            else
            {
                if (string.IsNullOrEmpty(SelectedPort)) return;

                try
                {
                    _serialService.Connect(SelectedPort, 115200);
                    IsConnected = _serialService.IsConnected;
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowContentAsync(title: "Error", content: ex.Message, "확인");
                }
            }
        }

        private void OnPortsChanged()
        {
            RunOnUIThread(() =>
            {
                RefreshPorts();
            });
        }

        [RelayCommand]
        private async Task SelectFileAsync()
        {
            var window = App.MainWindow;
            if (window == null) return;

            var file = await _filePickerService.PickFileAsync(window, new[] { ".bin", ".hex" });
            if (file != null)
            {
                FilePath = file.Path;
            }
        }

        [RelayCommand]
        private void ClearFilePath()
        {
            FilePath = null;
        }

        [RelayCommand]
        private async Task StartFirmwareUpdateAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath)) return;
            IsLoading = true;

            try
            {
                var progress = new Progress<int>(percent =>
                {
                    UpdateProgress = percent;
                });

                var param = new FirmwareUpdateUseCase.Params(FilePath, progress);

                switch (await _firmwareUpdateUseCase.ExecuteAsync(param))
                {
                    case Success<bool> s:

                        break;

                    case Error<bool> e:
                        await _dialogService.ShowContentAsync(title: "Error", content: e.ErrorMessage, "확인");
                        break;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(ex.Message);
            }
            finally
            {
                UpdateProgress = 0;
                IsLoading = false;
            }
        }
    }
}
