using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using NuciXNA.Primitives;

namespace NuciXNA.DataAccess.IO
{
    /// <summary>
    /// Fast bitmap manipulator.
    /// </summary>
    public class Bitmap : IDisposable
    {
        readonly Image<Rgba32> sourceImage;

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size2D Size { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bitmap"/> class.
        /// </summary>
        /// <param name="sourceImage">Source image.</param>
        public Bitmap(Image<Rgba32> sourceImage) => this.sourceImage = sourceImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bitmap"/> class.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public Bitmap(int width, int height)
        {
            sourceImage = new Image<Rgba32>(width, height);

            Size = new Size2D(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bitmap"/> class.
        /// </summary>
        /// <param name="size">Size.</param>
        public Bitmap(Size2D size) : this(size.Width, size.Height) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bitmap"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        Bitmap(string fileName)
        {
            sourceImage = Image.Load<Rgba32>(fileName);

            Size = new Size2D(sourceImage.Width, sourceImage.Height);
        }

        /// <summary>
        /// Gets or sets the colour of the pixel at the specified coordinates.
        /// </summary>
        /// <param name="x">The X-axis coordinate of the pixel.</param>
        /// <param name="y">The Y-axis coordinate of the pixel.</param>
        /// <returns>The colour of the specified pixel.</returns>
        public Colour this[int x, int y]
        {
            get => GetPixel(x, y);
            set => SetPixel(x, y, value);
        }

        /// <summary>
        /// Gets or sets the colour of the pixel at the specified point.
        /// </summary>
        /// <param name="point">The point representing the pixel location.</param>
        /// <returns>The colour of the specified pixel.</returns>
        public Colour this[Point2D point]
        {
            get => GetPixel(point);
            set => SetPixel(point, value);
        }

        /// <summary>
        /// Gets the colour of the specified pixel.
        /// </summary>
        /// <param name="x">X coordinate of the pixel.</param>
        /// <param name="y">Y coordinate of the pixel.</param>
        /// <returns>Pixel colou.r</returns>
        public Colour GetPixel(int x, int y)
        {
            Rgba32 pixel = sourceImage[x, y];

            return Colour.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
        }

        /// <summary>
        /// Gets the colour of the specified pixel.
        /// </summary>
        /// <param name="location">The location of the pixel.</param>
        /// <returns>Pixel colour.</returns>
        public Colour GetPixel(Point2D location)
            => GetPixel(location.X, location.Y);

        /// <summary>
        /// Sets the colour of the specified pixel.
        /// </summary>
        /// <param name="x">The X coordinate of the pixel.</param>
        /// <param name="y">The Y coordinate of the pixel.</param>
        /// <param name="colour">Pixel colour.</param>
        public void SetPixel(int x, int y, Colour colour)
            => sourceImage[x, y] = new(colour.R, colour.G, colour.B, colour.A);

        /// <summary>
        /// Sets the colour of the specified pixel.
        /// </summary>
        /// <param name="location">The location of the pixel.</param>
        /// <param name="colour">Pixel colour.</param>
        public void SetPixel(Point2D location, Colour colour)
            => SetPixel(location.X, location.Y, colour);

        /// <summary>
        /// Loads a bitmap from the specified file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>A new <see cref="Bitmap"/> instance.</returns>
        public static Bitmap Load(string fileName) => new(fileName);

        /// <summary>
        /// Saves the bitmap to the specified file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void Save(string fileName) => sourceImage.Save(fileName);

        /// <summary>
        /// Releases all resource used by the <see cref="Bitmap"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Bitmap"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Bitmap"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="Bitmap"/>
        /// so the garbage collector can reclaim the memory that the <see cref="Bitmap"/> was occupying.</remarks>
        public void Dispose() => sourceImage.Dispose();
    }
}
