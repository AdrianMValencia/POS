using POS.Application.Dtos.Sale.Response;

namespace POS.Application.Interfaces;

public interface IGeneratePdfApplication
{
    byte[] GenerateToPdfInvoice(SaleDetailPdf sale);
}
