namespace Efakturon.Infrastructure.Exports;

using Efakturon.Domain.Entities;

public interface IExportService
{
    byte[] GeneratePdf(Invoice invoice);
    string GenerateHtml(Invoice invoice);
    byte[] GeneratePeppolXml(Invoice invoice);
    byte[] GenerateQrCode(string data);
    string GeneratePayBySquareString(Invoice invoice);
    string GeneratePayMeString(Invoice invoice);
}
