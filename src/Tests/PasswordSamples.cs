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
    public async Task PasswordWithPdfTargetThrows()
    {
        var exception = await Assert.ThrowsAsync<Exception>(
            () => VerifyFile("password.pdf")
                .ImageMagickPdfPassword("password"));

        await Assert.That(exception!.Message).Contains("""ExcludeTargets("pdf")""");
    }
}
