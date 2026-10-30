using System.ComponentModel.DataAnnotations;

namespace Efakturon.Domain.Entities;

/// <summary>
/// Invitation for a user to join a tenant.
/// </summary>
public class UserInvitation : TenantEntity
{
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "User";

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiresAtUtc { get; set; }

    public bool IsUsed { get; set; } = false;

    public Guid? UsedByUserId { get; set; }
}
