#if DEBUG

[TestFixture]
public class Samples
{
    #region CompareImage

    [Test]
    public Task CompareImage() =>
        VerifyFile("sample.jpg");

    #endregion

    #region BackgroundColor

    [Test]
    public Task BackgroundColor() =>
        VerifyFile("transparent.png")
            .ImageMagickBackground(MagickColors.Blue);

    #endregion

    #region PdfPassword

    [Test]
    public Task PdfPassword() =>
        VerifyFile("password.pdf")
            .ImageMagickPdfPassword("password");

    #endregion

    #region VerifyPdf

    [Test]
    public Task VerifyPdf() =>
        VerifyFile("sample.pdf");

    #endregion

    #region VerifyPdfStream

    [Test]
    public Task VerifyPdfStream()
    {
        var stream = new MemoryStream(File.ReadAllBytes("sample.pdf"));
        return Verify(stream, "pdf");
    }

    #endregion

    [Test]
    public Task VerifySvg() =>
        VerifyFile("sample.svg");
}

#endif