using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Presentation.ViewModels
{
    public abstract class BaseViewModel : ObservableObject
    {
        protected readonly DispatcherQueue _dispatcherQueue;

        protected BaseViewModel()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }


        protected void RunOnUIThread(Action action)
        {
            if (_dispatcherQueue == null)
            {
                action();
                return;
            }

            if (_dispatcherQueue.HasThreadAccess)
            {
                action();
            }
            else
            {
                _dispatcherQueue.TryEnqueue(() => action());
            }
        }
    }
}
