using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

public enum LegalForm
{
    Company = 0,      // s.r.o., a.s., etc.
    SoleTrader = 1    // živnostník
}

public class Company : TenantEntity
{
    public enum CompanyType
    {
        OwnCompany = 0,
        Customer = 1,
        Supplier = 2
    }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public CompanyType Type { get; set; } = CompanyType.OwnCompany;

    public LegalForm LegalForm { get; set; } = LegalForm.Company;

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [Required]
    [StringLength(20)]
    public string ICO { get; set; } = string.Empty;

    [StringLength(20)]
    public string? DIC { get; set; }

    [StringLength(20)]
    public string? DICDPH { get; set; }

    [Column(TypeName = "boolean")]
    public bool IsVATPayer { get; set; }

    [StringLength(100)]
    public string? Street { get; set; }

    [StringLength(10)]
    public string? StreetNumber { get; set; }

    [StringLength(10)]
    public string? PostalCode { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(5)]
    public string? Country { get; set; } = "SK";

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(34)]
    public string? IBAN { get; set; }

    [StringLength(11)]
    public string? BIC { get; set; }

    [StringLength(200)]
    public string? Registration { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    [Column(TypeName = "boolean")]
    public bool IsActive { get; set; } = true;
}
