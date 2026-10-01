public static class ModuleInitializer
{
    #region InitializeOutputs

    [ModuleInitializer]
    public static void Init() =>
        // Only emit the source document. Skip rendering to png.
        VerifyImageMagick.Initialize(ImageMagickOutputs.None);

    #endregion
}
