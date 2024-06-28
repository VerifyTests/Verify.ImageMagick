#if DEBUG

[TestFixture]
public class PasswordSamples
{
    [Test]
    public Task PasswordSample() =>
        VerifyFile("password.pdf")
            .ImageMagickPdfPassword("password");
}

#endif