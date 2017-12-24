using System;
using System.Threading;
using System.Windows.Input;
using Coddee.Loggers;
using Coddee.ModuleDefinitions;
using Coddee.Services;
using Coddee.Unity;
using Coddee.Windows.AppBuilder;
using Coddee.WPF;

namespace Coddee.CodeTools
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for CodeToolsMainViewControl.
    /// </summary>
    public partial class CodeToolsMainViewControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeToolsMainViewControl"/> class.
        /// </summary>
        public CodeToolsMainViewControl()
        {
            this.InitializeComponent();
        }
        
    }
}