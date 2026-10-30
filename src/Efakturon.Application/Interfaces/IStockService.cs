namespace Efakturon.Application.Interfaces;

using Efakturon.Domain.Entities;

public interface IStockService
{
    /// <summary>
    /// Reduce stock when an invoice is issued.
    /// Returns true if successful, throws on insufficient stock.
    /// </summary>
    Task ReduceStockAsync(Guid tenantId, Guid productId, decimal quantity, Guid invoiceId, string referenceNumber);

    /// <summary>
    /// Restore stock when an invoice is cancelled.
    /// </summary>
    Task RestoreStockAsync(Guid tenantId, Guid productId, decimal quantity, Guid? invoiceId, string referenceNumber);

    /// <summary>
    /// Manual stock adjustment (receipt, correction).
    /// </summary>
    Task AdjustStockAsync(Guid tenantId, Guid productId, Guid warehouseId, int quantity, StockMovement.MovementType type, string? referenceNumber, string? note);

    /// <summary>
    /// Get current stock quantity for a product.
    /// </summary>
    Task<int> GetStockQuantityAsync(Guid tenantId, Guid productId);
}
