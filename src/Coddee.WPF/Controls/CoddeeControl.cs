// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Coddee.WPF.Controls
{
    public class CoddeeControl : Control,INotifyPropertyChanged
    {
        static CoddeeControl()
        {
            if (WPFApplication.Current != null)
                _container = WPFApplication.Current.GetContainer();
        }

        protected static readonly IContainer _container;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
