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

Enable:

<!-- snippet: ModuleInitializer.cs -->
<a id='snippet-ModuleInitializer.cs'></a>
```cs
using VerifyTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyImageMagick.Initialize();
    }
}
```
<sup><a href='/src/Tests/ModuleInitializer.cs#L1-L10' title='Snippet source file'>snippet source</a> | <a href='#snippet-ModuleInitializer.cs' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/src/Tests/Samples.cs#L17-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-verifypdf' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/src/Tests/Samples.cs#L27-L36' title='Snippet source file'>snippet source</a> | <a href='#snippet-verifypdfstream' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Result

[Samples.VerifyPdf.00.verified.png](/src/Tests/Samples.VerifyPdf.00.verified.png):

<img src="/src/Tests/Samples.VerifyPdf.00.verified.png" width="200px">


### Image Comparers

The following will use ImageMagick to compare the images instead of the default binary comparison.

<!-- snippet: CompareImage -->
<a id='snippet-compareimage'></a>
```cs
[Test]
public Task CompareImage()
{
    return Verifier.VerifyFile("sample.jpg");
}
```
<sup><a href='/src/Tests/Samples.cs#L7-L15' title='Snippet source file'>snippet source</a> | <a href='#snippet-compareimage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Register all comparers

All comparers can be regietered:

```
VerifyImageMagick.RegisterComparers();
```



## Icon

[Swirl](https://thenounproject.com/term/wizard/2744075/) designed by [Philipp Petzka](https://thenounproject.com/masteroficon) from [The Noun Project](https://thenounproject.com/).
