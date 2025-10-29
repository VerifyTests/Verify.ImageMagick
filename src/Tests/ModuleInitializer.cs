public static class ModuleInitializer
{
    #region enable

    [ModuleInitializer]
    public static void Init()
    {
        VerifyImageMagick.Initialize();
        VerifyImageMagick.RegisterComparers(threshold: 0.5);
    }

    #endregion

    [ModuleInitializer]
    public static void InitOther() =>
        VerifierSettings.InitializePlugins();
}