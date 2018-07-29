using System;
using System.Collections.Generic;
using HR.Data.Repositories;
using HR.Data.Rest.Repositories;

namespace HR.Data.REST
{
    public static class RestRepositories
    {
        public static KeyValuePair<Type, Type>[] All = 
        {
            new KeyValuePair<Type,Type>(typeof(IBranchRepository),typeof(BranchRepository)),
            new KeyValuePair<Type,Type>(typeof(ICityRepository),typeof(CityRepository)),
            new KeyValuePair<Type,Type>(typeof(ICompanyRepository),typeof(CompanyRepository)),
            new KeyValuePair<Type,Type>(typeof(ICountryRepository),typeof(CountryRepository)),
            new KeyValuePair<Type,Type>(typeof(IDepartmentRepository),typeof(DepartmentRepository)),
            new KeyValuePair<Type,Type>(typeof(IEmployeeRepository),typeof(EmployeeRepository)),
            new KeyValuePair<Type,Type>(typeof(IJobRepository),typeof(JobRepository)),
            new KeyValuePair<Type,Type>(typeof(IUserRepository),typeof(UserRepository)),
        };
    }
}
