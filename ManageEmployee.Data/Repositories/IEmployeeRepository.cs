using ManageEmployee.Models.Entities;

namespace ManageEmployee.Data.Repositories
{
    public interface IEmployeeRepository
    {
        Task AddEmployee(Employee entity);
        Task<IEnumerable<Employee>> GetEmployees(string? keyword);
        Task<IEnumerable<Title>> GetJobTitles();
        Task<bool> IsUniqueSSN(string input);
    }
}
