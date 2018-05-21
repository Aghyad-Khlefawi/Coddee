using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Validation;
using Coddee.WPF;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Managements
{
    public class JobEditorViewModel : EditorViewModelBase<JobEditorViewModel, JobEditorView, IJobRepository, Job, int>, IJobEditor
    {
        public JobEditorViewModel()
        {
            Title = "Job";
        }

        private string _jobTitle;
        [EditorField]
        public string JobTitle
        {
            get { return _jobTitle; }
            set { SetProperty(ref _jobTitle, value); }
        }

        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
            validationRules.Add(ValidationRule.CreateErrorRule(() => JobTitle));
        }

        protected override async Task MapEditedItemToEditor(Job item)
        {
            await base.MapEditedItemToEditor(item);
            this.JobTitle = item.Title;
        }

        protected override async Task MapEditorToEditedItem(Job item)
        {
            await base.MapEditorToEditedItem(item);
            item.Title = JobTitle;
        }
    }
}