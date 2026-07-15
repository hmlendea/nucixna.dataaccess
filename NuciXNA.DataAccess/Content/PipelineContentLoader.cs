using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NuciXNA.DataAccess.Content
{
    /// <summary>
    /// Loads content assets using the MonoGame pipeline <see cref="ContentManager"/>.
    /// Suitable for compiled <c>.xnb</c> assets.
    /// </summary>
    public sealed class PipelineContentLoader : ContentLoader
    {
        private readonly ContentManager content;

        /// <summary>Initialises a new instance of <see cref="PipelineContentLoader"/>.</summary>
        /// <param name="content">The MonoGame content manager used to load compiled pipeline assets.</param>
        public PipelineContentLoader(ContentManager content)
        {
            this.content = content;
        }

        /// <inheritdoc/>
        public override SoundEffect LoadSoundEffect(string contentPath)
            => content.Load<SoundEffect>(contentPath);

        /// <inheritdoc/>
        public override SpriteFont LoadSpriteFont(string contentPath)
            => content.Load<SpriteFont>(contentPath);

        /// <inheritdoc/>
        public override Texture2D LoadTexture2D(string contentPath)
            => content.Load<Texture2D>(contentPath);
    }
}
