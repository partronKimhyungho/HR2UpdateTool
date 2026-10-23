using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HR2UpdateTool.Presentation.Services
{
    public interface IFilePickerService
    {
        Task<StorageFile?> PickFileAsync(Window window, IEnumerable<string> extensions);
    }
}
