using Microsoft.Data.SqlClient;

namespace ManageEmployee.Data.Connection
{
    public interface IConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
