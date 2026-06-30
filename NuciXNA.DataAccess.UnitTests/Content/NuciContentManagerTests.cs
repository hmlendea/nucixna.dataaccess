using System;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Moq;
using NUnit.Framework;

using NuciXNA.DataAccess.Content;

namespace NuciXNA.DataAccess.UnitTests.Content
{
    public class NuciContentManagerTests
    {
        Mock<IContentLoader> pipelineContentLoaderMock;
        Mock<IContentLoader> plainFileContentLoaderMock;

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

        // LoadSoundEffect

        [Test]
        public void GivenPipelineContentExists_AndPlainFileExists_WhenLoadSoundEffectIsCalled_ThenNoExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(x => x.LoadSoundEffect(It.IsAny<string>()));

            plainFileContentLoaderMock
                .Setup(x => x.LoadSoundEffect(It.IsAny<string>()));

            Assert.DoesNotThrow(() => NuciContentManager.Instance.LoadSoundEffect("testSoundEffect"));
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_WhenLoadSoundEffectIsCalled_ThenPlainFileLoaderIsCalled()
        {
            pipelineContentLoaderMock
                .Setup(x => x.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(x => x.LoadSoundEffect(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSoundEffect("testSoundEffect");

            plainFileContentLoaderMock.Verify(x => x.LoadSoundEffect("testSoundEffect"), Times.Once);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndPlainFileExists_WhenLoadSoundEffectIsCalled_ThenNoExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(x => x.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(x => x.LoadSoundEffect(It.IsAny<string>()));

            Assert.DoesNotThrow(() => NuciContentManager.Instance.LoadSoundEffect("testSoundEffect"));
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndPlainFileDoesNotExist_WhenLoadSoundEffectIsCalled_ThenExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(x => x.TryLoadSoundEffect(It.IsAny<string>()))
                .Returns((SoundEffect)null);

            plainFileContentLoaderMock
                .Setup(x => x.LoadSoundEffect(It.IsAny<string>()))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => NuciContentManager.Instance.LoadSoundEffect("testSoundEffect"));
        }

        // LoadSpriteFont

        [Test]
        public void GivenPipelineContentExists_WhenLoadSpriteFontIsCalled_ThenNoExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(x => x.LoadSpriteFont(It.IsAny<string>()));

            Assert.DoesNotThrow(() => NuciContentManager.Instance.LoadSpriteFont("testSpriteFont"));
        }

        [Test]
        public void GivenPipelineContentExists_WhenLoadSpriteFontIsCalled_ThenPipelineLoaderIsCalled()
        {
            pipelineContentLoaderMock
                .Setup(x => x.LoadSpriteFont(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSpriteFont("testSpriteFont");

            pipelineContentLoaderMock.Verify(x => x.LoadSpriteFont("testSpriteFont"), Times.Once);
        }

        [Test]
        public void GivenPipelineContentExists_WhenLoadSpriteFontIsCalled_ThenPlainFileLoaderIsNotCalled()
        {
            pipelineContentLoaderMock
                .Setup(x => x.LoadSpriteFont(It.IsAny<string>()));

            NuciContentManager.Instance.LoadSpriteFont("testSpriteFont");

            plainFileContentLoaderMock.Verify(x => x.LoadSpriteFont(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenPipelineContentDoesNotExist_WhenLoadSpriteFontIsCalled_ThenExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(x => x.LoadSpriteFont(It.IsAny<string>()))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => NuciContentManager.Instance.LoadSpriteFont("testSpriteFont"));
        }

        // LoadTexture2D — no MissingTexturePlaceholder

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_WhenLoadTexture2DIsCalled_ThenPlainFileLoaderIsCalled()
        {
            pipelineContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(x => x.LoadTexture2D(It.IsAny<string>()));

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(x => x.LoadTexture2D("testTexture"), Times.Once);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_AndPlainFileExists_WhenLoadTexture2DIsCalled_ThenNoExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(x => x.LoadTexture2D(It.IsAny<string>()));

            Assert.DoesNotThrow(() => NuciContentManager.Instance.LoadTexture2D("testTexture"));
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndNoMissingTexturePlaceholder_AndPlainFileDoesNotExist_WhenLoadTexture2DIsCalled_ThenExceptionIsThrown()
        {
            pipelineContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(x => x.LoadTexture2D(It.IsAny<string>()))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => NuciContentManager.Instance.LoadTexture2D("testTexture"));
        }

        // LoadTexture2D — with MissingTexturePlaceholder

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndMissingTexturePlaceholderIsSet_WhenLoadTexture2DIsCalled_ThenPlainFileTryLoadIsCalled()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(x => x.TryLoadTexture2D("testTexture"), Times.Once);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndMissingTexturePlaceholderIsSet_WhenLoadTexture2DIsCalled_ThenPlainFileLoadIsNotCalled()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            plainFileContentLoaderMock.Verify(x => x.LoadTexture2D(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndMissingTexturePlaceholderIsSet_AndPlainFileTryLoadReturnsNull_WhenLoadTexture2DIsCalled_ThenPipelineLoadsPlaceholder()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            NuciContentManager.Instance.LoadTexture2D("testTexture");

            pipelineContentLoaderMock.Verify(x => x.LoadTexture2D("missing_texture"), Times.Once);
        }

        [Test]
        public void GivenPipelineTryLoadReturnsNull_AndMissingTexturePlaceholderIsSet_AndPlainFileTryLoadReturnsNull_AndPipelinePlaceholderLoadThrows_WhenLoadTexture2DIsCalled_ThenExceptionIsThrown()
        {
            NuciContentManager.MissingTexturePlaceholder = "missing_texture";

            pipelineContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            plainFileContentLoaderMock
                .Setup(x => x.TryLoadTexture2D(It.IsAny<string>()))
                .Returns((Texture2D)null);

            pipelineContentLoaderMock
                .Setup(x => x.LoadTexture2D(It.IsAny<string>()))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => NuciContentManager.Instance.LoadTexture2D("testTexture"));
        }
    }
}
