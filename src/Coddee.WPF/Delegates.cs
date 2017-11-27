using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.WPF
{
    public delegate void ViewModelEventHandler(IViewModel sender);

    public delegate void ViewModelEventHandler<TArgs>(IViewModel sender, TArgs args);
    public delegate Task AsyncViewModelEventHandler<TArgs>(IViewModel sender, TArgs args);
}
