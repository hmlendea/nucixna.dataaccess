using System.Threading;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NuciXNA.DataAccess.Content
{
    /// <summary>
    /// Thread-safe singleton manager that coordinates content loading across a pipeline
    /// loader and a plain-file fallback loader.
    /// </summary>
    public sealed class NuciContentManager
    {
        private static readonly Lock syncRoot = new();
        private static volatile NuciContentManager instance;

        private IContentLoader pipelineContentLoader;
        private IContentLoader plainFileContentLoader;

        /// <summary>Gets the singleton instance of <see cref="NuciContentManager"/>.</summary>
        public static NuciContentManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (syncRoot)
                    {
                        instance ??= new NuciContentManager();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets the content path of the texture to use as a placeholder when a
        /// requested texture cannot be found. Set to <c>null</c> or an empty string to
        /// disable placeholder substitution.
        /// </summary>
        public static string MissingTexturePlaceholder { get; set; }

        private NuciContentManager()
        {
        }

        /// <summary>
        /// Initialises the manager with the provided content loader instances.
        /// Use this overload in unit tests or when supplying custom loader implementations.
        /// </summary>
        /// <param name="pipelineContentLoader">The primary pipeline-based content loader.</param>
        /// <param name="plainFileContentLoader">The fallback plain-file content loader.</param>
        public void LoadContent(IContentLoader pipelineContentLoader, IContentLoader plainFileContentLoader)
        {
            this.pipelineContentLoader = pipelineContentLoader;
            this.plainFileContentLoader = plainFileContentLoader;
        }

        /// <summary>
        /// Initialises the manager using the standard MonoGame <see cref="ContentManager"/> and
        /// <see cref="GraphicsDevice"/>. This creates a <see cref="PipelineContentLoader"/> and
        /// a <see cref="PlainFileContentLoader"/> internally.
        /// </summary>
        /// <param name="content">The MonoGame content manager used for pipeline-compiled assets.</param>
        /// <param name="graphicsDevice">The graphics device used for loading raw texture files.</param>
        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            pipelineContentLoader = new PipelineContentLoader(content);
            plainFileContentLoader = new PlainFileContentLoader(graphicsDevice);
        }

        /// <summary>
        /// Loads a <see cref="SoundEffect"/>. Attempts the pipeline loader first; falls back
        /// to the plain-file loader if the pipeline loader returns <c>null</c>.
        /// </summary>
        /// <param name="contentPath">The path to the sound effect asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="SoundEffect"/>.</returns>
        public SoundEffect LoadSoundEffect(string contentPath)
        {
            SoundEffect soundEffect = pipelineContentLoader.TryLoadSoundEffect(contentPath);

            soundEffect ??= plainFileContentLoader.LoadSoundEffect(contentPath);

            return soundEffect;
        }

        /// <summary>
        /// Loads a <see cref="SpriteFont"/> exclusively from the pipeline loader.
        /// </summary>
        /// <param name="contentPath">The path to the sprite font asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="SpriteFont"/>.</returns>
        public SpriteFont LoadSpriteFont(string contentPath)
            => pipelineContentLoader.LoadSpriteFont(contentPath);

        /// <summary>
        /// Loads a <see cref="Texture2D"/>. Attempts the pipeline loader first; falls back to
        /// the plain-file loader if the pipeline loader returns <c>null</c>. When
        /// <see cref="MissingTexturePlaceholder"/> is set and neither loader finds the asset, the
        /// placeholder texture is loaded from the pipeline instead.
        /// </summary>
        /// <param name="contentPath">The path to the texture asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="Texture2D"/>, or <c>null</c> if no asset could be found.</returns>
        public Texture2D LoadTexture2D(string contentPath)
        {
            Texture2D texture2d = pipelineContentLoader.TryLoadTexture2D(contentPath);

            if (texture2d is null)
            {
                if (string.IsNullOrWhiteSpace(MissingTexturePlaceholder))
                {
                    texture2d = plainFileContentLoader.LoadTexture2D(contentPath);
                }
                else
                {
                    texture2d = plainFileContentLoader.TryLoadTexture2D(contentPath);
                }
            }

            if (texture2d is null &&
                !string.IsNullOrWhiteSpace(MissingTexturePlaceholder))
            {
                texture2d = pipelineContentLoader.LoadTexture2D(MissingTexturePlaceholder);
            }

            return texture2d;
        }
    }
}
