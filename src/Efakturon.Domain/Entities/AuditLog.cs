using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

/// <summary>
/// Audit log for tracking changes and important events.
/// </summary>
public class AuditLog : TenantEntity
{
    [Required]
    [StringLength(50)]
    public string Action { get; set; } = string.Empty;

    [StringLength(200)]
    public string? EntityName { get; set; }

    [StringLength(50)]
    public string? EntityType { get; set; }

    [StringLength(500)]
    public string? Details { get; set; }

    [StringLength(500)]
    public string? IpAddress { get; set; }

    public Guid? UserId { get; set; }
}
