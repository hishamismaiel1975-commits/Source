using Platform.Lib.Services.Grpc.Catalog.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Reporting.API.Reports.Products;

public class ProductsReport : IDocument
{
    private readonly ProductsDTO[] _products;
    private readonly string _title;
    public ProductsReport(ProductsDTO[] products, string title)
    {
        _products = products;
        _title = title;
    }

    public DocumentMetadata GetMetadata() =>
        DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header()
                .Text(_title)
                .FontSize(20)
                .Bold();

            page.Content().PaddingVertical(20).Column(column =>
            {
                column.Item()
                    .Text($"Report Date: {DateTime.Now:dd/MM/yyyy}");

                column.Item().PaddingTop(15).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Product");
                        header.Cell().Element(HeaderCell).Text("Brand");
                        header.Cell().Element(HeaderCell).Text("Type");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Price");
                    });

                    foreach (var product in _products)
                    {
                        table.Cell().Element(BodyCell).Text(product.Name ?? "");
                        table.Cell().Element(BodyCell).Text(product.BrandName ?? "");
                        table.Cell().Element(BodyCell).Text(product.TypeName ?? "");
                        table.Cell().Element(BodyCell).AlignRight()
                            .Text(product.Price.ToString());
                    }
                });
            });

            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Page ");
                text.CurrentPageNumber();
                text.Span(" of ");
                text.TotalPages();
            });
        });
    }

    private static IContainer HeaderCell(IContainer container) =>
        container.Background(Colors.Grey.Lighten2)
            .Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Medium);

    private static IContainer BodyCell(IContainer container) =>
        container.BorderBottom(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);
}
