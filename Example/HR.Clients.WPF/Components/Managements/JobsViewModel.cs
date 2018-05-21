using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.WPF;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Managements
{
    public class JobsViewModel : ManagementViewModelBase<JobsView,IJobEditor,IJobRepository,Job,int>
    {
        protected override bool Filter(Job item, string term)
        {
            return item.Title.ToLower().Contains(term.ToLower());
        }

        public override string Title
        {
            get { return "Jobs"; }
        }

        protected override string GetItemDescription(Job item)
        {
            return item.Title;
        }
    }
}