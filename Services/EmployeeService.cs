using MyWebApi.Models;

namespace MyWebApi.Services;

public sealed class EmployeeService : IEmployeeService
{
    private static readonly IReadOnlyList<EmployeeResponse> Employees =
    [
        new(1, "Ava", "Patel", "ava.patel@example.com", "Engineering"),
        new(2, "Noah", "Kim", "noah.kim@example.com", "Human Resources"),
        new(3, "Mia", "Garcia", "mia.garcia@example.com", "Finance"),
        new(4, "Liam", "Chen", "liam.chen@example.com", "Marketing"),
        new(5, "Sophia", "Brown", "sophia.brown@example.com", "Sales"),
        new(6, "Ethan", "Wilson", "ethan.wilson@example.com", "Engineering"),
        new(7, "Isabella", "Davis", "isabella.davis@example.com", "Finance"),
        new(8, "Lucas", "Martinez", "lucas.martinez@example.com", "Operations"),
        new(9, "Amelia", "Taylor", "amelia.taylor@example.com", "Human Resources"),
        new(10, "Mason", "Anderson", "mason.anderson@example.com", "Engineering"),
        new(11, "Harper", "Thomas", "harper.thomas@example.com", "Customer Support"),
        new(12, "James", "Jackson", "james.jackson@example.com", "Sales"),
        new(13, "Evelyn", "White", "evelyn.white@example.com", "Marketing"),
        new(14, "Benjamin", "Harris", "benjamin.harris@example.com", "Operations"),
        new(15, "Charlotte", "Martin", "charlotte.martin@example.com", "Engineering"),
        new(16, "Elijah", "Thompson", "elijah.thompson@example.com", "Finance"),
        new(17, "Abigail", "Moore", "abigail.moore@example.com", "Customer Support"),
        new(18, "Daniel", "Clark", "daniel.clark@example.com", "Sales"),
        new(19, "Emily", "Lewis", "emily.lewis@example.com", "Human Resources"),
        new(20, "Henry", "Walker", "henry.walker@example.com", "Operations")
    ];

    public Task<IReadOnlyList<EmployeeResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Employees);
    }
}
