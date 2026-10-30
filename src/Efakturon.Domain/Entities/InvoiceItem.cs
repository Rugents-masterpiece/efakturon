using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

public class InvoiceItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "ks";

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal UnitPriceWithoutVat { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.Percent;

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal VatRate { get; set; } = 23m;

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatBase { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalWithVat { get; set; }

    [Required]
    public int SortOrder { get; set; }
}

public enum DiscountType
{
    Percent = 0,
    Amount = 1
}
