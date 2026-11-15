using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NuciXNA.DataAccess.ArchitectureTests
{
    /// <summary>
    /// Parses handwritten source files once per test run so convention rules avoid repeated I/O.
    /// </summary>
    internal static class SourceFileIndex
    {
        private static readonly Dictionary<string, CompilationUnitSyntax> ParsedFiles = new(StringComparer.Ordinal);

        private static readonly Dictionary<string, SourceText> SourceTexts = new(StringComparer.Ordinal);

        internal static readonly IReadOnlyList<string> AllSourceFiles =
            ArchitectureTestPaths.ProductionSourceFiles
                .Concat(ArchitectureTestPaths.UnitTestsSourceFiles)
                .Concat(ArchitectureTestPaths.ArchitectureTestsSourceFiles)
                .ToArray();

        internal static CompilationUnitSyntax Root(string filePath)
        {
            if (!ParsedFiles.TryGetValue(filePath, out CompilationUnitSyntax? root))
            {
                root = CSharpSyntaxTree
                    .ParseText(File.ReadAllText(filePath))
                    .GetCompilationUnitRoot();

                ParsedFiles[filePath] = root;
            }

            return root;
        }

        internal static SourceText Text(string filePath)
        {
            if (!SourceTexts.TryGetValue(filePath, out SourceText? text))
            {
                text = new SourceText(filePath, File.ReadAllLines(filePath));

                SourceTexts[filePath] = text;
            }

            return text;
        }

        internal static int LineNumber(CompilationUnitSyntax root, SyntaxNode node)
            => root.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line + 1;

        internal static string Display(string filePath)
            => ArchitectureTestPaths.ToRelativePortablePath(filePath);
    }
}