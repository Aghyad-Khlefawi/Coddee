// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows.Controls;

namespace Coddee.WPF.Controls
{
    public class CoddeeControl : Control
    {
        static CoddeeControl()
        {
            if (WPFApplication.Current != null)
                _container = WPFApplication.Current.GetContainer();
        }

        protected static readonly IContainer _container;

    }
}
