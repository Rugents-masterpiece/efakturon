namespace Efakturon.Infrastructure.Services;

using Efakturon.Application.Interfaces;
using Efakturon.Domain.Entities;
using Efakturon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class InvoiceSequenceService : IInvoiceSequenceService
{
    private readonly EfakturonDbContext _dbContext;

    public InvoiceSequenceService(EfakturonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> ReserveNextNumberAsync(Guid tenantId, int? year = null)
    {
        year ??= DateTime.UtcNow.Year;
        var seq = await GetOrCreateSequenceAsync(tenantId, "FA", year.Value);

        var number = seq.NextNumber++;
        seq.UpdatedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return $"{seq.Prefix}{year:D4}-{number:D6}";
    }

    public async Task<string> PreviewNextNumberAsync(Guid tenantId, string prefix, int year)
    {
        var seq = await _dbContext.InvoiceSequences
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Prefix == prefix && s.Year == year);

        if (seq == null)
            return $"{prefix}{year:D4}-000001";

        return $"{prefix}{year:D4}-{seq.NextNumber:D6}";
    }

    public async Task<InvoiceSequence> GetOrCreateSequenceAsync(Guid tenantId, string prefix, int year)
    {
        var seq = await _dbContext.InvoiceSequences
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Prefix == prefix && s.Year == year);

        if (seq == null)
        {
            seq = new InvoiceSequence
            {
                TenantId = tenantId,
                Prefix = prefix,
                NextNumber = 1,
                Year = year,
                IsActive = true
            };
            _dbContext.InvoiceSequences.Add(seq);
            await _dbContext.SaveChangesAsync();
        }

        return seq;
    }
}
