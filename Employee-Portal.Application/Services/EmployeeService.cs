using Employee_Portal.Application.DTOs;
using Employee_Portal.Application.Interfaces;
using Employee_Portal.Domain.Entities;

namespace Employee_Portal.Application.Services;

public sealed class EmployeeService(IEmployeeRepository repository) : IEmployeeService
{
    public async Task<EmployeeResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        var employee = await repository.GetByIdAsync(
            id, cancellationToken);

        return employee is null ? null : Map(employee);
    }

    public async Task<IReadOnlyList<EmployeeResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var employees = await repository.GetAllAsync(
            cancellationToken);

        return employees.Select(Map).ToList();
    }

    public async Task<EmployeeResponse> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var employee = Employee.Create(
            request.Name,
            request.Email,
            request.Salary);

        var id = await repository.AddAsync(
            employee, cancellationToken);

        return new EmployeeResponse(
            id,
            employee.Name,
            employee.Email,
            employee.Salary);
    }

    private static EmployeeResponse Map(Employee employee) =>
        new(
            employee.Id,
            employee.Name,
            employee.Email,
            employee.Salary);
}