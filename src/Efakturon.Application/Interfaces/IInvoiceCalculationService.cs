namespace Efakturon.Application.Interfaces;

using Efakturon.Domain.Entities;

public interface IInvoiceCalculationService
{
    InvoiceItem CalculateItem(
        string name,
        string? description,
        decimal quantity,
        string unit,
        decimal unitPriceWithoutVat,
        decimal vatRate,
        decimal discountPercent,
        decimal discountAmount,
        DiscountType discountType);

    void CalculateInvoiceTotals(Invoice invoice);

    (decimal vatBase, decimal vatAmount) CalculateVat(decimal baseAmount, decimal vatRate);

    decimal RoundMoney(decimal amount);
}
