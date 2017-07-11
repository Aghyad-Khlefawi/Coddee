using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Coddee.WPF;
using Coddee.WPF.Dialogs;

namespace Coddee.WPF.Modules.Dialogs
{
    public class ContentDialogViewModel : DialogViewModelBase<ContentDialogView>
    {
        private UIElement _content;
        public UIElement Content
        {
            get { return _content; }
            set { SetProperty(ref this._content, value); }
        }
    }
}