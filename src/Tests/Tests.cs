[TestFixture]
public class Tests
{
    [Test]
    public Task FailingCompare()
    {
        return Verifier.ThrowsTask(async () =>
        {
            await Verifier.VerifyFile("sample.jpg")
                .DisableDiff()
                .UseMethodName("FailingCompareInner")
                .ImageMagickComparer(.0001);
        });
    }
}