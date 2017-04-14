// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee;

namespace HR.Data.Models
{
    public class Company : IUniqueObject<Guid>,IEquatable<Company>
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string StateName { get; set; }
        public int StateID { get; set; }
        public Guid GetKey => ID;


        public override string ToString()
        {
            return !string.IsNullOrEmpty(StateName) ? $"{Name}, {StateName}" : Name;
        }

        public bool Equals(Company other)
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
            return Equals((Company) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}