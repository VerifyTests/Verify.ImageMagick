#if DEBUG

public class TransparentSamples
{
    [Test]
    [Arguments("png")]
    [Arguments("svg")]
    [Arguments("pdf")]
    public Task TransparentNone(string format) =>
        VerifyFile($"transparent.{format}");

    [Test]
    [Arguments("png", Color.Transparent)]
    [Arguments("png", Color.Green)]
    [Arguments("png", Color.Blue)]
    [Arguments("svg", Color.Transparent)]
    [Arguments("svg", Color.Green)]
    [Arguments("svg", Color.Blue)]
    [Arguments("pdf", Color.Transparent)]
    [Arguments("pdf", Color.Green)]
    [Arguments("pdf", Color.Blue)]
    public Task TransparentSample(string format, Color backgroundColor) =>
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