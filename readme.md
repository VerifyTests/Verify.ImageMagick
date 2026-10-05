# <img src="/src/icon.png" height="30px"> Verify.ImageMagick

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://github.com/VerifyTests/Verify.ImageMagick/actions/workflows/build.yml/badge.svg)](https://github.com/VerifyTests/Verify.ImageMagick/actions/workflows/build.yml)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.ImageMagick.svg)](https://www.nuget.org/packages/Verify.ImageMagick/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of documents via [Magick.NET](https://github.com/dlemstra/Magick.NET).<!-- singleLineInclude: intro. path: /docs/intro.include.md -->

**See [Milestones](../../milestones?state=closed) for release notes.**

Converts documents pdfs to png for verification.

Verifying a `pdf` produces:

 * The pdf itself as `.verified.pdf`. This can be omitted with [`ExcludeTargets`](#choosing-what-is-verified).
 * A `.verified.txt` with the page count.
 * A png render of every page as `#page_0001.verified.png`, `#page_0002.verified.png`, etc.

Verifying an `svg` produces the svg as `.verified.svg` and a png render of it as `.verified.png`.

The page files are named by Verify's [paged documents](https://github.com/VerifyTests/Verify/blob/main/docs/paged-documents.md) support, which every Verify plugin that splits a document into pages shares. So do the settings that [choose what is verified](#choosing-what-is-verified).

Contains [comparers](https://github.com/VerifyTests/Verify/blob/master/docs/comparer.md) for png, jpg, bmp, and tiff.


## Sponsors


### Entity Framework Extensions<!-- include: sponsors. path: /docs/sponsors.include.md -->

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=simoncropp&utm_medium=Verify.ImageMagick) is a major sponsor and is proud to contribute to the development this project.

[![Entity Framework Extensions](https://raw.githubusercontent.com/VerifyTests/Verify.ImageMagick/refs/heads/main/docs/zzz.png)](https://entityframework-extensions.net/?utm_source=simoncropp&utm_medium=Verify.ImageMagick)

### Developed using JetBrains IDEs

[![JetBrains logo.](https://raw.githubusercontent.com/VerifyTests/Verify.ImageMagick/main/docs/jetbrains.png)](https://jb.gg/OpenSourceSupport)<!-- endInclude -->


## NuGet

 * https://nuget.org/packages/Verify.ImageMagick


## Usage

<!-- snippet: enable -->
<a id='snippet-enable'></a>
```cs
[ModuleInitializer]
public static void Init()
{
    VerifyImageMagick.Initialize();
    VerifyImageMagick.RegisterComparers(threshold: 0.5);
}
```
<sup><a href='/src/Tests/ModuleInitializer.cs#L3-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-enable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

`Initialize` registers the pdf to png converter and all comparers.


### Choosing what is verified

What a pdf or an svg is split into is controlled by Verify's settings for [paged documents](https://github.com/VerifyTests/Verify/blob/main/docs/paged-documents.md), which replace the options this plugin had of its own.

`PagesToInclude` limits the pages of a pdf that are verified, to its first pages or to those a delegate accepts. The pdf itself is still verified whole, and the info file still holds the number of pages it has:

<!-- snippet: PagesToInclude -->
<a id='snippet-PagesToInclude'></a>
```cs
[Test]
public Task PagesToInclude() =>
    VerifyFile("sample.pdf")
        .PagesToInclude(1);
```
<sup><a href='/src/Tests/Samples.cs#L51-L58' title='Snippet source file'>snippet source</a> | <a href='#snippet-PagesToInclude' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

`ExcludeDerivedTargets("png")` leaves out the png renders, keeping the pdf or the svg. The render is skipped rather than dropped afterwards. The pages of a pdf are then only counted, so its info file still says how many there are:

<!-- snippet: ExcludeDerivedTargets -->
<a id='snippet-ExcludeDerivedTargets'></a>
```cs
[Test]
public Task ExcludeDerivedTargets() =>
    VerifyFile("sample.pdf")
        .ExcludeDerivedTargets("png");
```
<sup><a href='/src/Tests/Samples.cs#L60-L67' title='Snippet source file'>snippet source</a> | <a href='#snippet-ExcludeDerivedTargets' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

It applies to a png rendered from a pdf or an svg. A png, webp or tiff that is itself being verified is not affected.

`ExcludeTargets("pdf")` leaves out the pdf, keeping its pages and its info file. A [password-protected pdf](#open-password-protected-pdfs) requires it.

Each can also be set for every test, on `VerifierSettings`:

<!-- snippet: InitializeOutputs -->
<a id='snippet-InitializeOutputs'></a>
```cs
[ModuleInitializer]
public static void Init()
{
    VerifyImageMagick.Initialize();

    // For every test: no png, so only the source document is verified
    VerifierSettings.ExcludeDerivedTargets("png");
}
```
<sup><a href='/src/StaticSettingsTests/ModuleInitializer.cs#L3-L14' title='Snippet source file'>snippet source</a> | <a href='#snippet-InitializeOutputs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### PDF converter

To register only the pdf to png converter:

```
VerifyImageMagick.RegisterPdfToPngConverter();
```


#### Verify a file

<!-- snippet: VerifyPdf -->
<a id='snippet-VerifyPdf'></a>
```cs
[Test]
public Task VerifyPdf() =>
    VerifyFile("sample.pdf");
```
<sup><a href='/src/Tests/Samples.cs#L32-L38' title='Snippet source file'>snippet source</a> | <a href='#snippet-VerifyPdf' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Verify a Stream

<!-- snippet: VerifyPdfStream -->
<a id='snippet-VerifyPdfStream'></a>
```cs
[Test]
public Task VerifyPdfStream()
{
    var stream = new MemoryStream(File.ReadAllBytes("sample.pdf"));
    return Verify(stream, "pdf");
}
```
<sup><a href='/src/Tests/Samples.cs#L40-L49' title='Snippet source file'>snippet source</a> | <a href='#snippet-VerifyPdfStream' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Result

[Samples.VerifyPdf.verified.txt](/src/Tests/Samples.VerifyPdf.verified.txt):

<!-- snippet: Samples.VerifyPdf.verified.txt -->
<a id='snippet-Samples.VerifyPdf.verified.txt'></a>
```txt
{
  PageCount: 2
}
```
<sup><a href='/src/Tests/Samples.VerifyPdf.verified.txt#L1-L3' title='Snippet source file'>snippet source</a> | <a href='#snippet-Samples.VerifyPdf.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

[Samples.VerifyPdf#page_0001.verified.png](/src/Tests/Samples.VerifyPdf%23page_0001.verified.png):

<img src="/src/Tests/Samples.VerifyPdf%23page_0001.verified.png" width="200px">


### Image Comparers

The following will use ImageMagick to compare the images instead of the default binary comparison.

<!-- snippet: CompareImage -->
<a id='snippet-CompareImage'></a>
```cs
[Test]
public Task CompareImage() =>
    VerifyFile("sample.jpg");
```
<sup><a href='/src/Tests/Samples.cs#L5-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-CompareImage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Register all comparers

All comparers can be registered:

```
VerifyImageMagick.RegisterComparers();
```


### Override transparent background

For images with a transparent background, that background can be overridden:

<!-- snippet: BackgroundColor -->
<a id='snippet-BackgroundColor'></a>
```cs
[Test]
public Task BackgroundColor() =>
    VerifyFile("transparent.png")
        .ImageMagickBackground(MagickColors.Blue);
```
<sup><a href='/src/Tests/Samples.cs#L13-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-BackgroundColor' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->



### Open password-protected PDFs

For password-protected PDF, the password can be provided to allow verification:

<!-- snippet: PdfPassword -->
<a id='snippet-PdfPassword'></a>
```cs
[Test]
public Task PdfPassword() =>
    VerifyFile("password.pdf")
        .ImageMagickPdfPassword("password")
        .ExcludeTargets("pdf");
```
<sup><a href='/src/Tests/Samples.cs#L22-L30' title='Snippet source file'>snippet source</a> | <a href='#snippet-PdfPassword' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Reviewing changes

A change to a pdf is a change to several files: the pdf, its info file, and every page. Verify tells the diff tool that the pages and the info file were derived from the pdf, and [DiffEngineViewer](https://github.com/VerifyTests/DiffEngine/blob/main/docs/viewer.md#files-derived-from-a-document), which draws a pdf's pages itself, shows them as one row and accepts them together. Other diff tools are given each file, as before. The png of an svg is derived from the svg in the same way.

When the pdf or the svg has itself changed, what was derived from it is compared exactly, skipping any [comparer](#image-comparers) registered for png. ImageMagick writes the time into every png it renders, so every page of a changed pdf is then reported as changed, whether or not it draws differently.


## Migrating from 3.x

Version 4 moves to the paged document support in Verify 33.3. The options of this plugin that chose what was verified are now settings of Verify:

| 3.x | 4.x |
| --- | --- |
| `Initialize(ImageMagickOutputs.None)` | `Initialize()` and `VerifierSettings.ExcludeDerivedTargets("png")` |
| `Initialize(ImageMagickOutputs.Png)` or `Initialize(ImageMagickOutputs.All)` | `Initialize()` |
| `.PagesToInclude(2)`, an extension method of this plugin | `.PagesToInclude(2)`, a member of Verify's `SettingsTask` and `VerifySettings` |

A call to `PagesToInclude` compiles unchanged. It can now also take a delegate, and both it and `ExcludeDerivedTargets` can be set for one verification or, on `VerifierSettings`, for all of them.

The page of a pdf is named by its number, which is 1 based. It was named by an index, which was 0 based and missing for a pdf with one page. A pdf also has an info file now, holding its page count:

| 3.x | 4.x |
| --- | --- |
| `Tests.Report#00.verified.png` | `Tests.Report#page_0001.verified.png` |
| `Tests.Report#01.verified.png` | `Tests.Report#page_0002.verified.png` |
| `Tests.Report.verified.png`, of a pdf with one page | `Tests.Report#page_0001.verified.png` |
| `Tests.Mail#Attachment.00.verified.png`, of a pdf that is a target named `Attachment` | `Tests.Mail#Attachment.page_0001.verified.png` |
| | `Tests.Report.verified.txt`, the info file |

The `.verified.pdf` is unchanged, and so are the `.verified.svg` and `.verified.png` of an svg.

Renamed snapshots show as a new file and a pending delete. Accepting both, or running once with [AutoVerify](https://github.com/VerifyTests/Verify/blob/main/docs/autoverify.md), moves a test over. A page draws as it did, but ImageMagick writes the time into every png it renders, so an accepted page differs from the file it replaces by those bytes, and source control shows a rename with a small change.


## Icon

[Swirl](https://thenounproject.com/term/wizard/2744075/) designed by [Philipp Petzka](https://thenounproject.com/masteroficon) from [The Noun Project](https://thenounproject.com/).
