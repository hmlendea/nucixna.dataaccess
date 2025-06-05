using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NuciXNA.DataAccess.Content
{
    /// <summary>
    /// An <see cref="IContentLoader"> that can load resources from the XNA Content Pipeline.
    /// </summary>
    /// <param name="content">The XNA content manager.</param>
    public class PipelineContentLoader(ContentManager content) : ContentLoader, IContentLoader
    {
        readonly ContentManager content = content;

        /// <summary>
        /// Loads a sound effect from the Content Pipeline.
        /// </summary>
        /// <returns>The sound effect.</returns>
        /// <param name="contentPath">The path to the content (without extension).</param>
        public override SoundEffect LoadSoundEffect(string contentPath)
            => content.Load<SoundEffect>(contentPath);

        /// <summary>
        /// Loads a sprite font from the Content Pipeline.
        /// </summary>
        /// <returns>The sprite font.</returns>
        /// <param name="contentPath">The path to the content (without extension).</param>
        public override SpriteFont LoadSpriteFont(string contentPath)
            => content.Load<SpriteFont>(contentPath);

        /// <summary>
        /// Loads a 2D texture either from the Content Pipeline.
        /// </summary>
        /// <returns>The 2D texture.</returns>
        /// <param name="contentPath">The path to the content (without extension).</param>
        public override Texture2D LoadTexture2D(string contentPath)
            => content.Load<Texture2D>(contentPath);
    }
}
