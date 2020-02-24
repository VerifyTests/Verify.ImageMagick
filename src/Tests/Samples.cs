using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

#region TestDefinition
public class Samples :
    VerifyBase
{
    public Samples(ITestOutputHelper output) :
        base(output)
    {
    }

    static Samples()
    {
        VerifyImageMagick.Initialize();
    }
    #endregion

    #region VerifyPdf

    [Fact]
    public Task VerifyPdf()
    {
        return VerifyFile("sample.pdf");
    }

    #endregion

    #region VerifyPdfStream

    [Fact]
    public Task VerifyPdfStream()
    {
        var settings = new VerifySettings();
        settings.UseExtension("pdf");
        return Verify(File.OpenRead("sample.pdf"), settings);
    }

    #endregion
}