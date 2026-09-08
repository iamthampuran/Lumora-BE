namespace Lumora.Application.Features.Studio.Commands.AddEmployees;

public record AddEmployeesCommand(Guid StudioId, IEnumerable<EmployeeDetail> EmployeeDetails);

public record EmployeeDetail(string Name, string Phonenumber, string Email, string Role);