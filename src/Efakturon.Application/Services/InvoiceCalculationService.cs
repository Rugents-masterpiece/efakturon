namespace Efakturon.Application.Services;

using Efakturon.Application.Interfaces;
using Efakturon.Domain.Entities;

public class InvoiceCalculationService : IInvoiceCalculationService
{
    public InvoiceItem CalculateItem(
        string name,
        string? description,
        decimal quantity,
        string unit,
        decimal unitPriceWithoutVat,
        decimal vatRate,
        decimal discountPercent,
        decimal discountAmount,
        DiscountType discountType)
    {
        if (quantity <= 0)
            throw new ArgumentException("Množstvo musí byť väčšie ako 0.", nameof(quantity));

        if (unitPriceWithoutVat < 0)
            throw new ArgumentException("Cena nemôže byť záporná.", nameof(unitPriceWithoutVat));

        if (vatRate < 0 || vatRate > 100)
            throw new ArgumentException("DPH sadzba musí byť medzi 0 a 100.", nameof(vatRate));

        decimal grossTotal = quantity * unitPriceWithoutVat;
        decimal discount = 0m;

        if (discountType == DiscountType.Percent)
        {
            if (discountPercent < 0 || discountPercent > 100)
                throw new ArgumentException("Zľava v percentách musí byť medzi 0 a 100.", nameof(discountPercent));
            discount = RoundMoney(grossTotal * discountPercent / 100m);
        }
        else
        {
            if (discountAmount < 0)
                throw new ArgumentException("Zľava v sume nemôže byť záporná.", nameof(discountAmount));
            discount = Math.Min(discountAmount, grossTotal);
            discount = RoundMoney(discount);
        }

        decimal netTotal = RoundMoney(grossTotal - discount);
        var (vatBase, vatAmount) = CalculateVat(netTotal, vatRate);
        decimal totalWithVat = RoundMoney(netTotal + vatAmount);

        return new InvoiceItem
        {
            Name = name,
            Description = description,
            Quantity = quantity,
            Unit = unit,
            UnitPriceWithoutVat = unitPriceWithoutVat,
            DiscountPercent = discountPercent,
            DiscountAmount = discountAmount,
            DiscountType = discountType,
            VatRate = vatRate,
            VatBase = vatBase,
            VatAmount = vatAmount,
            TotalWithVat = totalWithVat
        };
    }

    public void CalculateInvoiceTotals(Invoice invoice)
    {
        decimal totalWithoutVat = 0m;
        decimal totalVat = 0m;

        invoice.VatBase5 = 0m;
        invoice.VatAmount5 = 0m;
        invoice.VatBase19 = 0m;
        invoice.VatAmount19 = 0m;
        invoice.VatBase23 = 0m;
        invoice.VatAmount23 = 0m;

        foreach (var item in invoice.Items)
        {
            totalWithoutVat += item.VatBase;
            totalVat += item.VatAmount;

            if (Math.Abs(item.VatRate - 5m) < 0.01m)
            {
                invoice.VatBase5 += item.VatBase;
                invoice.VatAmount5 += item.VatAmount;
            }
            else if (Math.Abs(item.VatRate - 19m) < 0.01m)
            {
                invoice.VatBase19 += item.VatBase;
                invoice.VatAmount19 += item.VatAmount;
            }
            else if (Math.Abs(item.VatRate - 23m) < 0.01m)
            {
                invoice.VatBase23 += item.VatBase;
                invoice.VatAmount23 += item.VatAmount;
            }
            else
            {
                invoice.VatBase23 += item.VatBase;
                invoice.VatAmount23 += item.VatAmount;
            }
        }

        invoice.TotalWithoutVat = RoundMoney(totalWithoutVat);
        invoice.TotalVat = RoundMoney(totalVat);
        invoice.TotalWithVat = RoundMoney(invoice.TotalWithoutVat + invoice.TotalVat);
    }

    public (decimal vatBase, decimal vatAmount) CalculateVat(decimal baseAmount, decimal vatRate)
    {
        decimal vatAmount = RoundMoney(baseAmount * vatRate / 100m);
        return (RoundMoney(baseAmount), vatAmount);
    }

    public decimal RoundMoney(decimal amount)
    {
        return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }
}
