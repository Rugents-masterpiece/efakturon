using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efakturon.Domain.Entities;

public class Tenant : TenantEntity
{
    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [RegularExpression(@"^[a-z0-9]([a-z0-9-]*[a-z0-9])?$", ErrorMessage = "Subdoména môže obsahovať len malé písmená, čísla a pomlčky.")]
    public string Subdomain { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(200)]
    public string? ContactEmail { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    [StringLength(50)]
    public string Plan { get; set; } = "Basic";

    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<Company> Companies { get; set; } = new List<Company>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
    public ICollection<UserInvitation> Invitations { get; set; } = new List<UserInvitation>();
}
