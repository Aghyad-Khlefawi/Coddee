// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee;

namespace HR.Data.Models
{
    public class State : IUniqueObject<int>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int GetKey => ID;
    }
}