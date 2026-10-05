public static class ModuleInitializer
{
    #region InitializeOutputs

    [ModuleInitializer]
    public static void Init()
    {
        VerifyImageMagick.Initialize();

        // For every test: no png, so only the source document is verified
        VerifierSettings.ExcludeDerivedTargets("png");
    }

    #endregion
}
