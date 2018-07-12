using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models
{
    public class EmployeeJob:IEquatable<EmployeeJob>
    {
        public int EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeFullName => $"{EmployeeFirstName} {EmployeeLastName}";

        public int JobId { get; set; }
        public string JobTitle { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentTitle { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool Equals(EmployeeJob other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EmployeeId == other.EmployeeId && JobId == other.JobId && DepartmentId == other.DepartmentId && BranchId == other.BranchId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EmployeeJob) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EmployeeId;
                hashCode = (hashCode * 397) ^ JobId;
                hashCode = (hashCode * 397) ^ DepartmentId;
                hashCode = (hashCode * 397) ^ BranchId;
                return hashCode;
            }
        }
    }
}
