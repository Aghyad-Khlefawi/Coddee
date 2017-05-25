// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
   public class ValueChangedEventArgs:EventArgs
    {
        public string Key { get; set; }
        public object NewValue { get; set; }
        public object OldValue { get; set; }

    }
}
