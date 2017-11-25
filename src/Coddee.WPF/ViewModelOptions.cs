using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.WPF
{
    public class ViewModelOptions
    {
        public static readonly ViewModelOptions Default = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = true
        };
        public static readonly ViewModelOptions Editor  = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = false
        };

        public bool IncludeInHierarchicalValidation { get; set; }
    }
}
