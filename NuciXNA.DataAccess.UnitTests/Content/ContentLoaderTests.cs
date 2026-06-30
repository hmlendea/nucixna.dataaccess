using System.IO;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

using NuciXNA.DataAccess.Content;

namespace NuciXNA.DataAccess.UnitTests.Content
{
    public class ContentLoaderTests
    {
        private class ThrowingContentLoader : ContentLoader
        {
            public override SoundEffect LoadSoundEffect(string resourcePath) => throw new IOException("Resource not found");
            public override SpriteFont LoadSpriteFont(string resourcePath) => throw new IOException("Resource not found");
            public override Texture2D LoadTexture2D(string resourcePath) => throw new IOException("Resource not found");
        }

        private class NullReturningContentLoader : ContentLoader
        {
            public override SoundEffect LoadSoundEffect(string resourcePath) => null;
            public override SpriteFont LoadSpriteFont(string resourcePath) => null;
            public override Texture2D LoadTexture2D(string resourcePath) => null;
        }

        ThrowingContentLoader throwingLoader;
        NullReturningContentLoader nullReturningLoader;

        [SetUp]
        public void SetUp()
        {
            throwingLoader = new ThrowingContentLoader();
            nullReturningLoader = new NullReturningContentLoader();
        }

        // TryLoadSoundEffect

        [Test]
        public void GivenLoadSoundEffectThrows_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown()
        {
            Assert.DoesNotThrow(() => throwingLoader.TryLoadSoundEffect("test"));
        }

        [Test]
        public void GivenLoadSoundEffectThrows_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned()
        {
            SoundEffect result = throwingLoader.TryLoadSoundEffect("test");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenLoadSoundEffectDoesNotThrow_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown()
        {
            Assert.DoesNotThrow(() => nullReturningLoader.TryLoadSoundEffect("test"));
        }

        [Test]
        public void GivenLoadSoundEffectReturnsNull_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned()
        {
            SoundEffect result = nullReturningLoader.TryLoadSoundEffect("test");

            Assert.That(result, Is.Null);
        }

        // TryLoadSpriteFont

        [Test]
        public void GivenLoadSpriteFontThrows_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown()
        {
            Assert.DoesNotThrow(() => throwingLoader.TryLoadSpriteFont("test"));
        }

        [Test]
        public void GivenLoadSpriteFontThrows_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned()
        {
            SpriteFont result = throwingLoader.TryLoadSpriteFont("test");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenLoadSpriteFontDoesNotThrow_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown()
        {
            Assert.DoesNotThrow(() => nullReturningLoader.TryLoadSpriteFont("test"));
        }

        [Test]
        public void GivenLoadSpriteFontReturnsNull_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned()
        {
            SpriteFont result = nullReturningLoader.TryLoadSpriteFont("test");

            Assert.That(result, Is.Null);
        }

        // TryLoadTexture2D

        [Test]
        public void GivenLoadTexture2DThrows_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown()
        {
            Assert.DoesNotThrow(() => throwingLoader.TryLoadTexture2D("test"));
        }

        [Test]
        public void GivenLoadTexture2DThrows_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned()
        {
            Texture2D result = throwingLoader.TryLoadTexture2D("test");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenLoadTexture2DDoesNotThrow_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown()
        {
            Assert.DoesNotThrow(() => nullReturningLoader.TryLoadTexture2D("test"));
        }

        [Test]
        public void GivenLoadTexture2DReturnsNull_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned()
        {
            Texture2D result = nullReturningLoader.TryLoadTexture2D("test");

            Assert.That(result, Is.Null);
        }
    }
}
