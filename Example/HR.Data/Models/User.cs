using System;
using Coddee;

namespace HR.Data.Models
{
    public class User:IUniqueObject<Guid>
    {
        public Guid ID { get; set; }
        public string Username { get; set; }
        public Guid GetKey => ID;
    }
}
