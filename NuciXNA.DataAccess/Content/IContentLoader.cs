using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace NuciXNA.DataAccess.Content
{
    /// <summary>Defines a contract for loading XNA/MonoGame content assets.</summary>
    public interface IContentLoader
    {
        /// <summary>Loads a <see cref="SoundEffect"/> from the specified content path.</summary>
        /// <param name="contentPath">The path to the sound effect asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="SoundEffect"/>.</returns>
        SoundEffect LoadSoundEffect(string contentPath);

        /// <summary>
        /// Attempts to load a <see cref="SoundEffect"/> from the specified content path.
        /// Returns <c>null</c> instead of throwing if the asset cannot be loaded.
        /// </summary>
        /// <param name="contentPath">The path to the sound effect asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="SoundEffect"/>, or <c>null</c> if loading fails.</returns>
        SoundEffect TryLoadSoundEffect(string contentPath);

        /// <summary>Loads a <see cref="SpriteFont"/> from the specified content path.</summary>
        /// <param name="contentPath">The path to the sprite font asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="SpriteFont"/>.</returns>
        SpriteFont LoadSpriteFont(string contentPath);

        /// <summary>
        /// Attempts to load a <see cref="SpriteFont"/> from the specified content path.
        /// Returns <c>null</c> instead of throwing if the asset cannot be loaded.
        /// </summary>
        /// <param name="contentPath">The path to the sprite font asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="SpriteFont"/>, or <c>null</c> if loading fails.</returns>
        SpriteFont TryLoadSpriteFont(string contentPath);

        /// <summary>Loads a <see cref="Texture2D"/> from the specified content path.</summary>
        /// <param name="contentPath">The path to the texture asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="Texture2D"/>.</returns>
        Texture2D LoadTexture2D(string contentPath);

        /// <summary>
        /// Attempts to load a <see cref="Texture2D"/> from the specified content path.
        /// Returns <c>null</c> instead of throwing if the asset cannot be loaded.
        /// </summary>
        /// <param name="contentPath">The path to the texture asset, relative to the content root.</param>
        /// <returns>The loaded <see cref="Texture2D"/>, or <c>null</c> if loading fails.</returns>
        Texture2D TryLoadTexture2D(string contentPath);
    }
}
