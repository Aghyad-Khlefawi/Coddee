using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Coddee.Xamarin.Forms
{
    public partial class BusyIndicator
    {
        public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(BusyIndicator), default(bool));

        public static readonly BindableProperty DisplayedContentProperty = BindableProperty.Create(nameof(DisplayedContent), typeof(object), typeof(BusyIndicator), default(object));



        public object DisplayedContent
        {
            get { return GetValue(DisplayedContentProperty); }
            set { SetValue(DisplayedContentProperty, value); }
        }

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
    }
}
