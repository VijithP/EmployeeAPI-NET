using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Employee_Portal.Infrastructure.Data;

//Factory Design Pattern
public sealed class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("EmployeeDb")
            ?? throw new InvalidOperationException(
                "EmployeeDb connection string is missing.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}