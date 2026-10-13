using System;
using System.IO;

using NUnit.Framework;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using NuciXNA.DataAccess.IO;
using NuciXNA.Primitives;

namespace NuciXNA.DataAccess.UnitTests.IO
{
    [TestFixture]
    public sealed class BitmapTests
    {
        private static Colour TransparentBlack => Colour.FromArgb(0, 0, 0, 0);

        private static Colour OpaqueRed => Colour.FromArgb(255, 255, 0, 0);

        private static Colour OpaqueGreen => Colour.FromArgb(255, 0, 255, 0);

        private static Colour OpaqueBlue => Colour.FromArgb(255, 0, 0, 255);

        private static Colour OpaqueWhite => Colour.FromArgb(255, 255, 255, 255);

        private static Colour SemiTransparentPurple => Colour.FromArgb(128, 64, 0, 128);

        private static Colour OpaqueYellow => Colour.FromArgb(255, 255, 255, 0);

        private Bitmap subject;

        [SetUp]
        public void SetUp()
        {
            subject = new Bitmap(8, 8);
        }

        [TearDown]
        public void TearDown()
        {
            subject?.Dispose();
        }

        // Constructor — int, int

        [TestCase(4, 8)]
        [TestCase(8, 4)]
        [TestCase(16, 32)]
        [TestCase(32, 16)]
        [TestCase(64, 128)]
        [TestCase(128, 64)]
        [TestCase(1, 1)]
        [TestCase(96, 48)]
        [TestCase(42, 42)]
        public void GivenWidthAndHeight_WhenBitmapIsCreated_ThenSizeWidthMatchesWidth(int width, int height)
        {
            subject.Dispose();
            subject = new Bitmap(width, height);

            Assert.That(
                subject.Size.Width,
                Is.EqualTo(width));
        }

        [TestCase(4, 8)]
        [TestCase(8, 4)]
        [TestCase(16, 32)]
        [TestCase(32, 16)]
        [TestCase(64, 128)]
        [TestCase(128, 64)]
        [TestCase(1, 1)]
        [TestCase(96, 48)]
        [TestCase(42, 42)]
        public void GivenWidthAndHeight_WhenBitmapIsCreated_ThenSizeHeightMatchesHeight(int width, int height)
        {
            subject.Dispose();
            subject = new Bitmap(width, height);

            Assert.That(
                subject.Size.Height,
                Is.EqualTo(height));
        }

        // Constructor — Size2D

        [TestCase(4, 8)]
        [TestCase(16, 32)]
        [TestCase(64, 128)]
        [TestCase(1, 1)]
        [TestCase(96, 48)]
        public void GivenSize2D_WhenBitmapIsCreated_ThenSizeWidthMatchesSize(int width, int height)
        {
            subject.Dispose();
            subject = new Bitmap(new Size2D(width, height));

            Assert.That(
                subject.Size.Width,
                Is.EqualTo(width));
        }

        [TestCase(4, 8)]
        [TestCase(16, 32)]
        [TestCase(64, 128)]
        [TestCase(1, 1)]
        [TestCase(96, 48)]
        public void GivenSize2D_WhenBitmapIsCreated_ThenSizeHeightMatchesSize(int width, int height)
        {
            subject.Dispose();
            subject = new Bitmap(new Size2D(width, height));

            Assert.That(
                subject.Size.Height,
                Is.EqualTo(height));
        }

        [TestCase(4, 8)]
        [TestCase(16, 32)]
        [TestCase(64, 128)]
        [TestCase(96, 48)]
        public void GivenSize2D_WhenBitmapIsCreated_ThenSizeMatchesSize2DWidth(int width, int height)
        {
            Size2D size = new(width, height);

            subject.Dispose();
            subject = new Bitmap(size);

            Assert.That(
                subject.Size.Width,
                Is.EqualTo(size.Width));
        }

        [TestCase(4, 8)]
        [TestCase(16, 32)]
        [TestCase(64, 128)]
        [TestCase(96, 48)]
        public void GivenSize2D_WhenBitmapIsCreated_ThenSizeMatchesSize2DHeight(int width, int height)
        {
            Size2D size = new(width, height);

            subject.Dispose();
            subject = new Bitmap(size);

            Assert.That(
                subject.Size.Height,
                Is.EqualTo(size.Height));
        }

        // Constructor — Image<Rgba32>

        [TestCase(4, 8)]
        [TestCase(32, 16)]
        [TestCase(64, 128)]
        [TestCase(1, 1)]
        [TestCase(96, 48)]
        public void GivenImage_WhenBitmapIsCreated_ThenSizeWidthMatchesImageWidth(int width, int height)
        {
            using Image<Rgba32> image = new(width, height);

            subject.Dispose();
            subject = new Bitmap(image);

            Assert.That(
                subject.Size.Width,
                Is.EqualTo(width));
        }

        [TestCase(4, 8)]
        [TestCase(32, 16)]
        [TestCase(64, 128)]
        [TestCase(1, 1)]
        [TestCase(96, 48)]
        public void GivenImage_WhenBitmapIsCreated_ThenSizeHeightMatchesImageHeight(int width, int height)
        {
            using Image<Rgba32> image = new(width, height);

            subject.Dispose();
            subject = new Bitmap(image);

            Assert.That(
                subject.Size.Height,
                Is.EqualTo(height));
        }

        // GetPixel — default value on new bitmap

        [TestCase(0, 0)]
        [TestCase(0, 7)]
        [TestCase(7, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        public void GivenNewBitmap_WhenGetPixelIsCalled_ThenTransparentBlackIsReturned(int x, int y)
            => Assert.That(
                subject.GetPixel(x, y),
                Is.EqualTo(TransparentBlack));

        [TestCase(0, 0)]
        [TestCase(4, 4)]
        [TestCase(7, 7)]
        public void GivenNewBitmap_WhenGetPixelWithPoint2DIsCalled_ThenTransparentBlackIsReturned(int x, int y)
            => Assert.That(
                subject.GetPixel(new Point2D(x, y)),
                Is.EqualTo(TransparentBlack));

        // SetPixel / GetPixel — round trips with coordinates

        [Test]
        public void GivenOpaqueRedPixelSetAtOrigin_WhenGetPixelIsCalled_ThenOpaqueRedIsReturned()
        {
            subject.SetPixel(0, 0, OpaqueRed);

            Assert.That(
                subject.GetPixel(0, 0),
                Is.EqualTo(OpaqueRed));
        }

        [Test]
        public void GivenOpaqueGreenPixelSetAtOrigin_WhenGetPixelIsCalled_ThenOpaqueGreenIsReturned()
        {
            subject.SetPixel(0, 0, OpaqueGreen);

            Assert.That(
                subject.GetPixel(0, 0),
                Is.EqualTo(OpaqueGreen));
        }

        [Test]
        public void GivenOpaqueBluePixelSetAtOrigin_WhenGetPixelIsCalled_ThenOpaqueBlueIsReturned()
        {
            subject.SetPixel(0, 0, OpaqueBlue);

            Assert.That(
                subject.GetPixel(0, 0),
                Is.EqualTo(OpaqueBlue));
        }

        [Test]
        public void GivenOpaqueWhitePixelSetAtOrigin_WhenGetPixelIsCalled_ThenOpaqueWhiteIsReturned()
        {
            subject.SetPixel(0, 0, OpaqueWhite);

            Assert.That(
                subject.GetPixel(0, 0),
                Is.EqualTo(OpaqueWhite));
        }

        [Test]
        public void GivenSemiTransparentPurplePixelSetAtOrigin_WhenGetPixelIsCalled_ThenSemiTransparentPurpleIsReturned()
        {
            subject.SetPixel(0, 0, SemiTransparentPurple);

            Assert.That(
                subject.GetPixel(0, 0),
                Is.EqualTo(SemiTransparentPurple));
        }

        [TestCase(0, 0)]
        [TestCase(0, 7)]
        [TestCase(7, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        [TestCase(3, 5)]
        public void GivenPixelSetAtCoordinates_WhenGetPixelIsCalled_ThenSetColourIsReturned(int x, int y)
        {
            subject.SetPixel(x, y, OpaqueRed);

            Assert.That(
                subject.GetPixel(x, y),
                Is.EqualTo(OpaqueRed));
        }

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        public void GivenPixelSetTwiceAtSameCoordinates_WhenGetPixelIsCalled_ThenSecondColourIsReturned(int x, int y)
        {
            subject.SetPixel(x, y, OpaqueRed);
            subject.SetPixel(x, y, OpaqueBlue);

            Assert.That(
                subject.GetPixel(x, y),
                Is.EqualTo(OpaqueBlue));
        }

        [Test]
        public void GivenDifferentPixelsSetAtDifferentCoordinates_WhenGetPixelIsCalledForEach_ThenCorrectColoursAreReturned()
        {
            subject.SetPixel(0, 0, OpaqueRed);
            subject.SetPixel(4, 4, OpaqueGreen);
            subject.SetPixel(7, 7, OpaqueBlue);

            Assert.That(
                subject.GetPixel(0, 0),
                Is.EqualTo(OpaqueRed));

            Assert.That(
                subject.GetPixel(4, 4),
                Is.EqualTo(OpaqueGreen));

            Assert.That(
                subject.GetPixel(7, 7),
                Is.EqualTo(OpaqueBlue));
        }

        [Test]
        public void GivenPixelSetAtOneCoordinate_WhenGetPixelIsCalledAtDifferentCoordinate_ThenTransparentBlackIsReturned()
        {
            subject.SetPixel(0, 0, OpaqueRed);

            Assert.That(
                subject.GetPixel(7, 7),
                Is.EqualTo(TransparentBlack));
        }

        // SetPixel / GetPixel — round trips with Point2D

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        [TestCase(3, 5)]
        public void GivenPixelSetWithPoint2D_WhenGetPixelIsCalled_ThenSetColourIsReturned(int x, int y)
        {
            subject.SetPixel(new Point2D(x, y), OpaqueYellow);

            Assert.That(
                subject.GetPixel(x, y),
                Is.EqualTo(OpaqueYellow));
        }

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        [TestCase(3, 5)]
        public void GivenPixelSetWithCoordinates_WhenGetPixelWithPoint2DIsCalled_ThenSetColourIsReturned(int x, int y)
        {
            subject.SetPixel(x, y, OpaqueYellow);

            Assert.That(
                subject.GetPixel(new Point2D(x, y)),
                Is.EqualTo(OpaqueYellow));
        }

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        public void GivenPixelSetWithPoint2D_WhenGetPixelWithPoint2DIsCalled_ThenSetColourIsReturned(int x, int y)
        {
            Point2D point = new(x, y);

            subject.SetPixel(point, OpaqueGreen);

            Assert.That(
                subject.GetPixel(point),
                Is.EqualTo(OpaqueGreen));
        }

        // GetPixel — Point2D is consistent with int, int

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        [TestCase(2, 6)]
        public void GivenPixelIsSet_WhenGetPixelWithPoint2DAndCoordinatesAreBothCalled_ThenBothReturnSameColour(int x, int y)
        {
            subject.SetPixel(x, y, OpaqueRed);

            Assert.That(
                subject.GetPixel(new Point2D(x, y)),
                Is.EqualTo(subject.GetPixel(x, y)));
        }

        // Indexer — int, int

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        [TestCase(3, 5)]
        public void GivenPixelSetViaIndexer_WhenGetPixelViaIndexerIsCalled_ThenSetColourIsReturned(int x, int y)
        {
            subject[x, y] = OpaqueBlue;

            Assert.That(
                subject[x, y],
                Is.EqualTo(OpaqueBlue));
        }

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        public void GivenPixelSetViaIndexer_WhenGetPixelIsCalledWithSameCoordinates_ThenSetColourIsReturned(int x, int y)
        {
            subject[x, y] = OpaqueRed;

            Assert.That(
                subject.GetPixel(x, y),
                Is.EqualTo(OpaqueRed));
        }

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        public void GivenPixelSetViaSetPixel_WhenGetPixelViaIndexerIsCalled_ThenSetColourIsReturned(int x, int y)
        {
            subject.SetPixel(x, y, OpaqueGreen);

            Assert.That(
                subject[x, y],
                Is.EqualTo(OpaqueGreen));
        }

        // Indexer — Point2D

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        [TestCase(3, 5)]
        public void GivenPixelSetViaPoint2DIndexer_WhenGetPixelViaPoint2DIndexerIsCalled_ThenSetColourIsReturned(int x, int y)
        {
            Point2D point = new(x, y);

            subject[point] = OpaqueWhite;

            Assert.That(
                subject[point],
                Is.EqualTo(OpaqueWhite));
        }

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        public void GivenPixelSetViaPoint2DIndexer_WhenGetPixelIsCalledWithSameCoordinates_ThenSetColourIsReturned(int x, int y)
        {
            subject[new Point2D(x, y)] = OpaqueRed;

            Assert.That(
                subject.GetPixel(x, y),
                Is.EqualTo(OpaqueRed));
        }

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        public void GivenPixelSetViaCoordinateIndexer_WhenGetPixelViaPoint2DIndexerIsCalled_ThenSetColourIsReturned(int x, int y)
        {
            subject[x, y] = OpaqueBlue;

            Assert.That(
                subject[new Point2D(x, y)],
                Is.EqualTo(OpaqueBlue));
        }

        // Indexer — Point2D and int, int are consistent

        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(4, 4)]
        public void GivenPixelIsSet_WhenBothIndexersAreUsedToGet_ThenBothReturnSameColour(int x, int y)
        {
            subject.SetPixel(x, y, OpaqueRed);

            Assert.That(
                subject[x, y],
                Is.EqualTo(subject[new Point2D(x, y)]));
        }

        // Dispose

        [Test]
        public void GivenBitmap_WhenDisposeIsCalled_ThenNoExceptionIsThrown()
        {
            Bitmap bitmap = new(8, 8);

            Assert.That(() => bitmap.Dispose(), Throws.Nothing);
        }

        [Test]
        public void GivenLargeBitmap_WhenDisposeIsCalled_ThenNoExceptionIsThrown()
        {
            Bitmap bitmap = new(128, 128);

            Assert.That(() => bitmap.Dispose(), Throws.Nothing);
        }

        [Test]
        public void GivenBitmapConstructedFromSize2D_WhenDisposeIsCalled_ThenNoExceptionIsThrown()
        {
            Bitmap bitmap = new(new Size2D(32, 32));

            Assert.That(() => bitmap.Dispose(), Throws.Nothing);
        }

        [Test]
        public void GivenBitmapConstructedFromImage_WhenDisposeIsCalled_ThenNoExceptionIsThrown()
        {
            using Image<Rgba32> image = new(16, 16);
            Bitmap bitmap = new(image);

            Assert.That(() => bitmap.Dispose(), Throws.Nothing);
        }

        // Dispose — multiple calls

        [Test]
        public void GivenBitmap_WhenDisposeIsCalledTwice_ThenNoExceptionIsThrown()
        {
            Bitmap bitmap = new(8, 8);

            bitmap.Dispose();
            Assert.That(() => bitmap.Dispose(), Throws.Nothing);
        }

        [Test]
        public void GivenBitmap_WhenDisposeIsCalledMultipleTimes_ThenNoExceptionIsThrown()
        {
            Bitmap bitmap = new(8, 8);

            bitmap.Dispose();
            bitmap.Dispose();
            bitmap.Dispose();

            Assert.That(() => bitmap.Dispose(), Throws.Nothing);
        }

        // Dispose — after dispose, operations throw ObjectDisposedException

        [Test]
        public void GivenDisposedBitmap_WhenGetPixelIsCalled_ThenObjectDisposedExceptionIsThrown()
        {
            subject.Dispose();

            Assert.That(() => subject.GetPixel(0, 0), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void GivenDisposedBitmap_WhenSetPixelIsCalled_ThenObjectDisposedExceptionIsThrown()
        {
            subject.Dispose();

            Assert.That(() => subject.SetPixel(0, 0, OpaqueRed), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void GivenDisposedBitmap_WhenIndexerIsUsed_ThenObjectDisposedExceptionIsThrown()
        {
            subject.Dispose();

            Assert.That(() => { var _ = subject[0, 0]; }, Throws.TypeOf<ObjectDisposedException>());
            Assert.That(() => subject[0, 0] = OpaqueRed, Throws.TypeOf<ObjectDisposedException>());
        }

        // Load — static factory method

        [Test]
        public void GivenValidPngPath_WhenLoadIsCalled_ThenBitmapIsReturned()
        {
            using var tempFile = new TempFile(".png");
            using (var image = new Image<Rgba32>(16, 16))
            {
                image[0, 0] = new Rgba32(255, 0, 0, 255);
                image.Save(tempFile.Path);
            }

            using var bitmap = Bitmap.Load(tempFile.Path);

            Assert.That(bitmap, Is.Not.Null);
            Assert.That(bitmap.Size.Width, Is.EqualTo(16));
            Assert.That(bitmap.Size.Height, Is.EqualTo(16));
        }

        [Test]
        public void GivenNonExistentPath_WhenLoadIsCalled_ThenExceptionIsThrown()
        {
            Assert.That(() => Bitmap.Load("nonexistent.png"), Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void GivenInvalidImagePath_WhenLoadIsCalled_ThenExceptionIsThrown()
        {
            using var tempFile = new TempFile(".txt");
            System.IO.File.WriteAllText(tempFile.Path, "not an image");

            Assert.That(() => Bitmap.Load(tempFile.Path), Throws.Exception);
        }

        // Save — instance method

        [Test]
        public void GivenBitmapWithPixels_WhenSaveIsCalled_ThenFileIsCreated()
        {
            subject.SetPixel(0, 0, OpaqueRed);
            subject.SetPixel(7, 7, OpaqueBlue);

            using var tempFile = new TempFile(".png");

            subject.Save(tempFile.Path);

            Assert.That(System.IO.File.Exists(tempFile.Path), Is.True);

            using var loaded = Bitmap.Load(tempFile.Path);
            Assert.That(loaded.GetPixel(0, 0), Is.EqualTo(OpaqueRed));
            Assert.That(loaded.GetPixel(7, 7), Is.EqualTo(OpaqueBlue));
        }

        [Test]
        public void GivenBitmap_WhenSaveIsCalledWithDirectory_ThenFileIsCreatedInDirectory()
        {
            using var tempDir = new TempDirectory();
            var filePath = System.IO.Path.Combine(tempDir.Path, "output.png");

            subject.Save(filePath);

            Assert.That(System.IO.File.Exists(filePath), Is.True);
        }

        [Test]
        public void GivenBitmap_WhenSaveIsCalledTwice_ThenSecondFileOverwritesFirst()
        {
            subject.SetPixel(0, 0, OpaqueRed);

            using var tempFile = new TempFile(".png");

            subject.Save(tempFile.Path);
            using var firstLoad = Bitmap.Load(tempFile.Path);
            Assert.That(firstLoad.GetPixel(0, 0), Is.EqualTo(OpaqueRed));

            subject.SetPixel(0, 0, OpaqueBlue);
            subject.Save(tempFile.Path);
            using var secondLoad = Bitmap.Load(tempFile.Path);
            Assert.That(secondLoad.GetPixel(0, 0), Is.EqualTo(OpaqueBlue));
        }

        // Save — format preservation

        [Test]
        public void GivenBitmap_WhenSavedAsPng_ThenCanBeLoadedAsPng()
        {
            subject.SetPixel(0, 0, OpaqueGreen);

            using var tempFile = new TempFile(".png");

            subject.Save(tempFile.Path);

            using var loaded = Bitmap.Load(tempFile.Path);
            Assert.That(loaded.GetPixel(0, 0), Is.EqualTo(OpaqueGreen));
        }

        // Colour round-trip — ARGB to RGBA conversion

        [Test]
        public void GivenColourWithFullAlpha_WhenSetAndGet_ThenColourIsPreserved()
        {
            var colour = Colour.FromArgb(255, 100, 150, 200);

            subject.SetPixel(0, 0, colour);

            Assert.That(subject.GetPixel(0, 0), Is.EqualTo(colour));
        }

        [Test]
        public void GivenColourWithZeroAlpha_WhenSetAndGet_ThenColourIsPreserved()
        {
            var colour = Colour.FromArgb(0, 255, 255, 255);

            subject.SetPixel(0, 0, colour);

            Assert.That(subject.GetPixel(0, 0), Is.EqualTo(colour));
        }

        [Test]
        public void GivenColourWithPartialAlpha_WhenSetAndGet_ThenColourIsPreserved()
        {
            var colour = Colour.FromArgb(128, 64, 128, 192);

            subject.SetPixel(0, 0, colour);

            Assert.That(subject.GetPixel(0, 0), Is.EqualTo(colour));
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(255, 255, 255, 255)]
        [TestCase(128, 128, 128, 128)]
        [TestCase(255, 0, 0, 255)]
        [TestCase(0, 255, 0, 255)]
        [TestCase(0, 0, 255, 255)]
        [TestCase(255, 255, 0, 255)]
        [TestCase(255, 0, 255, 255)]
        [TestCase(0, 255, 255, 255)]
        public void GivenVariousColours_WhenSetAndGet_ThenColourIsPreserved(byte a, byte r, byte g, byte b)
        {
            var colour = Colour.FromArgb(a, r, g, b);

            subject.SetPixel(0, 0, colour);

            Assert.That(subject.GetPixel(0, 0), Is.EqualTo(colour));
        }

        // Size property

        [Test]
        public void GivenBitmap_WhenSizeIsAccessed_ThenReturnsCorrectSize2D()
        {
            subject.Dispose();
            subject = new Bitmap(100, 200);

            Assert.That(subject.Size.Width, Is.EqualTo(100));
            Assert.That(subject.Size.Height, Is.EqualTo(200));
        }

        [Test]
        public void GivenBitmapFromSize2D_WhenSizeIsAccessed_ThenReturnsSameSize2D()
        {
            var size = new Size2D(42, 84);
            subject.Dispose();
            subject = new Bitmap(size);

            Assert.That(subject.Size.Width, Is.EqualTo(size.Width));
            Assert.That(subject.Size.Height, Is.EqualTo(size.Height));
        }

        // Edge cases — boundary coordinates

        [TestCase(0, 0)]
        [TestCase(0, 7)]
        [TestCase(7, 0)]
        [TestCase(7, 7)]
        public void GivenBoundaryCoordinates_WhenSetAndGetPixel_ThenWorksCorrectly(int x, int y)
        {
            subject.SetPixel(x, y, OpaqueRed);

            Assert.That(subject.GetPixel(x, y), Is.EqualTo(OpaqueRed));
        }

        // Edge cases — large bitmap

        [Test]
        public void GivenLargeBitmap_WhenSetAndGetPixel_ThenWorksCorrectly()
        {
            subject.Dispose();
            subject = new Bitmap(1024, 1024);

            subject.SetPixel(0, 0, OpaqueRed);
            subject.SetPixel(1023, 1023, OpaqueBlue);
            subject.SetPixel(512, 512, OpaqueGreen);

            Assert.That(subject.GetPixel(0, 0), Is.EqualTo(OpaqueRed));
            Assert.That(subject.GetPixel(1023, 1023), Is.EqualTo(OpaqueBlue));
            Assert.That(subject.GetPixel(512, 512), Is.EqualTo(OpaqueGreen));
        }

        // Edge cases — non-square bitmap

        [Test]
        public void GivenNonSquareBitmap_WhenSetAndGetPixel_ThenWorksCorrectly()
        {
            subject.Dispose();
            subject = new Bitmap(100, 50);

            subject.SetPixel(0, 0, OpaqueRed);
            subject.SetPixel(99, 49, OpaqueBlue);

            Assert.That(subject.GetPixel(0, 0), Is.EqualTo(OpaqueRed));
            Assert.That(subject.GetPixel(99, 49), Is.EqualTo(OpaqueBlue));
        }

        // Point2D edge cases

        [Test]
        public void GivenPoint2DWithNegativeCoordinates_WhenUsed_ThenBehaviourIsConsistent()
        {
            // Point2D with negative coordinates - should work if within bounds
            // This tests the Point2D struct behavior, not Bitmap bounds checking
            var point = new Point2D(-1, -1);
            // Just verify no exception on creation
            Assert.That(point.X, Is.EqualTo(-1));
            Assert.That(point.Y, Is.EqualTo(-1));
        }

        // Helper classes for file-based tests

        private sealed class TempFile : IDisposable
        {
            public string Path { get; }

            public TempFile(string extension)
            {
                Path = System.IO.Path.GetTempFileName();
                if (!string.IsNullOrEmpty(extension))
                {
                    var newPath = System.IO.Path.ChangeExtension(Path, extension);
                    System.IO.File.Move(Path, newPath);
                    Path = newPath;
                }
            }

            public void Dispose()
            {
                if (System.IO.File.Exists(Path))
                {
                    System.IO.File.Delete(Path);
                }
            }
        }

        private sealed class TempDirectory : IDisposable
        {
            public string Path { get; }

            public TempDirectory()
            {
                Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
                System.IO.Directory.CreateDirectory(Path);
            }

            public void Dispose()
            {
                if (System.IO.Directory.Exists(Path))
                {
                    System.IO.Directory.Delete(Path, true);
                }
            }
        }
    }
}
