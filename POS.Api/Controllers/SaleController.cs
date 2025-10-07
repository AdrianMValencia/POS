using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Commons.Bases.Request;
using POS.Application.Dtos.Sale.Request;
using POS.Application.Interfaces;
using POS.Utilities.Static;

namespace POS.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleApplication _saleApplication;
        private readonly IGenerateExcelApplication _generateExcelApplication;
        private readonly IGeneratePdfApplication _generatePdfApplication;

        public SaleController(ISaleApplication saleApplication, IGenerateExcelApplication generateExcelApplication, IGeneratePdfApplication generatePdfApplication)
        {
            _saleApplication = saleApplication;
            _generateExcelApplication = generateExcelApplication;
            _generatePdfApplication = generatePdfApplication;
        }

        [HttpGet]
        public async Task<IActionResult> ListSales([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _saleApplication.ListSales(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsSales();
                var fileBytes = _generateExcelApplication.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("ExportToPdfSaleDetail/{saleId:int}")]
        public async Task<IActionResult> ExportToPdfSaleDetail(int saleId)
        {
            var response = await _saleApplication.ListSalesExportPdf(saleId);
            var fileBytes = _generatePdfApplication.GenerateToPdfInvoice(response.Data!);
            return File(fileBytes, ContentType.ContentTypePdf);
        }

        [HttpGet("{saleId:int}")]
        public async Task<IActionResult> SaleById(int saleId)
        {
            var response = await _saleApplication.SaleById(saleId);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterSale([FromBody] SaleRequestDto requestDto)
        {
            var response = await _saleApplication.RegisterSale(requestDto);
            return Ok(response);
        }

        [HttpPut("Cancel/{saleId:int}")]
        public async Task<IActionResult> CancelSale(int saleId)
        {
            var response = await _saleApplication.CancelSale(saleId);
            return Ok(response);
        }

        [HttpGet("ProductStockByWarehouse")]
        public async Task<IActionResult> GetProductStockByWarehouseId([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _saleApplication.GetProductStockByWarehouseId(filters);
            return Ok(response);
        }
    }
}
