using System.ComponentModel.DataAnnotations;

namespace Employee_Portal.Application.DTOs;

public sealed class CreateEmployeeRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100000000")]
    public decimal Salary { get; init; }
}