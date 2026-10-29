using System.Data;
using Employee_Portal.Application.Interfaces;
using Employee_Portal.Domain.Entities;
using Employee_Portal.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Employee_Portal.Infrastructure.Repositories;

//Reposiory Design Pattern
public sealed class EmployeeRepository(SqlConnectionFactory connectionFactory) : IEmployeeRepository
{
    public async Task<Employee?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Name, Email, Salary
            FROM dbo.Employees
            WHERE Id = @Id;
            """;

        await using var connection =
            connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return MapEmployee(reader);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Name, Email, Salary
            FROM dbo.Employees
            ORDER BY Id;
            """;

        var employees = new List<Employee>();

        await using var connection =
            connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            employees.Add(MapEmployee(reader));
        }

        return employees;
    }

    public async Task<int> AddAsync(
        Employee employee,
        CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.Employees (Name, Email, Salary)
            OUTPUT INSERTED.Id
            VALUES (@Name, @Email, @Salary);
            """;

        await using var connection =
            connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@Name", SqlDbType.NVarChar, 100).Value = employee.Name;

        command.Parameters.Add(
            "@Email", SqlDbType.NVarChar, 256).Value = employee.Email;

        var salaryParameter = command.Parameters.Add(
            "@Salary", SqlDbType.Decimal);

        salaryParameter.Precision = 18;
        salaryParameter.Scale = 2;
        salaryParameter.Value = employee.Salary;

        var result = await command.ExecuteScalarAsync(
            cancellationToken);

        return Convert.ToInt32(result);
    }

    private static Employee MapEmployee(SqlDataReader reader)
    {
        return Employee.Rehydrate(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Name")),
            reader.GetString(reader.GetOrdinal("Email")),
            reader.GetDecimal(reader.GetOrdinal("Salary")));
    }
}