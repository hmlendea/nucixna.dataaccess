using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NuciXNA.DataAccess.ArchitectureTests
{
    /// <summary>
    /// Resolves repository locations from build-time assembly metadata instead of
    /// discovering them from the current directory or the local file system layout.
    /// </summary>
    internal static class ArchitectureTestPaths
    {
        internal static readonly string RepositoryRoot = ResolveRepositoryRoot();

        internal static readonly string ProductionProjectDirectory =
            Path.Combine(RepositoryRoot, "NuciXNA.DataAccess");

        internal static readonly string ProductionProjectFile =
            Path.Combine(ProductionProjectDirectory, "NuciXNA.DataAccess.csproj");

        internal static readonly string UnitTestsProjectDirectory =
            Path.Combine(RepositoryRoot, "NuciXNA.DataAccess.UnitTests");

        internal static readonly string UnitTestsProjectFile =
            Path.Combine(UnitTestsProjectDirectory, "NuciXNA.DataAccess.UnitTests.csproj");

        internal static readonly string ArchitectureTestsProjectDirectory =
            Path.Combine(RepositoryRoot, "NuciXNA.DataAccess.ArchitectureTests");

        internal static readonly string ArchitectureTestsProjectFile = Path.Combine(
            ArchitectureTestsProjectDirectory,
            "NuciXNA.DataAccess.ArchitectureTests.csproj");

        internal static readonly string SolutionFile =
            Path.Combine(RepositoryRoot, "NuciXNA.DataAccess.slnx");

        internal static readonly IReadOnlyList<string> ProductionSourceFiles =
            CollectSourceFiles(ProductionProjectDirectory);

        internal static readonly IReadOnlyList<string> UnitTestsSourceFiles =
            CollectSourceFiles(UnitTestsProjectDirectory);

        internal static readonly IReadOnlyList<string> ArchitectureTestsSourceFiles =
            CollectSourceFiles(ArchitectureTestsProjectDirectory);

        internal static string ToRelativePortablePath(string filePath)
            => ToPortablePath(Path.GetRelativePath(RepositoryRoot, filePath));

        private static string ToPortablePath(string path)
            => path
                .Replace(Path.DirectorySeparatorChar, '/')
                .Replace(Path.AltDirectorySeparatorChar, '/');

        private static IReadOnlyList<string> CollectSourceFiles(string projectDirectory)
            => Directory
                .GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(file => !IsInIgnoredDirectory(file))
                .Where(file => !IsGeneratedFile(Path.GetFileName(file)))
                .OrderBy(file => file, StringComparer.Ordinal)
                .ToArray();

        private static bool IsInIgnoredDirectory(string filePath)
        {
            char[] separators =
            [
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar,
            ];

            string[] segments = filePath.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            return segments.Contains("bin") || segments.Contains("obj");
        }

        private static bool IsGeneratedFile(string fileName)
            => fileName.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".Generated.cs", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)
            || fileName.StartsWith("AssemblyInfo", StringComparison.OrdinalIgnoreCase);

        private static string ResolveRepositoryRoot()
        {
            AssemblyMetadataAttribute[] metadataAttributes = typeof(ArchitectureTestPaths)
                .Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(attribute => string.Equals(attribute.Key, "RepositoryRoot", StringComparison.Ordinal))
                .ToArray();

            if (metadataAttributes.Length == 0 || string.IsNullOrWhiteSpace(metadataAttributes[0].Value))
            {
                throw new InvalidOperationException("The RepositoryRoot assembly metadata value is unavailable.");
            }

            return Path.GetFullPath(metadataAttributes[0].Value!);
        }
    }
}
