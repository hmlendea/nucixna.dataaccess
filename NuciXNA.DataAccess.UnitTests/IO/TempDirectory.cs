using System;
using System.IO;

namespace NuciXNA.DataAccess.UnitTests.IO
{
    /// <summary>Creates a uniquely named temporary directory and deletes it on disposal.</summary>
    internal sealed class TempDirectory : IDisposable
    {
        internal string Path { get; }

        internal TempDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());

            Directory.CreateDirectory(Path);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, true);
            }
        }
    }
}