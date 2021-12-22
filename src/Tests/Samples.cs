[TestFixture]
public class Samples
{
    #region CompareImage

    [Test]
    public Task CompareImage()
    {
        return VerifyFile("sample.jpg");
    }

    #endregion

    #region VerifyPdf

    [Test]
    public Task VerifyPdf()
    {
        return VerifyFile("sample.pdf");
    }

    #endregion

    #region VerifyPdfStream

    [Test]
    public Task VerifyPdfStream()
    {
        return Verify(File.OpenRead("sample.pdf"))
            .UseExtension("pdf");
    }

    #endregion
}