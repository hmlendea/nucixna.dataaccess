using System;
using System.IO;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace NuciXNA.DataAccess.Content
{
    /// <summary>
    /// Loads content assets directly from the file system without requiring a compiled
    /// MonoGame pipeline. Supports <c>.wav</c> sound effects and <c>.png</c> textures.
    /// </summary>
    public sealed class PlainFileContentLoader : ContentLoader
    {
        private readonly GraphicsDevice graphicsDevice;

        /// <summary>Initialises a new instance of <see cref="PlainFileContentLoader"/>.</summary>
        /// <param name="graphicsDevice">The graphics device used when decoding raw texture files.</param>
        public PlainFileContentLoader(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        /// <inheritdoc/>
        public override SoundEffect LoadSoundEffect(string contentPath)
            => SoundEffect.FromStream(GetContentFileStream($"{contentPath}.wav"));

        /// <inheritdoc/>
        public override SpriteFont LoadSpriteFont(string contentPath)
            => throw new NotImplementedException();

        /// <inheritdoc/>
        public override Texture2D LoadTexture2D(string contentPath)
            => Texture2D.FromStream(graphicsDevice, GetContentFileStream($"{contentPath}.png"));

        private static FileStream GetContentFileStream(string filePath)
            => File.OpenRead(filePath);
    }
}
