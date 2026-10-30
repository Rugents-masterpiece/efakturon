using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

/// <summary>
/// Invoice number sequence per tenant.
/// </summary>
public class InvoiceSequence : TenantEntity
{
    [Required]
    [StringLength(50)]
    public string Prefix { get; set; } = "FA";

    [Required]
    public int NextNumber { get; set; } = 1;

    [Required]
    public int Year { get; set; }

    [Column(TypeName = "boolean")]
    public bool IsActive { get; set; } = true;

    [UniqueSequence]
    public string SequenceKey => $"{Prefix}_{Year}";
}

/// <summary>
/// Attribute to mark the unique sequence key property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class UniqueSequenceAttribute : Attribute { }
