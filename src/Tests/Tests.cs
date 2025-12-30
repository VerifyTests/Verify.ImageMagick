
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

    // https://github.com/VerifyTests/Verify.ImageMagick/issues/754
    // Regression test: Ensure image comparisons correctly detect differences
    // regardless of the threshold value (as long as diff > threshold)
    [Test]
    public async Task Issue754_DifferentImagesShouldNotBeEqual()
    {
        // Simulate UI screenshot comparison - white background with different text positions
        using var image1 = new MagickImage(MagickColors.White, 200, 100);
        using var text1 = new MagickImage(MagickColors.Black, 80, 20);
        image1.Composite(text1, 10, 40, CompositeOperator.Over); // Text on left

        using var image2 = new MagickImage(MagickColors.White, 200, 100);
        using var text2 = new MagickImage(MagickColors.Black, 80, 20);
        image2.Composite(text2, 110, 40, CompositeOperator.Over); // Text on right

        var stream1 = new MemoryStream();
        var stream2 = new MemoryStream();
        image1.Write(stream1, MagickFormat.Png);
        image2.Write(stream2, MagickFormat.Png);
        stream1.Position = 0;
        stream2.Position = 0;

        // Get raw diff to understand the magnitude
        using var img1 = new MagickImage(stream1);
        using var img2 = new MagickImage(stream2);
        var rawDiff = img1.Compare(img2, ErrorMetric.Fuzz);

        stream1.Position = 0;
        stream2.Position = 0;

        // With an extremely small threshold, different images MUST be detected
        var threshold = 0.00000000000001;
        var compare = await VerifyImageMagick.Compare(threshold, ErrorMetric.Fuzz, stream1, stream2);

        Assert.That(compare.IsEqual, Is.False,
            $"Different images (diff={rawDiff}) should be detected with threshold={threshold}");
    }

    // https://github.com/VerifyTests/Verify.ImageMagick/issues/754
    // Test that threshold=0 and tiny threshold behave consistently
    [Test]
    public async Task Issue754_ZeroVsNonZeroThreshold()
    {
        using var image1 = new MagickImage(MagickColors.White, 100, 100);
        using var image2 = new MagickImage(MagickColors.White, 100, 100);

        // Add a visible difference - 10x10 black square
        using var diff = new MagickImage(MagickColors.Black, 10, 10);
        image2.Composite(diff, 45, 45, CompositeOperator.Over);

        var stream1 = new MemoryStream();
        var stream2 = new MemoryStream();
        image1.Write(stream1, MagickFormat.Png);
        image2.Write(stream2, MagickFormat.Png);

        // Compare with threshold = 0
        stream1.Position = 0;
        stream2.Position = 0;
        var compareZero = await VerifyImageMagick.Compare(0, ErrorMetric.Fuzz, stream1, stream2);

        // Compare with tiny non-zero threshold
        stream1.Position = 0;
        stream2.Position = 0;
        var compareTiny = await VerifyImageMagick.Compare(0.00000000000001, ErrorMetric.Fuzz, stream1, stream2);

        // Both should detect the difference
        Assert.Multiple(() =>
        {
            Assert.That(compareZero.IsEqual, Is.False, "Threshold=0 should detect difference");
            Assert.That(compareTiny.IsEqual, Is.False, "Tiny threshold should detect difference");
        });
    }

    // https://github.com/VerifyTests/Verify.ImageMagick/issues/754
    // Verify RootMeanSquared works as a reliable alternative
    [Test]
    public async Task Issue754_RootMeanSquaredAsAlternative()
    {
        using var image1 = new MagickImage(MagickColors.White, 100, 100);
        using var block1 = new MagickImage(MagickColors.Gray, 20, 20);
        image1.Composite(block1, 10, 10, CompositeOperator.Over);

        using var image2 = new MagickImage(MagickColors.White, 100, 100);
        using var block2 = new MagickImage(MagickColors.Gray, 20, 20);
        image2.Composite(block2, 70, 70, CompositeOperator.Over);

        var stream1 = new MemoryStream();
        var stream2 = new MemoryStream();
        image1.Write(stream1, MagickFormat.Png);
        image2.Write(stream2, MagickFormat.Png);
        stream1.Position = 0;
        stream2.Position = 0;

        var threshold = 0.00000000000001;
        var compare = await VerifyImageMagick.Compare(threshold, ErrorMetric.RootMeanSquared, stream1, stream2);

        Assert.That(compare.IsEqual, Is.False,
            "RootMeanSquared should detect positional differences");
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