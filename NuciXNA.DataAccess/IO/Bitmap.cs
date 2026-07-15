using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using NuciXNA.Primitives;

namespace NuciXNA.DataAccess.IO
{
    /// <summary>
    /// A mutable, pixel-addressable bitmap backed by an RGBA32 image. Supports
    /// reading and writing individual pixels by coordinate or <see cref="Point2D"/>,
    /// loading from and saving to disk, and construction from an existing
    /// <see cref="Image{Rgba32}"/>.
    /// </summary>
    public sealed class Bitmap : IDisposable
    {
        private readonly Image<Rgba32> sourceImage;

        /// <summary>Gets the dimensions of the bitmap.</summary>
        public Size2D Size { get; }

        /// <summary>Gets or sets the pixel colour at the specified coordinates.</summary>
        /// <param name="x">The zero-based horizontal coordinate.</param>
        /// <param name="y">The zero-based vertical coordinate.</param>
        public Colour this[int x, int y]
        {
            get => GetPixel(x, y);
            set => SetPixel(x, y, value);
        }

        /// <summary>Gets or sets the pixel colour at the specified point.</summary>
        /// <param name="point">The zero-based pixel location.</param>
        public Colour this[Point2D point]
        {
            get => GetPixel(point);
            set => SetPixel(point, value);
        }

        /// <summary>
        /// Initialises a new <see cref="Bitmap"/> wrapping an existing <see cref="Image{Rgba32}"/>.
        /// The bitmap does not take ownership of the image's lifetime.
        /// </summary>
        /// <param name="sourceImage">The source image to wrap.</param>
        public Bitmap(Image<Rgba32> sourceImage)
        {
            this.sourceImage = sourceImage;

            Size = new Size2D(sourceImage.Width, sourceImage.Height);
        }

        /// <summary>
        /// Initialises a new blank <see cref="Bitmap"/> with all pixels set to transparent black.
        /// </summary>
        /// <param name="width">The width of the bitmap in pixels.</param>
        /// <param name="height">The height of the bitmap in pixels.</param>
        public Bitmap(int width, int height)
        {
            sourceImage = new Image<Rgba32>(width, height);

            Size = new Size2D(width, height);
        }

        /// <summary>
        /// Initialises a new blank <see cref="Bitmap"/> with all pixels set to transparent black.
        /// </summary>
        /// <param name="size">The dimensions of the bitmap in pixels.</param>
        public Bitmap(Size2D size) : this(size.Width, size.Height)
        {
        }

        private Bitmap(string fileName)
        {
            sourceImage = Image.Load<Rgba32>(fileName);

            Size = new Size2D(sourceImage.Width, sourceImage.Height);
        }

        /// <summary>Returns the colour of the pixel at the specified coordinates.</summary>
        /// <param name="x">The zero-based horizontal coordinate.</param>
        /// <param name="y">The zero-based vertical coordinate.</param>
        /// <returns>The <see cref="Colour"/> of the pixel at (<paramref name="x"/>, <paramref name="y"/>).</returns>
        public Colour GetPixel(int x, int y)
        {
            Rgba32 pixel = sourceImage[x, y];

            return Colour.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
        }

        /// <summary>Returns the colour of the pixel at the specified point.</summary>
        /// <param name="location">The zero-based pixel location.</param>
        /// <returns>The <see cref="Colour"/> of the pixel at <paramref name="location"/>.</returns>
        public Colour GetPixel(Point2D location)
            => GetPixel(location.X, location.Y);

        /// <summary>Sets the colour of the pixel at the specified coordinates.</summary>
        /// <param name="x">The zero-based horizontal coordinate.</param>
        /// <param name="y">The zero-based vertical coordinate.</param>
        /// <param name="colour">The colour to assign to the pixel.</param>
        public void SetPixel(int x, int y, Colour colour)
            => sourceImage[x, y] = new(colour.R, colour.G, colour.B, colour.A);

        /// <summary>Sets the colour of the pixel at the specified point.</summary>
        /// <param name="location">The zero-based pixel location.</param>
        /// <param name="colour">The colour to assign to the pixel.</param>
        public void SetPixel(Point2D location, Colour colour)
            => SetPixel(location.X, location.Y, colour);

        /// <summary>Loads a <see cref="Bitmap"/> from the specified image file on disk.</summary>
        /// <param name="fileName">The path to the image file to load.</param>
        /// <returns>A new <see cref="Bitmap"/> containing the loaded image data.</returns>
        public static Bitmap Load(string fileName) => new(fileName);

        /// <summary>Saves the bitmap to the specified file path on disk.</summary>
        /// <param name="fileName">The destination file path. The format is inferred from the file extension.</param>
        public void Save(string fileName) => sourceImage.Save(fileName);

        /// <summary>Releases the resources held by the underlying image.</summary>
        public void Dispose() => sourceImage.Dispose();
    }
}
