using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models
{
    public class EmployeeJob
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
    }
}
