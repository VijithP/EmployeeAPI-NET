using EmployeePortal.Api.Services;

namespace EmployeePortal.Api.Tests;

public class EmployeeServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsTwentyEmployees()
    {
        var service = new EmployeeService();

        var employees = await service.GetAllAsync(CancellationToken.None);

        Assert.Equal(20, employees.Count);
        AssertEmployee(employees[0], 1, "Ava", "Patel", "ava.patel@example.com", "Engineering");
        AssertEmployee(employees[1], 2, "Noah", "Kim", "noah.kim@example.com", "Human Resources");
        AssertEmployee(employees[2], 3, "Mia", "Garcia", "mia.garcia@example.com", "Finance");
        Assert.Equal(20, employees.Select(employee => employee.Id).Distinct().Count());
    }

    [Fact]
    public async Task GetAllAsync_ThrowsWhenCancellationIsRequested()
    {
        var service = new EmployeeService();
        using var cancellationSource = new CancellationTokenSource();
        await cancellationSource.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            service.GetAllAsync(cancellationSource.Token));
    }

    private static void AssertEmployee(
        Models.EmployeeResponse employee,
        int id,
        string firstName,
        string lastName,
        string email,
        string department)
    {
        Assert.Equal(id, employee.Id);
        Assert.Equal(firstName, employee.FirstName);
        Assert.Equal(lastName, employee.LastName);
        Assert.Equal(email, employee.Email);
        Assert.Equal(department, employee.Department);
    }
}
