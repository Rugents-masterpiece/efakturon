namespace Efakturon.Infrastructure.Services;

using Efakturon.Application.Interfaces;
using Efakturon.Domain.Entities;
using Efakturon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class StockService : IStockService
{
    private readonly EfakturonDbContext _dbContext;

    public StockService(EfakturonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ReduceStockAsync(Guid tenantId, Guid productId, decimal quantity, Guid invoiceId, string referenceNumber)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == tenantId);

        if (product == null)
            throw new InvalidOperationException($"Produkt {productId} nebol nájdený pre tenanta {tenantId}.");

        if (product.CurrentQuantity < (int)quantity)
            throw new InvalidOperationException(
                $"Nedostatočný sklad. Aktuálne: {product.CurrentQuantity}, potrebné: {(int)quantity}.");

        product.CurrentQuantity -= (int)quantity;
        await _dbContext.SaveChangesAsync();

        var movement = new StockMovement
        {
            TenantId = tenantId,
            Type = StockMovement.MovementType.Out,
            ProductId = productId,
            Quantity = (int)quantity,
            InvoiceId = invoiceId,
            ReferenceNumber = referenceNumber,
            CreatedAtUtc = DateTime.UtcNow
        };
        _dbContext.StockMovements.Add(movement);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RestoreStockAsync(Guid tenantId, Guid productId, decimal quantity, Guid? invoiceId, string referenceNumber)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == tenantId);

        if (product == null)
            throw new InvalidOperationException($"Produkt {productId} nebol nájdený pre tenanta {tenantId}.");

        product.CurrentQuantity += (int)quantity;
        await _dbContext.SaveChangesAsync();

        var movement = new StockMovement
        {
            TenantId = tenantId,
            Type = StockMovement.MovementType.Reversal,
            ProductId = productId,
            Quantity = (int)quantity,
            InvoiceId = invoiceId,
            ReferenceNumber = referenceNumber,
            CreatedAtUtc = DateTime.UtcNow
        };
        _dbContext.StockMovements.Add(movement);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AdjustStockAsync(Guid tenantId, Guid productId, Guid warehouseId, int quantity,
        StockMovement.MovementType type, string? referenceNumber, string? note)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == tenantId);

        if (product == null)
            throw new InvalidOperationException($"Produkt {productId} nebol nájdený pre tenanta {tenantId}.");

        if (type == StockMovement.MovementType.In)
            product.CurrentQuantity += quantity;
        else if (type == StockMovement.MovementType.Out)
        {
            if (product.CurrentQuantity < quantity)
                throw new InvalidOperationException("Nedostatočný sklad pre výdaj.");
            product.CurrentQuantity -= quantity;
        }
        else if (type == StockMovement.MovementType.Adjustment)
        {
            product.CurrentQuantity = quantity;
        }

        await _dbContext.SaveChangesAsync();

        var movement = new StockMovement
        {
            TenantId = tenantId,
            Type = type,
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            ReferenceNumber = referenceNumber,
            Note = note,
            CreatedAtUtc = DateTime.UtcNow
        };
        _dbContext.StockMovements.Add(movement);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> GetStockQuantityAsync(Guid tenantId, Guid productId)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == tenantId);

        return product?.CurrentQuantity ?? 0;
    }
}
