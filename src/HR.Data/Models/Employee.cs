// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee;

namespace HR.Data.Models
{
    public class Employee:IUniqueObject<Guid>
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }

        public string StateName { get; set; }

        public string FullName => $"{LastName}, {FirstName}";

        public Guid GetKey => ID;

        public override string ToString()
        {
            return FullName;
        }
    }
}