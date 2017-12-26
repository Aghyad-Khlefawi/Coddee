// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Coddee.CodeTools
{
    public interface ISolutionEventsHelper
    {
        event Action SolutionOpened;
        event Action SolutionClosed;
    }

    public interface ISolutionHelper
    {
        string GetCurrentSolutionName();
        bool IsSolutionLoaded();
        string GetCurrentSolutionPath();
    }

    public class VsHelper : ISolutionEventsHelper, ISolutionHelper
    {
        public Func<Type, object> GetService { get; set; }


        public event Action SolutionOpened;
        public event Action SolutionClosed;
        private DTE2 _dte;
        private SolutionEvents _solutionEvents;
        public void Initialize()
        {
            _dte = (DTE2)GetService(typeof(DTE));
            _solutionEvents=_dte.Events.SolutionEvents;
            _solutionEvents.Opened += () =>
            {
                SolutionOpened?.Invoke();
            };
            _solutionEvents.AfterClosing += () =>
            {
                SolutionClosed?.Invoke();
            };
        }



        public string GetCurrentSolutionName()
        {
            return Path.GetFileName(_dte.Solution.FileName).Replace(".sln", "");
        }

        public bool IsSolutionLoaded()
        {
            return _dte.Solution.IsOpen;
        }

        public string GetCurrentSolutionPath()
        {
            return new FileInfo(_dte.Solution.FileName).Directory.FullName;
        }
        
    }
}
