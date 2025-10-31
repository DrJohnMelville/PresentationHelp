using Melville.FileSystem;
using Melville.Lists;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.IO;

namespace PresentationHelp.DocumentScanner;

public class PdfWriter(ThreadSafeBindableCollection<SingleScannedDocument> scannedDocuments): IDocument
{
    public async Task WriteToFile(string outputFile)
    { 
        QuestPDF.Settings.License = LicenseType.Community;
        await using var stream = File.Create(outputFile);
        this.GeneratePdf(stream);
    }
    public void Compose(IDocumentContainer container)
    {
        foreach (var document in scannedDocuments)
        {
            container.Page(p =>
            {
                p.Size(8.5f, 11, Unit.Inch);
                if (document.Image.PixelWidth > document.Image.PixelHeight)
                    p.Content().RotateRight().AlignCenter().AlignMiddle().Image(document.ImageData);
                else
                    p.Content().AlignCenter().AlignMiddle().Image(document.ImageData);
            });
        }
    }
}