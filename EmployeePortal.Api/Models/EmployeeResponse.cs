namespace EmployeePortal.Api.Models;

/// <summary>Represents an employee returned by the API.</summary>
public sealed record EmployeeResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Department);
