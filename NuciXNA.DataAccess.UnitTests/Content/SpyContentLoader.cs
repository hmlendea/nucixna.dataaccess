using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;

namespace NuciXNA.DataAccess.UnitTests.Content
{
    /// <summary>A content loader that records the last forwarded path for each asset kind.</summary>
    internal sealed class SpyContentLoader : ContentLoader
    {
        internal string LastSoundEffectPath { get; private set; } = string.Empty;

        internal string LastSpriteFontPath { get; private set; } = string.Empty;

        internal string LastTexture2DPath { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public override SoundEffect LoadSoundEffect(string contentPath)
        {
            LastSoundEffectPath = contentPath;

            return null!;
        }

        /// <inheritdoc/>
        public override SpriteFont LoadSpriteFont(string contentPath)
        {
            LastSpriteFontPath = contentPath;

            return null!;
        }

        /// <inheritdoc/>
        public override Texture2D LoadTexture2D(string contentPath)
        {
            LastTexture2DPath = contentPath;

            return null!;
        }
    }
}