using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

public class Invoice : TenantEntity
{
    public enum InvoiceType
    {
        Standard = 0,
        CreditNote = 384
    }

    public enum InvoiceStatus
    {
        Draft = 0,
        Issued = 1,
        Sent = 2,
        PartiallyPaid = 3,
        Paid = 4,
        Overdue = 5,
        Cancelled = 6
    }

    [Required]
    [StringLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public InvoiceType Type { get; set; } = InvoiceType.Standard;

    [Required]
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    [Required]
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    public DateTime? TaxPointDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "EUR";

    [StringLength(20)]
    public string? VariableSymbol { get; set; }

    [StringLength(20)]
    public string? ConstantSymbol { get; set; }

    [StringLength(20)]
    public string? SpecificSymbol { get; set; }

    [StringLength(100)]
    public string? PaymentMethod { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    [StringLength(500)]
    public string? InternalNote { get; set; }

    [StringLength(200)]
    public string? SupplierName { get; set; }

    [StringLength(20)]
    public string? SupplierICO { get; set; }

    [StringLength(20)]
    public string? SupplierDIC { get; set; }

    [StringLength(20)]
    public string? SupplierDICDPH { get; set; }

    [StringLength(200)]
    public string? SupplierAddress { get; set; }

    [StringLength(34)]
    public string? SupplierIBAN { get; set; }

    [StringLength(11)]
    public string? SupplierBIC { get; set; }

    [StringLength(200)]
    public string? CustomerName { get; set; }

    [StringLength(20)]
    public string? CustomerICO { get; set; }

    [StringLength(20)]
    public string? CustomerDIC { get; set; }

    [StringLength(20)]
    public string? CustomerDICDPH { get; set; }

    [StringLength(200)]
    public string? CustomerAddress { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalWithoutVat { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalVat { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalWithVat { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountRemaining => TotalWithVat - AmountPaid;

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatBase5 { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatAmount5 { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatBase19 { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatAmount19 { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatBase23 { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatAmount23 { get; set; }

    public Guid SupplierId { get; set; }
    public Guid CustomerId { get; set; }

    public Company? SupplierNavigation { get; set; }
    public Company? CustomerNavigation { get; set; }

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
