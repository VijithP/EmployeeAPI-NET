namespace Employee_Portal.Application.DTOs;

public sealed record EmployeeResponse(
    int Id,
    string Name,
    string Email,
    decimal Salary);