# <img src="/src/icon.png" height="30px"> Verify.ImageMagick

[![Build status](https://ci.appveyor.com/api/projects/status/ersj3ag6pitygha5?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-ImageMagick)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.ImageMagick.svg)](https://www.nuget.org/packages/Verify.ImageMagick/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of documents via [Magick.NET](https://github.com/dlemstra/Magick.NET).

Converts documents pdfs to png for verification.

Contains [comparers](https://github.com/VerifyTests/Verify/blob/master/docs/comparer.md) for png, jpg, bmp, and tiff.

<a href='https://dotnetfoundation.org' alt='Part of the .NET Foundation'><img src='https://raw.githubusercontent.com/VerifyTests/Verify/master/docs/dotNetFoundation.svg' height='30px'></a><br>
Part of the <a href='https://dotnetfoundation.org' alt=''>.NET Foundation</a>



## NuGet package

https://nuget.org/packages/Verify.ImageMagick/


## Usage

Given a test with the following definition:

<!-- snippet: TestDefinition -->
<a id='snippet-testdefinition'></a>
```cs
[TestFixture]
public class Samples
{
    static Samples()
    {
        VerifyImageMagick.Initialize();
    }
```
<sup><a href='/src/Tests/Samples.cs#L7-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-testdefinition' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

`Initialize` registers the pdf to png converter an all comparers.


### PDF converter

To register only the pdf to png converter:

```
VerifyImageMagick.RegisterPdfToPngConverter();
```


#### Verify a file

<!-- snippet: VerifyPdf -->
<a id='snippet-verifypdf'></a>
```cs
[Test]
public Task VerifyPdf()
{
    return Verifier.VerifyFile("sample.pdf");
}
```
<sup><a href='/src/Tests/Samples.cs#L18-L26' title='Snippet source file'>snippet source</a> | <a href='#snippet-verifypdf' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Verify a Stream

<!-- snippet: VerifyPdfStream -->
<a id='snippet-verifypdfstream'></a>
```cs
[Test]
public Task VerifyPdfStream()
{
    return Verifier.Verify(File.OpenRead("sample.pdf"))
        .UseExtension("pdf");
}
```
<sup><a href='/src/Tests/Samples.cs#L28-L37' title='Snippet source file'>snippet source</a> | <a href='#snippet-verifypdfstream' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Result

[Samples.VerifyPdf.00.verified.png](/src/Tests/Samples.VerifyPdf.00.verified.png):

<img src="/src/Tests/Samples.VerifyPdf.00.verified.png" width="200px">


### Comparers

Register all comparers

```
VerifyImageMagick.RegisterComparers();
```



## Icon

[Swirl](https://thenounproject.com/term/wizard/2744075/) designed by [Philipp Petzka](https://thenounproject.com/masteroficon) from [The Noun Project](https://thenounproject.com/).
