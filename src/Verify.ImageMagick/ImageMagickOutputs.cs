namespace VerifyTests;

/// <summary>
/// Controls which outputs a document (pdf or svg) is split into.
/// </summary>
[Flags]
public enum ImageMagickOutputs
{
    /// <summary>
    /// Render pdf pages and svg documents to png.
    /// </summary>
    Png = 1,

    /// <summary>
    /// All outputs.
    /// </summary>
    All = Png
}
