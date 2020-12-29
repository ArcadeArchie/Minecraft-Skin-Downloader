using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCSkinDownloader.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        protected ObservableAsPropertyHelper<bool> isBusy;
        public bool IsBusy { get { return isBusy.Value; } }
    }
}
