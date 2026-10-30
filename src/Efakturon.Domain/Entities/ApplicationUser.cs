using Microsoft.AspNetCore.Identity;

namespace Efakturon.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public bool IsSuperAdmin { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LockedOutAtUtc { get; set; }
    public int FailedLoginAttempts { get; set; }
}
