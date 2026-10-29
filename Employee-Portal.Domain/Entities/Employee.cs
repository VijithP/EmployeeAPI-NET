namespace Employee_Portal.Domain.Entities;
public sealed class Employee
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public decimal Salary { get; private set; }

    private Employee(
        int id,
        string name,
        string email,
        decimal salary)
    {
        Id = id;
        Name = name;
        Email = email;
        Salary = salary;
    }

    public static Employee Create(
        string name,
        string email,
        decimal salary)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        if (name.Trim().Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters.");

        if (string.IsNullOrWhiteSpace(email) ||
            !System.Net.Mail.MailAddress.TryCreate(email, out var address) ||
            !string.Equals(
                address.Address,
                email.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("A valid email is required.");
        }

        if (salary <= 0 || salary > 100_000_000m)
            throw new ArgumentOutOfRangeException(nameof(salary));

        return new Employee(
            0,
            name.Trim(),
            email.Trim(),
            salary);
    }

    public static Employee Rehydrate(
        int id,
        string name,
        string email,
        decimal salary)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        return new Employee(
            id,
            name,
            email,
            salary);
    }
}