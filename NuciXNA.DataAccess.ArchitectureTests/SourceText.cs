using System.Collections.Generic;

namespace NuciXNA.DataAccess.ArchitectureTests
{
    /// <summary>The raw lines of a parsed source file.</summary>
    internal sealed class SourceText
    {
        internal SourceText(string filePath, IReadOnlyList<string> lines)
        {
            FilePath = filePath;
            Lines = lines;
        }

        internal string FilePath { get; }

        internal IReadOnlyList<string> Lines { get; }

        internal string this[int lineNumber] => Lines[lineNumber - 1];
    }
}