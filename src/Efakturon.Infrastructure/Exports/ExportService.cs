namespace Efakturon.Infrastructure.Exports;

using Efakturon.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;
using System.Text;
using System.Text.RegularExpressions;

public class ExportService : IExportService
{
    public ExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GeneratePdf(Invoice invoice)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                page.Header().Text($"FAKTÚRA č. {invoice.InvoiceNumber}  |  Dátum: {invoice.IssueDate:dd.MM.yyyy}").FontSize(14).Bold();

                page.Content().Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Dodávateľ").Bold().FontSize(12);
                            c.Item().PaddingBottom(5).Text(invoice.SupplierName ?? "");
                            c.Item().Text($"IČO: {invoice.SupplierICO ?? ""}");
                            if (!string.IsNullOrEmpty(invoice.SupplierDIC)) c.Item().Text($"DIČ: {invoice.SupplierDIC}");
                            if (!string.IsNullOrEmpty(invoice.SupplierDICDPH)) c.Item().Text($"IČ DPH: {invoice.SupplierDICDPH}");
                            c.Item().Text(invoice.SupplierAddress ?? "");
                        });

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Odberateľ").Bold().FontSize(12);
                            c.Item().PaddingBottom(5).Text(invoice.CustomerName ?? "");
                            c.Item().Text($"IČO: {invoice.CustomerICO ?? ""}");
                            if (!string.IsNullOrEmpty(invoice.CustomerDIC)) c.Item().Text($"DIČ: {invoice.CustomerDIC}");
                            if (!string.IsNullOrEmpty(invoice.CustomerDICDPH)) c.Item().Text($"IČ DPH: {invoice.CustomerDICDPH}");
                            c.Item().Text(invoice.CustomerAddress ?? "");
                        });
                    });

                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Popis").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Množstvo");
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Jedn. cena");
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Zľava");
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("DPH %");
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Celkom s DPH");
                        });

                        foreach (var item in invoice.Items.OrderBy(i => i.SortOrder))
                        {
                            table.Cell().Padding(4).BorderBottom(1).Text(item.Name);
                            table.Cell().Padding(4).BorderBottom(1).Text($"{item.Quantity} {item.Unit}");
                            table.Cell().Padding(4).BorderBottom(1).Text($"{item.UnitPriceWithoutVat:F2} €");
                            table.Cell().Padding(4).BorderBottom(1).Text(GetDiscountText(item));
                            table.Cell().Padding(4).BorderBottom(1).Text($"{item.VatRate:F0} %");
                            table.Cell().Padding(4).BorderBottom(1).Text($"{item.TotalWithVat:F2} €");
                        }
                    });

                    column.Item().PaddingTop(10).AlignRight().Table(vatTable =>
                    {
                        vatTable.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(1);
                        });

                        if (invoice.VatBase5 > 0)
                        {
                            vatTable.Cell().Padding(2).Text("Základ DPH 5 %:");
                            vatTable.Cell().Padding(2).AlignRight().Text($"{invoice.VatBase5:F2} €");
                            vatTable.Cell().Padding(2).AlignRight().Text($"{invoice.VatAmount5:F2} €");
                        }
                        if (invoice.VatBase19 > 0)
                        {
                            vatTable.Cell().Padding(2).Text("Základ DPH 19 %:");
                            vatTable.Cell().Padding(2).AlignRight().Text($"{invoice.VatBase19:F2} €");
                            vatTable.Cell().Padding(2).AlignRight().Text($"{invoice.VatAmount19:F2} €");
                        }
                        if (invoice.VatBase23 > 0)
                        {
                            vatTable.Cell().Padding(2).Text("Základ DPH 23 %:");
                            vatTable.Cell().Padding(2).AlignRight().Text($"{invoice.VatBase23:F2} €");
                            vatTable.Cell().Padding(2).AlignRight().Text($"{invoice.VatAmount23:F2} €");
                        }

                        vatTable.Cell().PaddingTop(5).BorderTop(1).Text("Celkom bez DPH:");
                        vatTable.Cell().PaddingTop(5).BorderTop(1).AlignRight().Text($"{invoice.TotalWithoutVat:F2} €");
                        vatTable.Cell().PaddingTop(5).BorderTop(1).AlignRight().Text($"{invoice.TotalVat:F2} €");

                        vatTable.Cell().PaddingTop(5).BorderTop(1).Text("Celkom s DPH:");
                        vatTable.Cell().PaddingTop(5).BorderTop(1).AlignRight().Text($"{invoice.TotalWithVat:F2} €");
                        vatTable.Cell().PaddingTop(5).BorderTop(1).Text("");
                    });

                    column.Item().PaddingTop(10).Text("Platobné údaje:");
                    column.Item().Text($"Splatnosť: {invoice.DueDate:dd.MM.yyyy}");
                    if (!string.IsNullOrEmpty(invoice.VariableSymbol))
                        column.Item().Text($"Variabilný symbol: {invoice.VariableSymbol}");
                    if (!string.IsNullOrEmpty(invoice.PaymentMethod))
                        column.Item().Text($"Spôsob úhrady: {invoice.PaymentMethod}");
                    if (!string.IsNullOrEmpty(invoice.Note))
                        column.Item().PaddingTop(5).Text(invoice.Note);
                });
            });
        }).GeneratePdf();
    }

    public string GenerateHtml(Invoice invoice)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"sk\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine($"<title>Faktúra č. {Regex.Escape(invoice.InvoiceNumber)}</title>");
        sb.AppendLine("<style>body{font-family:Arial,sans-serif;margin:40px;color:#333}");
        sb.AppendLine("h1{color:#2c3e50;border-bottom:2px solid #3498db;padding-bottom:10px}");
        sb.AppendLine("table{width:100%;border-collapse:collapse;margin:20px 0}");
        sb.AppendLine("th,td{padding:8px 12px;text-align:left;border-bottom:1px solid #ddd}");
        sb.AppendLine("th{background-color:#f8f9fa;font-weight:600}");
        sb.AppendLine(".totals{text-align:right;margin-top:20px}.totals .grand{font-size:1.2em;font-weight:bold;color:#27ae60}</style></head><body>");
        sb.AppendLine($"<h1>FAKTÚRA č. {Regex.Escape(invoice.InvoiceNumber)}</h1>");
        sb.AppendLine("<div style=\"display:flex;justify-content:space-between\">");
        sb.AppendLine($"<div><h3>Dodávateľ</h3><div>{Regex.Escape(invoice.SupplierName ?? "")}</div>");
        sb.AppendLine($"<div>IČO: {Regex.Escape(invoice.SupplierICO ?? "")}</div>");
        if (!string.IsNullOrEmpty(invoice.SupplierDIC)) sb.AppendLine($"<div>DIČ: {Regex.Escape(invoice.SupplierDIC)}</div>");
        sb.AppendLine($"<div>{Regex.Escape(invoice.SupplierAddress ?? "")}</div></div>");
        sb.AppendLine($"<div><h3>Odberateľ</h3><div>{Regex.Escape(invoice.CustomerName ?? "")}</div>");
        sb.AppendLine($"<div>IČO: {Regex.Escape(invoice.CustomerICO ?? "")}</div>");
        if (!string.IsNullOrEmpty(invoice.CustomerDIC)) sb.AppendLine($"<div>DIČ: {Regex.Escape(invoice.CustomerDIC)}</div>");
        sb.AppendLine($"<div>{Regex.Escape(invoice.CustomerAddress ?? "")}</div></div></div>");
        sb.AppendLine("<table><thead><tr><th>Popis</th><th>Množstvo</th><th>Jedn. cena</th><th>DPH</th><th>Celkom</th></tr></thead><tbody>");
        foreach (var item in invoice.Items.OrderBy(i => i.SortOrder))
        {
            sb.AppendLine($"<tr><td>{Regex.Escape(item.Name)}</td><td>{item.Quantity} {item.Unit}</td>");
            sb.AppendLine($"<td>{item.UnitPriceWithoutVat:F2} €</td><td>{item.VatRate:F0}%</td>");
            sb.AppendLine($"<td>{item.TotalWithVat:F2} €</td></tr>");
        }
        sb.AppendLine("</tbody></table>");
        sb.AppendLine($"<div class=\"totals\"><div>Bez DPH: {invoice.TotalWithoutVat:F2} €</div>");
        sb.AppendLine($"<div>DPH: {invoice.TotalVat:F2} €</div>");
        sb.AppendLine($"<div class=\"grand\">Celkom s DPH: {invoice.TotalWithVat:F2} €</div></div></body></html>");
        return sb.ToString();
    }

    public byte[] GeneratePeppolXml(Invoice invoice)
    {
        var ubl = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
        var cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        var cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";

        var doc = new System.Xml.Linq.XDocument(
            new System.Xml.Linq.XDeclaration("1.0", "UTF-8", "true"),
            new System.Xml.Linq.XElement(ubl + ":Invoice",
                new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns + "ubl", ubl),
                new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns + "cbc", cbc),
                new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns + "cac", cac),
                new System.Xml.Linq.XElement(cbc + ":UBLVersionID", "2.1"),
                new System.Xml.Linq.XElement(cbc + ":CustomizationID", "urn:fdc:peppol.eu:poacc:billing:3.0"),
                new System.Xml.Linq.XElement(cbc + ":ID", invoice.InvoiceNumber),
                new System.Xml.Linq.XElement(cbc + ":IssueDate", invoice.IssueDate.ToString("yyyy-MM-dd")),
                new System.Xml.Linq.XElement(cbc + ":InvoiceTypeCode", "380"),
                new System.Xml.Linq.XElement(cbc + ":DocumentCurrencyCode", invoice.Currency),
                new System.Xml.Linq.XElement(cac + ":AccountingSupplierParty",
                    new System.Xml.Linq.XElement(cac + ":Party",
                        new System.Xml.Linq.XElement(cac + ":PartyName",
                            new System.Xml.Linq.XElement(cbc + ":Name", invoice.SupplierName)),
                        new System.Xml.Linq.XElement(cac + ":PostalAddress",
                            new System.Xml.Linq.XElement(cbc + ":Country",
                                new System.Xml.Linq.XElement(cbc + ":IdentificationCode", "SK"))))),
                new System.Xml.Linq.XElement(cac + ":AccountingCustomerParty",
                    new System.Xml.Linq.XElement(cac + ":Party",
                        new System.Xml.Linq.XElement(cac + ":PartyName",
                            new System.Xml.Linq.XElement(cbc + ":Name", invoice.CustomerName)),
                        new System.Xml.Linq.XElement(cac + ":PostalAddress",
                            new System.Xml.Linq.XElement(cbc + ":Country",
                                new System.Xml.Linq.XElement(cbc + ":IdentificationCode", "SK"))))),
                new System.Xml.Linq.XElement(cac + ":TaxTotal",
                    new System.Xml.Linq.XElement(cbc + ":TaxAmount", invoice.TotalVat)),
                new System.Xml.Linq.XElement(cac + ":LegalMonetaryTotal",
                    new System.Xml.Linq.XElement(cbc + ":LineExtensionAmount", invoice.TotalWithoutVat),
                    new System.Xml.Linq.XElement(cbc + ":TaxExclusiveAmount", invoice.TotalWithoutVat),
                    new System.Xml.Linq.XElement(cbc + ":TaxInclusiveAmount", invoice.TotalWithVat),
                    new System.Xml.Linq.XElement(cbc + ":PayableAmount", invoice.AmountRemaining > 0 ? invoice.AmountRemaining : invoice.TotalWithVat)),
                from item in invoice.Items.OrderBy(i => i.SortOrder)
                select new System.Xml.Linq.XElement(cac + ":InvoiceLine",
                    new System.Xml.Linq.XElement(cbc + ":ID", item.SortOrder),
                    new System.Xml.Linq.XElement(cbc + ":InvoicedQuantity", item.Quantity),
                    new System.Xml.Linq.XElement(cbc + ":LineExtensionAmount", item.VatBase),
                    new System.Xml.Linq.XElement(cbc + ":TaxAmount", item.VatAmount),
                    new System.Xml.Linq.XElement(cac + ":Item",
                        new System.Xml.Linq.XElement(cbc + ":Name", item.Name)),
                    new System.Xml.Linq.XElement(cac + ":Price",
                        new System.Xml.Linq.XElement(cbc + ":PriceAmount", item.UnitPriceWithoutVat)))
            )
        );

        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.UTF8);
        doc.Save(writer);
        return ms.ToArray();
    }

    public byte[] GenerateQrCode(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var pngByteQRCode = new PngByteQRCode(qrCodeData);
        return pngByteQRCode.GetGraphic(5);
    }

    public string GeneratePayBySquareString(Invoice invoice)
    {
        var amount = invoice.TotalWithVat.ToString("F2");
        var currency = invoice.Currency;
        var vs = invoice.VariableSymbol ?? invoice.InvoiceNumber;
        var message = $"Platba za faktúru {invoice.InvoiceNumber}";
        var iban = invoice.SupplierIBAN ?? "";

        return $"*ACC:{iban}*AM:{amount}*CC:{currency}*RF:{invoice.InvoiceNumber}*MSG:{message}*X-VS:{vs}";
    }

    public string GeneratePayMeString(Invoice invoice)
    {
        var amount = invoice.TotalWithVat.ToString("F2");
        var iban = invoice.SupplierIBAN ?? "";
        var vs = invoice.VariableSymbol ?? invoice.InvoiceNumber;

        return $"{iban}|{amount}|{invoice.Currency}|{vs}|Platba za faktúru {invoice.InvoiceNumber}";
    }

    private static string GetDiscountText(InvoiceItem item)
    {
        if (item.DiscountType == DiscountType.Percent && item.DiscountPercent > 0)
            return $"{item.DiscountPercent:F0}%";
        if (item.DiscountAmount > 0)
            return $"{item.DiscountAmount:F2} €";
        return "-";
    }
}
