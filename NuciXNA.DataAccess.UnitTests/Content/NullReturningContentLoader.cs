using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;

namespace NuciXNA.DataAccess.UnitTests.Content
{
    /// <summary>A content loader whose required loads always return <c>null</c>.</summary>
    internal sealed class NullReturningContentLoader : ContentLoader
    {
        /// <inheritdoc/>
        public override SoundEffect LoadSoundEffect(string contentPath) => null!;

        /// <inheritdoc/>
        public override SpriteFont LoadSpriteFont(string contentPath) => null!;

        /// <inheritdoc/>
        public override Texture2D LoadTexture2D(string contentPath) => null!;
    }
}