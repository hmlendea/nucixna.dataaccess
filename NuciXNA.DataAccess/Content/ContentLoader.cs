using System;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace NuciXNA.DataAccess.Content
{
    /// <summary>
    /// Base class for content loaders. Provides safe try-load wrappers around
    /// the abstract load methods that concrete implementations must supply.
    /// </summary>
    public abstract class ContentLoader : IContentLoader
    {
        /// <inheritdoc/>
        public abstract SoundEffect LoadSoundEffect(string contentPath);

        /// <inheritdoc/>
        public SoundEffect TryLoadSoundEffect(string contentPath)
            => TryLoad(() => LoadSoundEffect(contentPath));

        /// <inheritdoc/>
        public abstract SpriteFont LoadSpriteFont(string contentPath);

        /// <inheritdoc/>
        public SpriteFont TryLoadSpriteFont(string contentPath)
            => TryLoad(() => LoadSpriteFont(contentPath));

        /// <inheritdoc/>
        public abstract Texture2D LoadTexture2D(string contentPath);

        /// <inheritdoc/>
        public Texture2D TryLoadTexture2D(string contentPath)
            => TryLoad(() => LoadTexture2D(contentPath));

        private static T TryLoad<T>(Func<T> load) where T : class
        {
            try
            {
                return load();
            }
            catch
            {
                return null;
            }
        }
    }
}
