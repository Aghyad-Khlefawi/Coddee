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
        string GetActiveConfiguration();
        void AddExistedFileToProject(string projectPath, string fileName);
    }

    public class VsHelper : ISolutionEventsHelper, ISolutionHelper
    {
        public Func<Type, object> GetService { get; set; }
        public DTE2 Dte { get; private set; }


        public event Action SolutionOpened;
        public event Action SolutionClosed;
        private Solution2 _solution;
        private SolutionEvents _solutionEvents;
        public void Initialize()
        {
            Dte = (DTE2)GetService(typeof(DTE));
            _solution = (Solution2)Dte.Solution;
            _solutionEvents = Dte.Events.SolutionEvents;
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
            return Path.GetFileName(Dte.Solution.FileName).Replace(".sln", "");
        }

        public bool IsSolutionLoaded()
        {
            return Dte.Solution.IsOpen;
        }

        public void AddExistedFileToProject(string projectPath, string fileName)
        {
            foreach (Project project in Dte.Solution.Projects)
            {
                if (project.FullName == projectPath)
                {
                    project.ProjectItems.AddFromFile(fileName);
                    break;
                }
            }
        }
        public string GetCurrentSolutionPath()
        {
            return new FileInfo(Dte.Solution.FileName).Directory.FullName;
        }

        public string GetActiveConfiguration()
        {
            return Dte.Solution.Projects.Item(1).ConfigurationManager.ActiveConfiguration.ConfigurationName;
        }
    }
}
