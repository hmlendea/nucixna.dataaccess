using System;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Moq;
using NUnit.Framework;

using NuciXNA.DataAccess.Content;

namespace NuciXNA.DataAccess.UnitTests.Content
{
    [TestFixture]
    public sealed class NuciContentManagerTests
    {
        private Mock<IContentLoader> pipelineContentLoaderMock;
        private Mock<IContentLoader> plainFileContentLoaderMock;

        [SetUp]
        public void SetUp()
        {
            pipelineContentLoaderMock = new Mock<IContentLoader>();
            plainFileContentLoaderMock = new Mock<IContentLoader>();

            NuciContentManager.Instance.LoadContent(
                pipelineContentLoaderMock.Object,
                plainFileContentLoaderMock.Object);

            NuciContentManager.MissingTexturePlaceholder = null;
        }

        // LoadSoundEffect — pipeline succeeds via TryLoad

        [Test]
        public void GivenPipelineContentExists_AndPlainFileExists_WhenLoadSoundEffectIsCalled_ThenNoExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadSoundEffect(It.IsAny<string>()));

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadSoundEffect(It.IsAny<string>()));

            Assert.That(
                () => NuciContentManager.Instance.LoadSoundEffect("testSoundEffect"),
                Throws.Nothing);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_WhenLoadSoundEffectIsCalled_ThenPlainFileLoaderIsCalled()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadSoundEffect(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSoundEffect("testSoundEffect");

            plainFileContentLoaderMock.Verify(loader => loader.LoadSoundEffect("testSoundEffect"), Times.Once);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndPlainFileExists_WhenLoadSoundEffectIsCalled_ThenNoExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadSoundEffect(It.IsAny<string>()));

            Assert.That(
                () => NuciContentManager.Instance.LoadSoundEffect("testSoundEffect"),
                Throws.Nothing);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndPlainFileDoesNotExist_WhenLoadSoundEffectIsCalled_ThenExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadSoundEffect(It.IsAny<string>()))
                .Throws<Exception>();

            Assert.That(
                () => NuciContentManager.Instance.LoadSoundEffect("testSoundEffect"),
                Throws.TypeOf<Exception>());
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        [TestCase("audio/effects/explosion")]
        public void GivenPipelineTryLoadReturnsNull_WhenLoadSoundEffectIsCalled_ThenPipelineTryLoadIsCalledWithCorrectPath(string path)
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadSoundEffect(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSoundEffect(path);

            pipelineContentLoaderMock.Verify(loader => loader.TryLoadSoundEffect(path), Times.Once);
        }

        [TestCase("testSoundEffect")]
        [TestCase("sounds/background")]
        [TestCase("audio/effects/explosion")]
        public void GivenPipelineTryLoadReturnsNull_WhenLoadSoundEffectIsCalled_ThenPlainFileLoadIsCalledWithCorrectPath(string path)
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadSoundEffect(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSoundEffect(path);

            plainFileContentLoaderMock.Verify(loader => loader.LoadSoundEffect(path), Times.Once);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_WhenLoadSoundEffectIsCalled_ThenPipelineTryLoadIsCalledOnce()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadSoundEffect(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSoundEffect("testSoundEffect");

            pipelineContentLoaderMock.Verify(loader => loader.TryLoadSoundEffect(It.IsAny<string>()), Times.Once);
        }

        // LoadSpriteFont

        [Test]
        public void GivenPipelineContentExists_WhenLoadSpriteFontIsCalled_ThenNoExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.LoadSpriteFont(It.IsAny<string>()));

            Assert.That(
                () => NuciContentManager.Instance.LoadSpriteFont("testSpriteFont"),
                Throws.Nothing);
        }

        [Test]
        public void GivenPipelineContentExists_WhenLoadSpriteFontIsCalled_ThenPipelineLoaderIsCalled()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.LoadSpriteFont(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSpriteFont("testSpriteFont");

            pipelineContentLoaderMock.Verify(loader => loader.LoadSpriteFont("testSpriteFont"), Times.Once);
        }

        [Test]
        public void GivenPipelineContentExists_WhenLoadSpriteFontIsCalled_ThenPlainFileLoaderIsNotCalled()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.LoadSpriteFont(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSpriteFont("testSpriteFont");

            plainFileContentLoaderMock.Verify(loader => loader.LoadSpriteFont(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenPipelineContentExists_WhenLoadSpriteFontIsCalled_ThenPlainFileTryLoadIsNotCalled()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.LoadSpriteFont(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSpriteFont("testSpriteFont");

            plainFileContentLoaderMock.Verify(loader => loader.TryLoadSpriteFont(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenPipelineContentDoesNotExist_WhenLoadSpriteFontIsCalled_ThenExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.LoadSpriteFont(It.IsAny<string>()))
                .Throws<Exception>();

            Assert.That(
                () => NuciContentManager.Instance.LoadSpriteFont("testSpriteFont"),
                Throws.TypeOf<Exception>());
        }

        [TestCase("testSpriteFont")]
        [TestCase("fonts/ui")]
        [TestCase("fonts/hud/score")]
        public void GivenPipelineContentExists_WhenLoadSpriteFontIsCalled_ThenPipelineLoadIsCalledWithCorrectPath(string path)
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.LoadSpriteFont(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSpriteFont(path);

            pipelineContentLoaderMock.Verify(loader => loader.LoadSpriteFont(path), Times.Once);
        }

        // LoadTexture2D — no MissingTexturePlaceholder

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_WhenLoadTexture2DIsCalled_ThenPlainFileLoadIsCalled()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(loader => loader.LoadTexture2D("testTexture"), Times.Once);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_WhenLoadTexture2DIsCalled_ThenPlainFileTryLoadIsNotCalled()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(loader => loader.TryLoadTexture2D(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_AndPlainFileLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenNullIsReturned()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            Texture2D result = NuciContentManager.Instance.LoadTexture2D("testTexture");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_AndPlainFileExists_WhenLoadTexture2DIsCalled_ThenNoExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            Assert.That(
                () => NuciContentManager.Instance.LoadTexture2D("testTexture"),
                Throws.Nothing);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_AndPlainFileDoesNotExist_WhenLoadTexture2DIsCalled_ThenExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()))
                .Throws<Exception>();

            Assert.That(
                () => NuciContentManager.Instance.LoadTexture2D("testTexture"),
                Throws.TypeOf<Exception>());
        }

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_WhenLoadTexture2DIsCalled_ThenPipelineTryLoadIsCalledWithCorrectPath(string path)
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D(path);

            pipelineContentLoaderMock.Verify(loader => loader.TryLoadTexture2D(path), Times.Once);
        }

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_WhenLoadTexture2DIsCalled_ThenPlainFileLoadIsCalledWithCorrectPath(string path)
        {
            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D(path);

            plainFileContentLoaderMock.Verify(loader => loader.LoadTexture2D(path), Times.Once);
        }

        // LoadTexture2D — empty and whitespace MissingTexturePlaceholder (treated as absent)

        [Test]
        public void GivenEmptyMissingTexturePlaceholder_AndPipelineTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPlainFileLoadIsCalled()
        {
            NuciContentManager.MissingTexturePlaceholder = "";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(loader => loader.LoadTexture2D("testTexture"), Times.Once);
        }

        [Test]
        public void GivenEmptyMissingTexturePlaceholder_AndPipelineTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPlainFileTryLoadIsNotCalled()
        {
            NuciContentManager.MissingTexturePlaceholder = "";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(loader => loader.TryLoadTexture2D(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenWhitespaceMissingTexturePlaceholder_AndPipelineTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPlainFileLoadIsCalled()
        {
            NuciContentManager.MissingTexturePlaceholder = "   ";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(loader => loader.LoadTexture2D("testTexture"), Times.Once);
        }

        [Test]
        public void GivenWhitespaceMissingTexturePlaceholder_AndPipelineTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPlainFileTryLoadIsNotCalled()
        {
            NuciContentManager.MissingTexturePlaceholder = "   ";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(loader => loader.TryLoadTexture2D(It.IsAny<string>()), Times.Never);
        }

        // LoadTexture2D — with MissingTexturePlaceholder

        [Test]
        public void GivenMissingTexturePlaceholderIsSet_AndPipelineTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPlainFileTryLoadIsCalled()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(loader => loader.TryLoadTexture2D("testTexture"), Times.Once);
        }

        [Test]
        public void GivenMissingTexturePlaceholderIsSet_AndPipelineTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPlainFileLoadIsNotCalled()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(loader => loader.LoadTexture2D(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenMissingTexturePlaceholderIsSet_AndPipelineTryLoadReturnsNull_AndPlainFileTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPipelineLoadsPlaceholder()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            pipelineContentLoaderMock.Verify(loader => loader.LoadTexture2D("missing_texture"), Times.Once);
        }

        [TestCase("missing_texture")]
        [TestCase("textures/missing")]
        [TestCase("placeholders/default")]
        public void GivenMissingTexturePlaceholderIsSet_AndBothLoadersReturnNull_WhenLoadTexture2DIsCalled_ThenPipelineLoadsWithPlaceholderPath(
            string placeholderPath)
        {
            NuciContentManager.MissingTexturePlaceholder = placeholderPath;

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            pipelineContentLoaderMock.Verify(loader => loader.LoadTexture2D(placeholderPath), Times.Once);
        }

        [Test]
        public void GivenMissingTexturePlaceholderIsSet_AndPipelineTryLoadReturnsNull_AndPlainFileTryLoadReturnsNull_AndPipelinePlaceholderLoadThrows_WhenLoadTexture2DIsCalled_ThenExceptionIsThrown()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            pipelineContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()))
                .Throws<Exception>();

            Assert.That(
                () => NuciContentManager.Instance.LoadTexture2D("testTexture"),
                Throws.TypeOf<Exception>());
        }

        [Test]
        public void GivenMissingTexturePlaceholderIsSet_AndPipelineTryLoadReturnsNull_AndPlainFileTryLoadReturnsNull_AndPipelinePlaceholderLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenNullIsReturned()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            pipelineContentLoaderMock
                .Setup(loader => loader.LoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            Texture2D result = NuciContentManager.Instance.LoadTexture2D("testTexture");

            Assert.That(result, Is.Null);
        }

        [TestCase("testTexture")]
        [TestCase("textures/player")]
        [TestCase("sprites/enemies/goblin")]
        public void GivenMissingTexturePlaceholderIsSet_AndPipelineTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPlainFileTryLoadIsCalledWithCorrectPath(
            string path)
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(loader => loader.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            NuciContentManager.Instance.LoadTexture2D(path);

            plainFileContentLoaderMock.Verify(loader => loader.TryLoadTexture2D(path), Times.Once);
        }
    }
}
