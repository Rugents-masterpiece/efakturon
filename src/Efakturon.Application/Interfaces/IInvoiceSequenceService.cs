namespace Efakturon.Application.Interfaces;

using Efakturon.Domain.Entities;

public interface IInvoiceSequenceService
{
    /// <summary>
    /// Reserve the next invoice number for the given tenant and year.
    /// The number is consumed only when the invoice is issued (not on draft save).
    /// Thread-safe via database-level locking.
    /// </summary>
    Task<string> ReserveNextNumberAsync(Guid tenantId, int? year = null);

    /// <summary>
    /// Preview the next number without consuming it.
    /// </summary>
    Task<string> PreviewNextNumberAsync(Guid tenantId, string prefix, int year);

    /// <summary>
    /// Get or create a sequence for the given tenant, prefix, and year.
    /// </summary>
    Task<InvoiceSequence> GetOrCreateSequenceAsync(Guid tenantId, string prefix, int year);
}
