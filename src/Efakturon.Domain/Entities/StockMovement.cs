using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

/// <summary>
/// Stock movement record.
/// </summary>
public class StockMovement : TenantEntity
{
    public enum MovementType
    {
        In = 0,       // Receipt
        Out = 1,      // Issue (invoice)
        Adjustment = 2, // Manual correction
        Reversal = 3  // Reversal
    }

    [Required]
    public MovementType Type { get; set; }

    public Guid WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    [Column(TypeName = "int")]
    public int Quantity { get; set; }

    [StringLength(200)]
    public string? ReferenceNumber { get; set; }

    public Guid? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }
}
