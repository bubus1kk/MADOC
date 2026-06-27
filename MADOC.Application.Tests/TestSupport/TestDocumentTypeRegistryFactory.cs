using MADOC.Application.Documents;

namespace MADOC.Application.Tests.TestSupport;

internal static class TestDocumentTypeRegistryFactory
{
    public static DocumentTypeRegistry CreateRegistryForTestPrintDocument(string templateFileName = "test_print.html")
    {
        return new DocumentTypeRegistry(new[]
        {
            new DocumentTypeDescriptor(new DocumentTypeKey("test_print"),"Тестовый печатный документ",
                typeof(TestPrintDocument),templateFileName)
        });
    }
}
