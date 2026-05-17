using ManageEmployee.Data.Connection;
using ManageEmployee.Models.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManageEmployee.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public EmployeeRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddEmployee(Employee entity)
        {
            try
            {
                await using SqlConnection con = _connectionFactory.CreateConnection();
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_AddEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.Name;
                    cmd.Parameters.Add("@SSN", SqlDbType.NVarChar, 11).Value = entity.SSN;
                    cmd.Parameters.Add("@DOB", SqlDbType.Date).Value = entity.DOB;
                    cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 200).Value = entity.Address;
                    cmd.Parameters.Add("@City", SqlDbType.NVarChar, 100).Value = entity.City;
                    cmd.Parameters.Add("@State", SqlDbType.NVarChar, 100).Value = entity.State;
                    cmd.Parameters.Add("@Zip", SqlDbType.NVarChar, 10).Value = entity.Zip;
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = entity.Phone;
                    cmd.Parameters.Add("@JoinDate", SqlDbType.Date).Value = entity.JoinDate;
                    cmd.Parameters.Add("@Salary", SqlDbType.Decimal).Value = entity.Salary;
                    cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = entity.Title;

                    await cmd.ExecuteNonQueryAsync();
                }

            }
            catch (SqlException ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while adding the employee. Please try again later.", ex);
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployees(string? keyword)
        {
            await using SqlConnection con = _connectionFactory.CreateConnection();

            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_GetEmployees", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@keyword", SqlDbType.NVarChar, 100).Value = keyword == null ? (object)DBNull.Value : keyword;

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    List<Employee> employees = new List<Employee>();
                    while (await reader.ReadAsync())
                    {
                        Employee emp = new Employee
                        {
                            EmployeeId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            SSN = reader.GetString(2),
                            DOB = reader.GetDateTime(3),
                            Address = reader.GetString(4),
                            City = reader.GetString(5),
                            State = reader.GetString(6),
                            Zip = reader.GetString(7),
                            Phone = reader.GetString(8),
                            JoinDate = reader.GetDateTime(9),
                            ExitDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                            Salary = reader.GetDecimal(11),
                            Title = reader.GetString(12)
                        };
                        employees.Add(emp);
                    }
                    return employees;
                }
            }
        }

        public async Task<IEnumerable<Title>> GetJobTitles()
        {
            await using SqlConnection con = _connectionFactory.CreateConnection();

            await con.OpenAsync();

            string query = @"select Title,MIN(Salary) MinSalary,MAX(Salary) MaxSalary from EmployeeSalary
                    group by Title
                    order by Title";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    List<Title> titles = new List<Title>();
                    while (await reader.ReadAsync())
                    {
                        titles.Add(new Title
                        {
                            Name = reader.GetString(0),
                            Min = reader.GetDecimal(1),
                            Max = reader.GetDecimal(2)
                        });
                    }
                    return titles;
                }
            }
        }

        public async Task<bool> IsUniqueSSN(string input)
        {
            await using SqlConnection con = _connectionFactory.CreateConnection();

            await con.OpenAsync();

            const string query = @"SELECT COUNT(1)
                                   FROM dbo.Employee
                                   WHERE SSN = @SSN;";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@SSN", SqlDbType.NVarChar, 11).Value = input ?? (object)DBNull.Value;

                var result = await cmd.ExecuteScalarAsync();

                int count = 0;
                if (result != null && result != DBNull.Value)
                {
                    count = Convert.ToInt32(result);
                }

                return count == 0;
            }
        }
    }
}
