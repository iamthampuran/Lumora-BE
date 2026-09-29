namespace Lumora.Application.Features.Studio.Commands.AssignInquiryEmployees;

public record AssignInquiryEmployeesCommand(Guid InquiryId, IEnumerable<Guid> EmployeeIds);