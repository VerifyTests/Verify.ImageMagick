public class OutputsTests
{
    [Test]
    public Task PdfWithoutPng() =>
        VerifyFile("sample.pdf");

    [Test]
    public Task SvgWithoutPng() =>
        VerifyFile("sample.svg");
}
