
[TestFixture]
[SetCulture("en-US")]
public class Tests
{
#if Debug
    [Test]
    public Task FailingCompare() =>
        ThrowsTask(async () =>
            {
                await VerifyFile("sample.jpg")
                    .DisableDiff()
                    .UseMethodName("FailingCompareInner")
                    .ImageMagickComparer(.0001);
            })
            .IgnoreStackTrace()
            .ScrubLinesContaining("clipboard", "DiffEngineTray");
#endif

    [Test]
    public Task CompareSame()
    {
        var compare = VerifyImageMagick.Compare(
            .0001,
            ErrorMetric.Fuzz,
            File.OpenRead("sample.jpg"),
            File.OpenRead("sample.jpg"));
        return Verify(compare);
    }

    [Test]
    public Task ShouldNotMessWithTargetName() =>
        Verify(new Target("png", File.OpenRead("sample.jpg"), "name"));

    [Test]
    public Task ShouldNotMessWithTargetNames() =>
        Verify(
            targets:
            [
                new("png", File.OpenRead("sample.jpg"), "name1"),
                new("png", File.OpenRead("sample.jpg"), "name2")
            ]);

    [Test]
    public Task CompareDifferent()
    {
        var compare = VerifyImageMagick.Compare(
            .0001,
            ErrorMetric.Fuzz,
            File.OpenRead("sample.jpg"),
            File.OpenRead("sample.png"));
        return Verify(compare);
    }

    [Test]
    public Task VerifyPdf() =>
        VerifyFile("sample.pdf");

    [Test]
    public Task VerifyWebp() =>
        VerifyFile("sample.webp");

    [Test]
    public Task VerifyPdfWithName() =>
        Verify(targets: [new("pdf", File.OpenRead("sample.pdf"), "name")]);
}