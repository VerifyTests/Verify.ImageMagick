using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyNUnit;
using NUnit.Framework;

#region TestDefinition
[TestFixture]
public class Samples
{
    static Samples()
    {
        VerifyImageMagick.Initialize();
    }
    #endregion

    #region VerifyPdf

    [Test]
    public Task VerifyPdf()
    {
        return Verifier.VerifyFile("sample.pdf");
    }

    #endregion

    #region VerifyPdfStream

    [Test]
    public Task VerifyPdfStream()
    {
        var settings = new VerifySettings();
        settings.UseExtension("pdf");
        return Verifier.Verify(File.OpenRead("sample.pdf"), settings);
    }

    #endregion
}