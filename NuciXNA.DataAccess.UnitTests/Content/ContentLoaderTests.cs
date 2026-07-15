using System;
using System.IO;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

using NuciXNA.DataAccess.Content;

namespace NuciXNA.DataAccess.UnitTests.Content
{
    [TestFixture]
    public sealed class ContentLoaderTests
    {
        private sealed class ThrowingContentLoader : ContentLoader
        {
            private readonly Exception exceptionToThrow;

            public ThrowingContentLoader(Exception exceptionToThrow)
            {
                this.exceptionToThrow = exceptionToThrow;
            }

            public override SoundEffect LoadSoundEffect(string resourcePath) => throw exceptionToThrow;

            public override SpriteFont LoadSpriteFont(string resourcePath) => throw exceptionToThrow;

            public override Texture2D LoadTexture2D(string resourcePath) => throw exceptionToThrow;
        }

        private sealed class NullReturningContentLoader : ContentLoader
        {
            public override SoundEffect LoadSoundEffect(string resourcePath) => null;

            public override SpriteFont LoadSpriteFont(string resourcePath) => null;

            public override Texture2D LoadTexture2D(string resourcePath) => null;
        }

        private sealed class SpyContentLoader : ContentLoader
        {
            public string LastSoundEffectPath { get; private set; }

            public string LastSpriteFontPath { get; private set; }

            public string LastTexture2DPath { get; private set; }

            public override SoundEffect LoadSoundEffect(string resourcePath)
            {
                LastSoundEffectPath = resourcePath;

                return null;
            }

            public override SpriteFont LoadSpriteFont(string resourcePath)
            {
                LastSpriteFontPath = resourcePath;

                return null;
            }

            public override Texture2D LoadTexture2D(string resourcePath)
            {
                LastTexture2DPath = resourcePath;

                return null;
            }
        }

        private ThrowingContentLoader ioExceptionLoader;
        private ThrowingContentLoader argumentExceptionLoader;
        private ThrowingContentLoader invalidOperationExceptionLoader;
        private ThrowingContentLoader outOfMemoryExceptionLoader;
        private ThrowingContentLoader nullReferenceExceptionLoader;
        private NullReturningContentLoader nullReturningLoader;
        private SpyContentLoader spyLoader;

        [SetUp]
        public void SetUp()
        {
            ioExceptionLoader = new ThrowingContentLoader(new IOException("Resource not found"));
            argumentExceptionLoader = new ThrowingContentLoader(new ArgumentException("Invalid argument"));
            invalidOperationExceptionLoader = new ThrowingContentLoader(new InvalidOperationException("Invalid operation"));
            outOfMemoryExceptionLoader = new ThrowingContentLoader(new OutOfMemoryException("Out of memory"));
            nullReferenceExceptionLoader = new ThrowingContentLoader(new NullReferenceException("Null reference"));
            nullReturningLoader = new NullReturningContentLoader();
            spyLoader = new SpyContentLoader();
        }

        // TryLoadSoundEffect — IOException

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        [TestCase("audio/effects/explosion")]
        [TestCase("")]
        public void GivenLoadSoundEffectThrowsIOException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => ioExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        [TestCase("audio/effects/explosion")]
        [TestCase("")]
        public void GivenLoadSoundEffectThrowsIOException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(ioExceptionLoader.TryLoadSoundEffect(path), Is.Null);

        // TryLoadSoundEffect — ArgumentException

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsArgumentException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => argumentExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsArgumentException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(argumentExceptionLoader.TryLoadSoundEffect(path), Is.Null);

        // TryLoadSoundEffect — InvalidOperationException

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsInvalidOperationException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => invalidOperationExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsInvalidOperationException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(invalidOperationExceptionLoader.TryLoadSoundEffect(path), Is.Null);

        // TryLoadSoundEffect — OutOfMemoryException

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsOutOfMemoryException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => outOfMemoryExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsOutOfMemoryException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(outOfMemoryExceptionLoader.TryLoadSoundEffect(path), Is.Null);

        // TryLoadSoundEffect — NullReferenceException

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsNullReferenceException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReferenceExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsNullReferenceException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReferenceExceptionLoader.TryLoadSoundEffect(path), Is.Null);

        // TryLoadSoundEffect — null return

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        [TestCase("audio/effects/explosion")]
        public void GivenLoadSoundEffectReturnsNull_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReturningLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        [TestCase("audio/effects/explosion")]
        public void GivenLoadSoundEffectReturnsNull_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReturningLoader.TryLoadSoundEffect(path), Is.Null);

        // TryLoadSoundEffect — path forwarding

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        [TestCase("audio/effects/explosion")]
        public void GivenPath_WhenTryLoadSoundEffectIsCalled_ThenLoadSoundEffectIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadSoundEffect(path);

            Assert.That(spyLoader.LastSoundEffectPath, Is.EqualTo(path));
        }

        // TryLoadSpriteFont — IOException

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        [TestCase("fonts/hud/score")]
        [TestCase("")]
        public void GivenLoadSpriteFontThrowsIOException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => ioExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        [TestCase("fonts/hud/score")]
        [TestCase("")]
        public void GivenLoadSpriteFontThrowsIOException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(ioExceptionLoader.TryLoadSpriteFont(path), Is.Null);

        // TryLoadSpriteFont — ArgumentException

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsArgumentException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => argumentExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsArgumentException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(argumentExceptionLoader.TryLoadSpriteFont(path), Is.Null);

        // TryLoadSpriteFont — InvalidOperationException

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsInvalidOperationException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => invalidOperationExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsInvalidOperationException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(invalidOperationExceptionLoader.TryLoadSpriteFont(path), Is.Null);

        // TryLoadSpriteFont — OutOfMemoryException

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsOutOfMemoryException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => outOfMemoryExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsOutOfMemoryException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(outOfMemoryExceptionLoader.TryLoadSpriteFont(path), Is.Null);

        // TryLoadSpriteFont — NullReferenceException

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsNullReferenceException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReferenceExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsNullReferenceException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReferenceExceptionLoader.TryLoadSpriteFont(path), Is.Null);

        // TryLoadSpriteFont — null return

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        [TestCase("fonts/hud/score")]
        public void GivenLoadSpriteFontReturnsNull_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReturningLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        [TestCase("fonts/hud/score")]
        public void GivenLoadSpriteFontReturnsNull_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReturningLoader.TryLoadSpriteFont(path), Is.Null);

        // TryLoadSpriteFont — path forwarding

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        [TestCase("fonts/hud/score")]
        public void GivenPath_WhenTryLoadSpriteFontIsCalled_ThenLoadSpriteFontIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadSpriteFont(path);

            Assert.That(spyLoader.LastSpriteFontPath, Is.EqualTo(path));
        }

        // TryLoadTexture2D — IOException

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        [TestCase("")]
        public void GivenLoadTexture2DThrowsIOException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => ioExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        [TestCase("")]
        public void GivenLoadTexture2DThrowsIOException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(ioExceptionLoader.TryLoadTexture2D(path), Is.Null);

        // TryLoadTexture2D — ArgumentException

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsArgumentException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => argumentExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsArgumentException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(argumentExceptionLoader.TryLoadTexture2D(path), Is.Null);

        // TryLoadTexture2D — InvalidOperationException

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsInvalidOperationException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => invalidOperationExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsInvalidOperationException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(invalidOperationExceptionLoader.TryLoadTexture2D(path), Is.Null);

        // TryLoadTexture2D — OutOfMemoryException

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsOutOfMemoryException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => outOfMemoryExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsOutOfMemoryException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(outOfMemoryExceptionLoader.TryLoadTexture2D(path), Is.Null);

        // TryLoadTexture2D — NullReferenceException

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsNullReferenceException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReferenceExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsNullReferenceException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReferenceExceptionLoader.TryLoadTexture2D(path), Is.Null);

        // TryLoadTexture2D — null return

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        public void GivenLoadTexture2DReturnsNull_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReturningLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        public void GivenLoadTexture2DReturnsNull_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReturningLoader.TryLoadTexture2D(path), Is.Null);

        // TryLoadTexture2D — path forwarding

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        public void GivenPath_WhenTryLoadTexture2DIsCalled_ThenLoadTexture2DIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadTexture2D(path);

            Assert.That(spyLoader.LastTexture2DPath, Is.EqualTo(path));
        }
    }
}
