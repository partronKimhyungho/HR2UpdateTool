using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace HR2UpdateTool.Presentation.Services
{
    public class SerialPortWatcher : IDisposable
    {
        private DeviceWatcher? _deviceWatcher;
        private bool _isDisposed;

        public event Action? PortsChanged;

        public SerialPortWatcher()
        {
            StartWatching();
        }

        private void StartWatching()
        {
            if (_deviceWatcher != null) return;

            string selector = SerialDevice.GetDeviceSelector();
            _deviceWatcher = DeviceInformation.CreateWatcher(selector);

            _deviceWatcher.Added += OnDeviceChanged;
            _deviceWatcher.Removed += OnDeviceChanged;
            _deviceWatcher.Updated += OnDeviceChanged;

            _deviceWatcher.Start();
        }

        private void OnDeviceChanged(DeviceWatcher sender, object args)
        {
            PortsChanged?.Invoke();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            if (_deviceWatcher != null)
            {
                _deviceWatcher.Added -= OnDeviceChanged;
                _deviceWatcher.Removed -= OnDeviceChanged;
                _deviceWatcher.Updated -= OnDeviceChanged;
                _deviceWatcher.Stop();
                _deviceWatcher = null;
            }

            _isDisposed = true;
        }
    }
}
