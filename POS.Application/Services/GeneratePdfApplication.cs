using Microsoft.Extensions.Configuration;
using POS.Application.Dtos.Sale.Response;
using POS.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace POS.Application.Services;

public class GeneratePdfApplication : IGeneratePdfApplication
{
    private readonly IConfiguration _configuration;

    public GeneratePdfApplication(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public byte[] GenerateToPdfInvoice(SaleDetailPdf sale)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        });

        // =============================
        // MÉTODO PARA LA CABECERA
        // =============================
        void ComposeHeader(IContainer container)
        {
            // Estilo de título: tamaño 20, negrita y color azul
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            // Se organiza la cabecera en una fila (row)
            container.Row(row =>
            {
                // Parte izquierda de la cabecera: información de la factura
                row.RelativeItem().Column(column =>
                {
                    // Número de comprobante
                    column.Item().Text(sale.VoucherNumber).Style(titleStyle);

                    // Fecha de venta
                    column.Item().Text(text =>
                    {
                        text.Span("Fecha de Venta: ").SemiBold();
                        text.Span($"{sale.DateOfSale:d}"); // formato corto de fecha
                    });
                    column.Spacing(10); // espacio entre elementos

                    // Tipo de documento
                    column.Item().Text(text =>
                    {
                        text.Span("Tipo de Documento: ").SemiBold();
                        text.Span($"{sale.VoucherDescription}");
                    });

                    // Cliente
                    column.Item().Text(text =>
                    {
                        text.Span("Cliente: ").SemiBold();
                        text.Span($"{sale.Client}");
                    });

                    // Almacén
                    column.Item().Text(text =>
                    {
                        text.Span("Almacén: ").SemiBold();
                        text.Span($"{sale.Warehouse}");
                    });
                });

                // Parte derecha de la cabecera: logo de la empresa
                row.ConstantItem(100) // ancho fijo de 100
                   .Image(_configuration.GetSection("ImagePDF:Entreprise").Value) // se obtiene la imagen de la configuración
                   .FitWidth(); // se ajusta al ancho
            });
        }

        // =============================
        // MÉTODO PARA EL CONTENIDO
        // =============================
        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                // Agregamos la tabla de productos vendidos
                column.Item().Element(ComposeTable);

                // Espaciado entre la tabla y los totales
                column.Spacing(10);

                // SubTotal
                column.Item().AlignRight().Text(text =>
                {
                    text.Span("SubTotal: ").SemiBold();
                    text.Span($"S/ {sale.SubTotal}");
                });

                // IGV
                column.Item().AlignRight().Text(text =>
                {
                    text.Span("IGV:    ").SemiBold();
                    text.Span($"S/ {sale.Igv}");
                });

                // Total de la venta
                column.Item().AlignRight().Text(text =>
                {
                    text.Span("Monto Total: ").Bold();
                    text.Span($"S/ {sale.TotalAmount}").Bold();
                });

                // Si existe observación, se agrega al final
                if (!string.IsNullOrWhiteSpace(sale.Observation))
                    column.Item().PaddingTop(25).Element(ComposeComments);
            });
        }

        // =============================
        // MÉTODO PARA LA TABLA DE DETALLES
        // =============================
        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // Definición de columnas de la tabla
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // Código
                    columns.RelativeColumn(3); // Producto
                    columns.RelativeColumn();  // Precio
                    columns.RelativeColumn();  // Cantidad
                    columns.RelativeColumn();  // Total
                });

                // Encabezado de la tabla
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Código");
                    header.Cell().Element(CellStyle).Text("Producto");
                    header.Cell().Element(CellStyle).AlignRight().Text("Precio");
                    header.Cell().Element(CellStyle).AlignRight().Text("Cantidad");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                    // Estilo del encabezado (negrita, línea inferior)
                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .DefaultTextStyle(x => x.SemiBold())
                            .PaddingVertical(5)
                            .BorderBottom(1)
                            .BorderColor(Colors.Black);
                    }
                });

                // Filas con los productos vendidos
                foreach (var item in sale.SaleDetails)
                {
                    table.Cell().Element(CellStyle).Text(item.Code);                // Código del producto
                    table.Cell().Element(CellStyle).Text(item.Name);                // Nombre del producto
                    table.Cell().Element(CellStyle).AlignRight().Text($"S/ {item.UnitSalePrice}"); // Precio
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Quantity}");        // Cantidad
                    table.Cell().Element(CellStyle).AlignRight().Text($"S/ {item.TotalAmount}");   // Total

                    // Estilo de cada fila de producto
                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .PaddingVertical(5);
                    }
                }
            });
        }

        // =============================
        // MÉTODO PARA COMENTARIOS
        // =============================
        void ComposeComments(IContainer container)
        {
            container
                .Background(Colors.Grey.Lighten3) // Fondo gris claro
                .Padding(10)                      // Márgenes internos
                .Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Text("Observación").FontSize(14); // Título
                    column.Item().Text(sale.Observation);           // Texto de la observación
                });
        }

        // =============================
        // MÉTODO PARA PIE DE PÁGINA
        // =============================
        void ComposeFooter(IContainer container)
        {
            container.AlignCenter()
                     .Text(text =>
                     {
                         text.DefaultTextStyle(x => x.FontSize(10)); // Texto pequeño
                         text.Span("Página ");                      // Texto fijo
                         text.CurrentPageNumber();                   // Número de página actual
                         text.Span(" de ");                          // Texto fijo
                         text.TotalPages();                          // Total de páginas
                     });
        }

        // =============================
        // GENERACIÓN DEL PDF
        // =============================
        var invoice = document.GeneratePdf(); // Se genera el PDF en memoria
        var fileBytes = invoice.ToArray();    // Se convierte a un arreglo de bytes
        return fileBytes;
    }
}
