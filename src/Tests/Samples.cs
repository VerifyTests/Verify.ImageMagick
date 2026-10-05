#if DEBUG

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
            .ImageMagickPdfPassword("password")
            .ExcludeTargets("pdf");

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

    #region PagesToInclude

    [Test]
    public Task PagesToInclude() =>
        VerifyFile("sample.pdf")
            .PagesToInclude(1);

    #endregion

    #region ExcludeDerivedTargets

    [Test]
    public Task ExcludeDerivedTargets() =>
        VerifyFile("sample.pdf")
            .ExcludeDerivedTargets("png");

    #endregion

    [Test]
    public Task VerifySvg() =>
        VerifyFile(ProjectFiles.sample_svg.Path);

    [Test]
    public Task VerifySvgWithCrlf()
    {
        var content = ProjectFiles.sample_svg.ReadAllText()
            .Replace("\r\n", "\n")
            .Replace("\n", "\r\n");
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return Verify(stream, "svg");
    }
}

#endif