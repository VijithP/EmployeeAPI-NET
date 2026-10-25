using MyWebApi.Models;

namespace MyWebApi.Services;

public sealed class EmployeeService : IEmployeeService
{
    private static readonly IReadOnlyList<EmployeeResponse> Employees =
    [
        new(1, "Ava", "Patel", "ava.patel@example.com", "Engineering"),
        new(2, "Noah", "Kim", "noah.kim@example.com", "Human Resources"),
        new(3, "Mia", "Garcia", "mia.garcia@example.com", "Finance")
    ];

    public Task<IReadOnlyList<EmployeeResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Employees);
    }
}
