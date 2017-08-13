// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee;

namespace HR.Data.Models
{
    public class Employee:IUniqueObject<Guid>,IEquatable<Employee>
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

        public bool Equals(Employee other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID.Equals(other.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Employee) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}