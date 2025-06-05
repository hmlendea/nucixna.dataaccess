using System;
using System.IO;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace NuciXNA.DataAccess.Content
{
    /// <summary>
    /// An <see cref="IContentLoader"> that can load resources from plain disk files.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    public class PlainFileContentLoader(GraphicsDevice graphicsDevice) : ContentLoader, IContentLoader
    {
        GraphicsDevice GraphicsDevice { get; set; } = graphicsDevice;

        /// <summary>
        /// Loads a sound effect from disk (WAVs only).
        /// </summary>
        /// <returns>The sound effect.</returns>
        /// <param name="contentPath">The path to the content (without extension).</param>
        public override SoundEffect LoadSoundEffect(string contentPath)
        {
            FileStream fileStream = GetContentFileStream($"{contentPath}.wav");
            SoundEffect soundEffect = SoundEffect.FromStream(fileStream);

            return soundEffect;
        }

        /// <summary>
        /// Loads a sprite font from the Content Pipeline.
        /// </summary>
        /// <returns>The sprite font.</returns>
        /// <param name="contentPath">The path to the content (without extension).</param>
        public override SpriteFont LoadSpriteFont(string contentPath)
            => throw new NotImplementedException();

        /// <summary>
        /// Loads a 2D texture either from disk (PNGs only).
        /// </summary>
        /// <returns>The 2D texture.</returns>
        /// <param name="contentPath">The path to the content (without extension).</param>
        public override Texture2D LoadTexture2D(string contentPath)
        {
            FileStream fileStream = GetContentFileStream($"{contentPath}.png");
            Texture2D texture2D = Texture2D.FromStream(GraphicsDevice, fileStream);

            return texture2D;
        }

        private static FileStream GetContentFileStream(string filePath)
            => File.OpenRead(filePath);
    }
}
