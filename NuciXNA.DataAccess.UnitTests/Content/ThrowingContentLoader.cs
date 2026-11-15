using System;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;

namespace NuciXNA.DataAccess.UnitTests.Content
{
    /// <summary>A content loader that throws the supplied exception for every required load.</summary>
    internal sealed class ThrowingContentLoader : ContentLoader
    {
        private readonly Exception exceptionToThrow;

        internal ThrowingContentLoader(Exception exceptionToThrow)
        {
            this.exceptionToThrow = exceptionToThrow;
        }

        /// <inheritdoc/>
        public override SoundEffect LoadSoundEffect(string contentPath) => throw exceptionToThrow;

        /// <inheritdoc/>
        public override SpriteFont LoadSpriteFont(string contentPath) => throw exceptionToThrow;

        /// <inheritdoc/>
        public override Texture2D LoadTexture2D(string contentPath) => throw exceptionToThrow;
    }
}