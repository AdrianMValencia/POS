namespace POS.Application.Dtos.Sale.Response;

public class SaleDetailPdf
{
    public int SaleId { get; set; }
    public string VoucherDescription { get; set; } = null!;
    public string VoucherNumber { get; set; } = null!;
    public string Client { get; set; } = null!;
    public string Warehouse { get; set; } = null!;
    public decimal SubTotal { get; set; }
    public decimal Igv { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Observation { get; set; }
    public DateTime DateOfSale { get; set; }
    public ICollection<SaleDetailByIdResponseDto> SaleDetails { get; set; } = null!;
}
