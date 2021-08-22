using DbChange.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbChange.Repositories
{
    public interface IEmployeeRepository
    {
        List<Employee> GetEmployees();
    }
}
