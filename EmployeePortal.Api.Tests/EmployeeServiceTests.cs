using Employee_Portal.Application.DTOs;
using Employee_Portal.Application.Interfaces;
using Employee_Portal.Application.Services;
using Employee_Portal.Domain.Entities;

namespace EmployeePortal.Api.Tests;

public class EmployeeServiceTests
{
    [Fact]
    public async Task GetAllAsync_MapsRepositoryEmployees()
    {
        var repository = new FakeEmployeeRepository
        {
            Employees =
            [
                Employee.Rehydrate(1, "Ava Patel", "ava.patel@example.com", 75000m),
                Employee.Rehydrate(2, "Noah Kim", "noah.kim@example.com", 82000m)
            ]
        };
        var service = new EmployeeService(repository);

        var employees = await service.GetAllAsync(CancellationToken.None);

        Assert.Collection(
            employees,
            employee => Assert.Equal(
                new EmployeeResponse(1, "Ava Patel", "ava.patel@example.com", 75000m),
                employee),
            employee => Assert.Equal(
                new EmployeeResponse(2, "Noah Kim", "noah.kim@example.com", 82000m),
                employee));
    }

    [Fact]
    public async Task GetByIdAsync_MapsEmployeeReturnedByRepository()
    {
        var repository = new FakeEmployeeRepository
        {
            Employee = Employee.Rehydrate(
                7, "Ava Patel", "ava.patel@example.com", 75000m)
        };
        var service = new EmployeeService(repository);

        var employee = await service.GetByIdAsync(7, CancellationToken.None);

        Assert.Equal(
            new EmployeeResponse(7, "Ava Patel", "ava.patel@example.com", 75000m),
            employee);
        Assert.Equal(7, repository.RequestedEmployeeId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenRepositoryDoesNotFindEmployee()
    {
        var service = new EmployeeService(new FakeEmployeeRepository());

        var employee = await service.GetByIdAsync(7, CancellationToken.None);

        Assert.Null(employee);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetByIdAsync_ThrowsForNonPositiveId(int id)
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.GetByIdAsync(id, CancellationToken.None));

        Assert.Null(repository.RequestedEmployeeId);
    }

    [Fact]
    public async Task CreateAsync_PersistsEmployeeAndReturnsGeneratedId()
    {
        var repository = new FakeEmployeeRepository { AddedEmployeeId = 12 };
        var service = new EmployeeService(repository);
        var request = new CreateEmployeeRequest
        {
            Name = " Ava Patel ",
            Email = " ava.patel@example.com ",
            Salary = 75000m
        };

        var employee = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal(
            new EmployeeResponse(12, "Ava Patel", "ava.patel@example.com", 75000m),
            employee);
        Assert.NotNull(repository.AddedEmployee);
        Assert.Equal("Ava Patel", repository.AddedEmployee.Name);
        Assert.Equal("ava.patel@example.com", repository.AddedEmployee.Email);
        Assert.Equal(75000m, repository.AddedEmployee.Salary);
    }

    [Fact]
    public async Task CreateAsync_ThrowsForNullRequest()
    {
        var repository = new FakeEmployeeRepository();
        var service = new EmployeeService(repository);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.CreateAsync(null!, CancellationToken.None));

        Assert.Null(repository.AddedEmployee);
    }

    private sealed class FakeEmployeeRepository : IEmployeeRepository
    {
        public IReadOnlyList<Employee> Employees { get; init; } = [];
        public Employee? Employee { get; init; }
        public int AddedEmployeeId { get; init; } = 1;
        public int? RequestedEmployeeId { get; private set; }
        public Employee? AddedEmployee { get; private set; }

        public Task<Employee?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            RequestedEmployeeId = id;
            return Task.FromResult(Employee);
        }

        public Task<IReadOnlyList<Employee>> GetAllAsync(
            CancellationToken cancellationToken) =>
            Task.FromResult(Employees);

        public Task<int> AddAsync(
            Employee employee,
            CancellationToken cancellationToken)
        {
            AddedEmployee = employee;
            return Task.FromResult(AddedEmployeeId);
        }
    }
}
