// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Services;

namespace Coddee.WPF.DebugTool
{
    public class DebugToolViewModel : ViewModelBase<DebugToolView>, IDebugTool
    {
        /// <summary>
        /// The condition to toggle the tool window
        /// </summary>
        private Func<KeyEventArgs, bool> _toggleCondition;
        private bool _windowVisible;
        
        protected override Task OnInitialization()
        {
            var shellWindow = (Window) Resolve<IShell>();
            shellWindow.KeyDown += (sender, args) =>
            {
                if (_toggleCondition(args))
                    ToggleWindow();
            };
            return base.OnInitialization();
        }
        

        private void ToggleWindow()
        {
            _windowVisible = !_windowVisible;
            if (_windowVisible)
            {
                CreateView();
                View.Show();
            }
            else
                View.Close();
        }

        public void SetToggleCondition(Func<KeyEventArgs, bool> toggleCondition)
        {
            _toggleCondition = toggleCondition;
        }
    }
}