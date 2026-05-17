using ManageEmployee.Models.DTOs;
using ManageEmployee.Models.Entities;

namespace ManageEmployee.Business.Services
{
    public interface IEmployeeService
    {
        Task AddEmployee(EmployeeDto employee);
        Task<IEnumerable<EmployeeDto>> GetEmployees(string? keyword);
        Task<IEnumerable<TitleDto>> GetJobTitles();
        Task<bool> IsUniqueSSN(string input);
    }
}
