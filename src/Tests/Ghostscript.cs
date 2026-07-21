static class Ghostscript
{
    // ImageMagick shells out to Ghostscript (gswin64c.exe on windows, gs elsewhere) to render pdfs.
    // With no Ghostscript installed, that command fails with exit code 127 (command not found).
    // Replaces the pdf converter registered by Initialize with one that treats that as inconclusive,
    // so pdf tests do not fail on machines that do not have Ghostscript installed.
    public static void RegisterPdfConverter() =>
        VerifierSettings.RegisterStreamConverter("pdf", Convert);

    static ConversionResult Convert(string? name, Stream stream, IReadOnlyDictionary<string, object> context)
    {
        try
        {
            return VerifyImageMagick.Convert(name, stream, context, MagickFormat.Pdf);
        }
        catch (MagickDelegateErrorException exception)
            when (exception.Message.Contains("(127)"))
        {
            throw new InconclusiveException(
                $"""
                 Ghostscript is required to convert pdfs, and it is not installed.
                 https://ghostscript.com/releases/gsdnld.html
                 {exception.Message}
                 """);
        }
    }
}
