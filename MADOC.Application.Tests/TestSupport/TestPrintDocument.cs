namespace MADOC.Application.Tests.TestSupport;

public sealed class TestPrintDocument
{
    public string Text { get; set; } = string.Empty;

    public int Number { get; set; }

    public string ComputedText
    {
        get
        {
            return $"{Text}-{Number}";
        }
    }
}
