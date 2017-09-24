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
            Resolutions = AsyncObservableCollection<Resolution>.Create();
            Resolutions.Add(new Resolution { Height = double.NaN, Width = double.NaN, Title = "Full screen" });
            Resolutions.Add(new Resolution { Height = 1080, Width = 1920});
            Resolutions.Add(new Resolution { Height = 992, Width = 1768});
            Resolutions.Add(new Resolution { Height = 1050, Width = 1680});
            Resolutions.Add(new Resolution { Height = 1024, Width = 1600});
            Resolutions.Add(new Resolution { Height = 900, Width = 1600});
            Resolutions.Add(new Resolution { Height = 900, Width = 1440});
            Resolutions.Add(new Resolution { Height = 768, Width = 1366});
            Resolutions.Add(new Resolution { Height = 768, Width = 1360});
            Resolutions.Add(new Resolution { Height = 1024, Width = 1280});

            foreach (var resolution in Resolutions.Skip(1))
            {
                resolution.Title = $"{resolution.Width}x{resolution.Height}";
            }
            Resolutions.SelectedItem = Resolutions.First();

        }
    }
}