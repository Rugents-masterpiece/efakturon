using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

/// <summary>
/// Payment record linked to an invoice.
/// </summary>
public class Payment : TenantEntity
{
    [Required]
    public Guid InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? Reference { get; set; }

    [StringLength(50)]
    public string? Method { get; set; } // Bank transfer, Cash, Card, etc.
}
