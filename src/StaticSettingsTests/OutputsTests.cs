public class OutputsTests
{
    [Test]
    public Task PdfWithoutPng() =>
        VerifyFile(ProjectFiles.sample_pdf.Path);

    [Test]
    public Task SvgWithoutPng() =>
        VerifyFile(ProjectFiles.sample_svg.Path);
}
