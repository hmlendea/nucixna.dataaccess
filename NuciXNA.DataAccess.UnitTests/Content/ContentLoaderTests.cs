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
        private ThrowingContentLoader ioExceptionLoader = null!;
        private ThrowingContentLoader argumentExceptionLoader = null!;
        private ThrowingContentLoader invalidOperationExceptionLoader = null!;
        private ThrowingContentLoader outOfMemoryExceptionLoader = null!;
        private ThrowingContentLoader nullReferenceExceptionLoader = null!;
        private NullReturningContentLoader nullReturningLoader = null!;
        private SpyContentLoader spyLoader = null!;

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

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsArgumentException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => argumentExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsArgumentException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(argumentExceptionLoader.TryLoadSoundEffect(path), Is.Null);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsInvalidOperationException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => invalidOperationExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsInvalidOperationException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(invalidOperationExceptionLoader.TryLoadSoundEffect(path), Is.Null);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsOutOfMemoryException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => outOfMemoryExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsOutOfMemoryException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(outOfMemoryExceptionLoader.TryLoadSoundEffect(path), Is.Null);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsNullReferenceException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReferenceExceptionLoader.TryLoadSoundEffect(path), Throws.Nothing);

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsNullReferenceException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReferenceExceptionLoader.TryLoadSoundEffect(path), Is.Null);

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

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        [TestCase("audio/effects/explosion")]
        public void GivenPath_WhenTryLoadSoundEffectIsCalled_ThenLoadSoundEffectIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadSoundEffect(path);

            Assert.That(
                spyLoader.LastSoundEffectPath,
                Is.EqualTo(path));
        }

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

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsArgumentException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => argumentExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsArgumentException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(argumentExceptionLoader.TryLoadSpriteFont(path), Is.Null);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsInvalidOperationException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => invalidOperationExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsInvalidOperationException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(invalidOperationExceptionLoader.TryLoadSpriteFont(path), Is.Null);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsOutOfMemoryException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => outOfMemoryExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsOutOfMemoryException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(outOfMemoryExceptionLoader.TryLoadSpriteFont(path), Is.Null);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsNullReferenceException_WhenTryLoadSpriteFontIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReferenceExceptionLoader.TryLoadSpriteFont(path), Throws.Nothing);

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        public void GivenLoadSpriteFontThrowsNullReferenceException_WhenTryLoadSpriteFontIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReferenceExceptionLoader.TryLoadSpriteFont(path), Is.Null);

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

        [TestCase("testFont")]
        [TestCase("fonts/ui")]
        [TestCase("fonts/hud/score")]
        public void GivenPath_WhenTryLoadSpriteFontIsCalled_ThenLoadSpriteFontIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadSpriteFont(path);

            Assert.That(
                spyLoader.LastSpriteFontPath,
                Is.EqualTo(path));
        }

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

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsArgumentException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => argumentExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsArgumentException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(argumentExceptionLoader.TryLoadTexture2D(path), Is.Null);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsInvalidOperationException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => invalidOperationExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsInvalidOperationException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(invalidOperationExceptionLoader.TryLoadTexture2D(path), Is.Null);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsOutOfMemoryException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => outOfMemoryExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsOutOfMemoryException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(outOfMemoryExceptionLoader.TryLoadTexture2D(path), Is.Null);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsNullReferenceException_WhenTryLoadTexture2DIsCalled_ThenNoExceptionIsThrown(string path)
            => Assert.That(() => nullReferenceExceptionLoader.TryLoadTexture2D(path), Throws.Nothing);

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        public void GivenLoadTexture2DThrowsNullReferenceException_WhenTryLoadTexture2DIsCalled_ThenNullIsReturned(string path)
            => Assert.That(nullReferenceExceptionLoader.TryLoadTexture2D(path), Is.Null);

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

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        public void GivenPath_WhenTryLoadTexture2DIsCalled_ThenLoadTexture2DIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadTexture2D(path);

            Assert.That(
                spyLoader.LastTexture2DPath,
                Is.EqualTo(path));
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsUnauthorizedAccessException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
        {
            ThrowingContentLoader loader = new(new UnauthorizedAccessException("Access denied"));
            Assert.That(() => loader.TryLoadSoundEffect(path), Throws.Nothing);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsUnauthorizedAccessException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
        {
            ThrowingContentLoader loader = new(new UnauthorizedAccessException("Access denied"));
            Assert.That(loader.TryLoadSoundEffect(path), Is.Null);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsFileNotFoundException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
        {
            ThrowingContentLoader loader = new(new FileNotFoundException("File not found"));
            Assert.That(() => loader.TryLoadSoundEffect(path), Throws.Nothing);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsFileNotFoundException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
        {
            ThrowingContentLoader loader = new(new FileNotFoundException("File not found"));
            Assert.That(loader.TryLoadSoundEffect(path), Is.Null);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsDirectoryNotFoundException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
        {
            ThrowingContentLoader loader = new(new DirectoryNotFoundException("Directory not found"));
            Assert.That(() => loader.TryLoadSoundEffect(path), Throws.Nothing);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsDirectoryNotFoundException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
        {
            ThrowingContentLoader loader = new(new DirectoryNotFoundException("Directory not found"));
            Assert.That(loader.TryLoadSoundEffect(path), Is.Null);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsNotSupportedException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
        {
            ThrowingContentLoader loader = new(new NotSupportedException("Not supported"));
            Assert.That(() => loader.TryLoadSoundEffect(path), Throws.Nothing);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsNotSupportedException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
        {
            ThrowingContentLoader loader = new(new NotSupportedException("Not supported"));
            Assert.That(loader.TryLoadSoundEffect(path), Is.Null);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsStackOverflowException_WhenTryLoadSoundEffectIsCalled_ThenNoExceptionIsThrown(string path)
        {
            ThrowingContentLoader loader = new(new StackOverflowException("Stack overflow"));
            Assert.That(() => loader.TryLoadSoundEffect(path), Throws.Nothing);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        public void GivenLoadSoundEffectThrowsStackOverflowException_WhenTryLoadSoundEffectIsCalled_ThenNullIsReturned(string path)
        {
            ThrowingContentLoader loader = new(new StackOverflowException("Stack overflow"));
            Assert.That(loader.TryLoadSoundEffect(path), Is.Null);
        }

        [TestCase(".")]
        [TestCase("..")]
        [TestCase("./relative")]
        [TestCase("../parent")]
        [TestCase("path/with spaces/file")]
        [TestCase("path/with-unicode/文件")]
        [TestCase("path/with/special/chars_!@#$%^&*()")]
        [TestCase("very/long/path/that/goes/deep/into/the/directory/structure/with/many/segments")]
        public void GivenEdgeCasePath_WhenTryLoadSoundEffectIsCalled_ThenLoadSoundEffectIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadSoundEffect(path);
            Assert.That(spyLoader.LastSoundEffectPath, Is.EqualTo(path));
        }

        [TestCase(".")]
        [TestCase("..")]
        [TestCase("./relative")]
        [TestCase("../parent")]
        [TestCase("path/with spaces/file")]
        [TestCase("path/with-unicode/文件")]
        [TestCase("path/with/special/chars_!@#$%^&*()")]
        [TestCase("very/long/path/that/goes/deep/into/the/directory/structure/with/many/segments")]
        public void GivenEdgeCasePath_WhenTryLoadSpriteFontIsCalled_ThenLoadSpriteFontIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadSpriteFont(path);
            Assert.That(spyLoader.LastSpriteFontPath, Is.EqualTo(path));
        }

        [TestCase(".")]
        [TestCase("..")]
        [TestCase("./relative")]
        [TestCase("../parent")]
        [TestCase("path/with spaces/file")]
        [TestCase("path/with-unicode/文件")]
        [TestCase("path/with/special/chars_!@#$%^&*()")]
        [TestCase("very/long/path/that/goes/deep/into/the/directory/structure/with/many/segments")]
        public void GivenEdgeCasePath_WhenTryLoadTexture2DIsCalled_ThenLoadTexture2DIsCalledWithThatPath(string path)
        {
            spyLoader.TryLoadTexture2D(path);
            Assert.That(spyLoader.LastTexture2DPath, Is.EqualTo(path));
        }

        [Test]
        public void GivenMultipleCalls_WhenTryLoadSoundEffectIsCalled_ThenLastPathIsRetained()
        {
            spyLoader.TryLoadSoundEffect("first");
            spyLoader.TryLoadSoundEffect("second");
            spyLoader.TryLoadSoundEffect("third");

            Assert.That(spyLoader.LastSoundEffectPath, Is.EqualTo("third"));
        }

        [Test]
        public void GivenMultipleCalls_WhenTryLoadSpriteFontIsCalled_ThenLastPathIsRetained()
        {
            spyLoader.TryLoadSpriteFont("first");
            spyLoader.TryLoadSpriteFont("second");
            spyLoader.TryLoadSpriteFont("third");

            Assert.That(spyLoader.LastSpriteFontPath, Is.EqualTo("third"));
        }

        [Test]
        public void GivenMultipleCalls_WhenTryLoadTexture2DIsCalled_ThenLastPathIsRetained()
        {
            spyLoader.TryLoadTexture2D("first");
            spyLoader.TryLoadTexture2D("second");
            spyLoader.TryLoadTexture2D("third");

            Assert.That(spyLoader.LastTexture2DPath, Is.EqualTo("third"));
        }

        [Test]
        public void GivenLoadThrowsException_WhenTryLoadIsCalled_ThenExceptionIsFullySuppressed()
        {
            InvalidOperationException exception = new("Test exception");
            ThrowingContentLoader loader = new(exception);

            SoundEffect? soundResult = loader.TryLoadSoundEffect("test");
            Assert.That(soundResult, Is.Null);

            SpriteFont? fontResult = loader.TryLoadSpriteFont("test");
            Assert.That(fontResult, Is.Null);

            Texture2D? textureResult = loader.TryLoadTexture2D("test");
            Assert.That(textureResult, Is.Null);
        }
    }
}
