# <img src="/src/icon.png" height="30px"> Verify.ImageMagick

[![Build status](https://ci.appveyor.com/api/projects/status/ersj3ag6pitygha5?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify-ImageMagick)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.ImageMagick.svg)](https://www.nuget.org/packages/Verify.ImageMagick/)

Extends [Verify](https://github.com/SimonCropp/Verify) to allow verification of documents via [Magick.NET](https://github.com/dlemstra/Magick.NET).

Converts documents pdfs to png for verification.

Contains [comparers](https://github.com/SimonCropp/Verify/blob/master/docs/comparer.md) for png, jpg, bmp, and tiff.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify.aspose?utm_source=nuget-verify.aspose&utm_medium=referral&utm_campaign=enterprise).

toc


## NuGet package

https://nuget.org/packages/Verify.ImageMagick/


## Usage

Given a test with the following definition:

snippet: TestDefinition

`Initialize` registers the pdf to png converter an all comparers.


### PDF converter

To register only the pdf to png converter:

```
VerifyImageMagick.RegisterPdfToPngConverter();
```


#### Verify a file

snippet: VerifyPdf


#### Verify a Stream

snippet: VerifyPdfStream


#### Result

[Samples.VerifyPdf.00.verified.png](/src/Tests/Samples.VerifyPdf.00.verified.png):

<img src="/src/Tests/Samples.VerifyPdf.00.verified.png" width="200px">


### Comparers

Register all comparers

```
VerifyImageMagick.RegisterComparers();
```


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Swirl](https://thenounproject.com/term/wizard/2744075/) designed by [Philipp Petzka](https://thenounproject.com/masteroficon) from [The Noun Project](https://thenounproject.com/).