using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

/// <summary>
/// Warehouse storage location.
/// </summary>
public class Warehouse : TenantEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }

    [Column(TypeName = "boolean")]
    public bool IsActive { get; set; } = true;
}
