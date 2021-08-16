﻿using System.IO;
using System.Threading.Tasks;
using VerifyNUnit;
using NUnit.Framework;

[TestFixture]
public class Samples
{
    #region CompareImage

    [Test]
    public Task CompareImage()
    {
        return Verifier.VerifyFile("sample.jpg");
    }

    #endregion

    #region VerifyPdf

    [Test]
    public Task VerifyPdf()
    {
        return Verifier.VerifyFile("sample.pdf");
    }

    #endregion

    #region VerifyPdfStream

    [Test]
    public Task VerifyPdfStream()
    {
        return Verifier.Verify(File.OpenRead("sample.pdf"))
            .UseExtension("pdf");
    }

    #endregion
}