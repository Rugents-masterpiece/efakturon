using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

/// <summary>
/// Product / service that can be sold and optionally tracked in stock.
/// </summary>
public class Product : TenantEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Sku { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "ks";

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal SalePriceWithoutVat { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal VatRate { get; set; } = 23m;

    [Column(TypeName = "boolean")]
    public bool IsStockItem { get; set; }

    [Column(TypeName = "int")]
    public int CurrentQuantity { get; set; }

    [Column(TypeName = "int")]
    public int MinimumQuantity { get; set; } = 0;

    [Column(TypeName = "boolean")]
    public bool IsActive { get; set; } = true;
}
