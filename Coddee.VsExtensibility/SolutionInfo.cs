// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.CodeTools;
using EnvDTE;
using EnvDTE80;

namespace Coddee.VsExtensibility
{
    public class SolutionInfo
    {
        protected readonly DTE2 _dte;
        protected readonly VsHelper _vsHelper;
        protected Solution2 _solution;

        public SolutionInfo(VsHelper vsHelper)
        {
            _dte = vsHelper.Dte;
            _vsHelper = vsHelper;
        }
        
        public string Name { get; set; }
        public string FullName { get; set; }

        public void UpdateCurrentSolution()
        {
            _solution = (Solution2)_dte.Solution;
        }

        public Project GetProjectByFullName(string fullName)
        {
            foreach (Project project in _dte.Solution.Projects)
            {
                if (project.FullName == fullName)
                {
                    return project;
                }
            }
            return null;
        }

        public void AddExistedFileToProject(string projectProjectPath, string file)
        {
            _vsHelper.AddExistedFileToProject(projectProjectPath,file);
        }
    }
}
