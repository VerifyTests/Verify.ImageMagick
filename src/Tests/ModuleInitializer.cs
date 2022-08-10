public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyImageMagick.Initialize();
        VerifyImageMagick.RegisterComparers(0.05);
    }
}