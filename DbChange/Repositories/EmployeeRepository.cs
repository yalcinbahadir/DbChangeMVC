using DbChange.Data;
using DbChange.Entities;
using DbChange.Servers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbChange.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalServer> _hubContext;
        string cs = "";
        public EmployeeRepository(ApplicationDbContext context, IConfiguration configuration, IHubContext<SignalServer> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
            cs = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                SqlDependency.Start(cs);

                string commandText = "Select Id, Name, Age from dbo.Employees";
                SqlCommand command = new SqlCommand(commandText, conn);
                SqlDependency dependency = new SqlDependency(command);
                dependency.OnChange += Dependency_OnChange;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var employee = new Employee
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Age = Convert.ToInt32(reader["Age"])
                    };

                    employees.Add(employee);
                }
            }
            return _context.Employees.ToList();
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            var notification = e.ToString();
            _hubContext.Clients.All.SendAsync("refreshEmployees");

           
           
        }
    }
}
