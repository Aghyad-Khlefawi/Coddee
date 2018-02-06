// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.Data;
using Coddee.Validation;
using Coddee.WPF;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Companies.Editors
{


    public class CompanyEditorViewModel : EditorViewModelBase<CompanyEditorViewModel, CompanyEditorView, ICompanyRepository, Company, Guid>
    {
        private AsyncObservableCollection<State> _states;
        public AsyncObservableCollection<State> States
        {
            get { return _states; }
            set { SetProperty(ref this._states, value); }
        }

        private string _name;
        [EditorField(IsRequired = true)]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }
        
        public override void PreSave()
        {
            EditedItem.StateID = States.SelectedItem.ID;
            EditedItem.StateName = States.SelectedItem.Name;
            base.PreSave();
        }

        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
          //  validationRules.Add(ValidationRule.CreateErrorRule(() => Name,
          //                                 () => "Name field is required"));
        }

        protected override async Task<Company> SaveItem()
        {
            try
            {
                return await base.SaveItem();
            }
            catch (DBException ex)
            {
                LogError(ex);
                return null;
            }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            States = AsyncObservableCollection<State>.Create(await Resolve<IStateRepository>().GetItems());
        }

    }
}