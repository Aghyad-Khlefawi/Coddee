﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee;
using Coddee.WPF;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Companies.Editors
{
    public class EmployeeEditorViewModel : EditorViewModel<EmployeeEditorViewModel,EmployeeEditorView, IEmployeeRepository, Employee, Guid>
    {
        private Company _selectedCompany;

        public void Add(Company companiesSelectedItem)
        {
            _selectedCompany = companiesSelectedItem;
            base.Add();
        }

        public void Edit(Employee item, Company companiesSelectedItem)
        {
            _selectedCompany = companiesSelectedItem;
            base.Edit(item);
        }

        public override void PreSave()
        {
            EditedItem.CompanyID = _selectedCompany.ID;
            EditedItem.CompanyName = _selectedCompany.Name;
            EditedItem.StateName = _selectedCompany.StateName;
            base.PreSave();
        }

        public EmployeeEditorViewModel(IObjectMapper mapper, IEmployeeRepository repository) : base(mapper, repository)
        {
        }
    }
}