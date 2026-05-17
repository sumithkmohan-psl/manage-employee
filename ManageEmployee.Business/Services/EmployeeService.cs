using ManageEmployee.Data.Repositories;
using ManageEmployee.Models.DTOs;
using ManageEmployee.Models.Entities;

namespace ManageEmployee.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public Task AddEmployee(EmployeeDto employee)
        {
            try
            {
                var entity = new Employee
                {
                    Name = employee.Name,
                    SSN = employee.SSN,
                    DOB = employee.DOB,
                    Address = employee.Address,
                    City = employee.City,
                    State = employee.State,
                    Zip = employee.Zip,
                    Phone = employee.Phone,
                    JoinDate = employee.JoinDate,
                    ExitDate = employee.ExitDate,
                    Salary = employee.Salary,
                    Title = employee.Title
                };

                return _repository.AddEmployee(entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to add employee at this time.", ex);                
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployees(string? keyword)
        {
            try
            {
                var employees = await _repository.GetEmployees(keyword);

                return employees.Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    SSN = e.SSN,
                    DOB = e.DOB,
                    Address = e.Address,
                    City = e.City,
                    State = e.State,
                    Zip = e.Zip,
                    Phone = e.Phone,
                    JoinDate = e.JoinDate,
                    ExitDate = e.ExitDate,
                    Salary = e.Salary,
                    Title = e.Title
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to retrieve employees at this time.", ex);
            }
        }

        public async Task<IEnumerable<TitleDto>> GetJobTitles()
        {
            try
            {
                var titles = await _repository.GetJobTitles();

                return titles.Select(t => new TitleDto
                {
                    Name = t.Name,
                    Min = t.Min,
                    Max = t.Max
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to retrieve job titles at this time.", ex);
            }
        }

        public async Task<bool> IsUniqueSSN(string input)
        {
            try
            {
                return await _repository.IsUniqueSSN(input);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to validate SSN uniqueness at this time.", ex);
            }
        }
    }
}
