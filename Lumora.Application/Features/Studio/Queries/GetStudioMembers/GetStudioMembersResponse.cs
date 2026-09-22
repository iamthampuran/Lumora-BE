namespace Lumora.Application.Features.Studio.Queries.GetStudioMembers;

public record GetStudioMembersResponse(Guid Id, string FullName, string Email, string Phone, string EmployeeRole);
