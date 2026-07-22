[TestFixture]
public class PasswordSamples
{
    // A password protected pdf cannot produce a deterministic pdf target, so the pdf target is
    // excluded and only the rendered pages are verified.
    [Test]
    public Task PasswordSample() =>
        VerifyFile("password.pdf")
            .ImageMagickPdfPassword("password")
            .ExcludeTargets("pdf");

    [Test]
    public void PasswordWithPdfTargetThrows()
    {
        var exception = Assert.ThrowsAsync<Exception>(
            () => VerifyFile("password.pdf")
                .ImageMagickPdfPassword("password"))!;

        Assert.That(exception.Message, Does.Contain("""ExcludeTargets("pdf")"""));
    }
}
