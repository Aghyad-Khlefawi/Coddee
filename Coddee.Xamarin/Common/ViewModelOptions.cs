using System;
using System.Collections.Generic;
using System.Text;

namespace Coddee.Xamarin.Common
{
    public class ViewModelOptions
    {
        public static readonly ViewModelOptions Default = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = true,
            ShowErrors = true
        };
        public static readonly ViewModelOptions Editor = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = false,
            ShowErrors = true
        };
        public static readonly ViewModelOptions EditorNoErrors = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = false,
            ShowErrors = false
        };
        public bool IncludeInHierarchicalValidation { get; set; }
        public bool ShowErrors { get; set; }
    }
}
