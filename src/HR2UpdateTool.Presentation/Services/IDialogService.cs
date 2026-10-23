using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Presentation.Services
{
    public interface IDialogService
    {
        Task ShowContentAsync(string title, string content, string closeButtonText);
        Task ShowAlertAsync(string message);
    }
}
