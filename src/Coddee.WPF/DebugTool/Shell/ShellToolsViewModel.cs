// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Collections;

namespace Coddee.WPF.DebugTool.Shell
{
    public class ShellToolsViewModel : ViewModelBase<ShellToolsView>
    {
        public ShellToolsViewModel()
        {
            SetResolutionCommand = CreateReactiveCommand(SetResolution);
        }

        private AsyncObservableCollection<Resolution> _resolutions;
        public AsyncObservableCollection<Resolution> Resolutions
        {
            get { return _resolutions; }
            set { SetProperty(ref this._resolutions, value); }
        }
        public ICommand SetResolutionCommand { get; }

        private void SetResolution()
        {
            var window = Resolve<IShell>() as Window;

            if (double.IsNaN(Resolutions.SelectedItem.Height))
                window.WindowState = WindowState.Maximized;
            else
            {
                window.WindowState = WindowState.Normal;
                window.Height = Resolutions.SelectedItem.Height;
                window.Width = Resolutions.SelectedItem.Width;
            }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Resolutions = AsyncObservableCollection<Resolution>.Create(Resolution.CommonResolutions);
            Resolutions.SelectedItem = Resolutions.First();
        }
    }
}