// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Coddee.CodeTools
{
    public interface ISolutionEventsHelper
    {
        event Action SolutionOpened;
        event Action SolutionLoaded;
        event Action SolutionClosed;
    }

    public interface ISolutionHelper
    {
        string GetCurrentSolutionName();
        bool IsSolutionLoaded();
        string GetCurrentSolutionPath();
    }

    public class VsHelper : ISolutionEventsHelper, ISolutionHelper, IVsSolutionLoadEvents, IVsSolutionEvents
    {
        public Func<Type, object> GetService { get; set; }


        public event Action SolutionOpened;
        public event Action SolutionLoaded;
        public event Action SolutionClosed;
        private IVsSolution _solution;
        private DTE _dte;
        public void Initialize()
        {
            _solution = (IVsSolution)GetService(typeof(SVsSolution));
            _dte = (DTE)GetService(typeof(DTE));
            _solution.AdviseSolutionEvents(this, out uint pdeCookie);
        }


        public int OnBeforeOpenSolution(string pszSolutionFilename)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeBackgroundSolutionLoadBegins()
        {
            return VSConstants.S_OK;
        }

        public int OnQueryBackgroundLoadProjectBatch(out bool pfShouldDelayLoadToNextIdle)
        {
            pfShouldDelayLoadToNextIdle = false;
            return VSConstants.S_OK;
        }

        public int OnBeforeLoadProjectBatch(bool fIsBackgroundIdleBatch)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProjectBatch(bool fIsBackgroundIdleBatch)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterBackgroundSolutionLoadComplete()
        {
            SolutionLoaded?.Invoke();
            return VSConstants.S_OK;
        }

        public int OnDisconnect()
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeOpenProject(ref Guid guidProjectID, ref Guid guidProjectType, string pszFileName, IVsSolutionLoadManagerSupport pSLMgrSupport)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            SolutionOpened?.Invoke();
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            SolutionClosed?.Invoke();
            return VSConstants.S_OK;
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
