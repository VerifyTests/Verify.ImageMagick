#if DEBUG

[TestFixture]
public class TransparentSamples
{
    [Test]
    public Task TransparentNone(
        [Values("png", "svg", "pdf")] string format) =>
        VerifyFile($"transparent.{format}");

    [Test]
    public Task TransparentSample(
        [Values("png", "svg", "pdf")] string format,
        [Values] Color backgroundColor) =>
        VerifyFile($"transparent.{format}")
            .ImageMagickBackground(Map(backgroundColor));

    static MagickColor Map(Color color) => color switch
    {
        Color.Transparent => MagickColors.Transparent,
        Color.Green => MagickColors.Green,
        Color.Blue => MagickColors.Blue,
        _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
    };

    public enum Color
    {
        Transparent,
        Green,
        Blue
    }
}

#endif