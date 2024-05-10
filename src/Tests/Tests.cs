
using ImageMagick;

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
    public Task CompareDifferent()
    {
        var compare = VerifyImageMagick.Compare(
            .0001,
            ErrorMetric.Fuzz,
            File.OpenRead("sample.jpg"),
            File.OpenRead("sample.png"));
        return Verify(compare);
    }
}