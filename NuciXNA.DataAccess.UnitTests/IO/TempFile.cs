using System;
using System.IO;

namespace NuciXNA.DataAccess.UnitTests.IO
{
    /// <summary>Creates a uniquely named temporary file and deletes it on disposal.</summary>
    internal sealed class TempFile : IDisposable
    {
        internal string Path { get; }

        internal TempFile(string extension)
        {
            Path = System.IO.Path.GetTempFileName();

            if (!string.IsNullOrEmpty(extension))
            {
                string newPath = System.IO.Path.ChangeExtension(Path, extension);

                File.Move(Path, newPath);

                Path = newPath;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }
    }
}