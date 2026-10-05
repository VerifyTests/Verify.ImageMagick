namespace VerifyTests;

/// <summary>
/// What <c>Initialize</c> used to be given to choose the outputs a pdf or an svg is split into. Settings
/// of Verify choose that now, and nothing reads this: it is here so that code still naming it is
/// told what to use in its place.
/// </summary>
[Obsolete(
    "ImageMagickOutputs and the outputs argument of Initialize are replaced by settings of Verify: VerifierSettings.ExcludeDerivedTargets(\"png\") to leave out the png images. See https://github.com/VerifyTests/Verify.ImageMagick#migrating-from-3x",
    true)]
[Flags]
public enum ImageMagickOutputs
{
    /// <summary>
    /// No outputs. Only the source document is emitted.
    /// </summary>
    None = 0,

    /// <summary>
    /// Render pdf pages and svg documents to png.
    /// </summary>
    Png = 1,

    /// <summary>
    /// All outputs.
    /// </summary>
    All = Png
}
