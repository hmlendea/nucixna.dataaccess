using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciXNA.DataAccess.ArchitectureTests
{
    /// <summary>
    /// Enforces source-file conventions across the handwritten production and test sources.
    /// </summary>
    [TestFixture]
    public sealed class SourceFileConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenCollected_ThenAtLeastOneFileIsAvailable()
            => Assert.That(
                ArchitectureTestPaths.ProductionSourceFiles,
                Is.Not.Empty,
                "No handwritten production source files were discovered.");

        [Test]
        public void GivenUnitTestSources_WhenCollected_ThenAtLeastOneFileIsAvailable()
            => Assert.That(
                ArchitectureTestPaths.UnitTestsSourceFiles,
                Is.Not.Empty,
                "No handwritten unit-test source files were discovered.");

        [Test]
        public void GivenArchitectureTestSources_WhenCollected_ThenAtLeastOneFileIsAvailable()
            => Assert.That(
                ArchitectureTestPaths.ArchitectureTestsSourceFiles,
                Is.Not.Empty,
                "No handwritten architecture-test source files were discovered.");

        [Test]
        public void GivenEverySourceFile_WhenTypeDeclarationsAreCounted_ThenExactlyOneIsDeclared()
        {
            string[] violations = AllSourceFiles()
                .Where(file => DeclaredTypeCount(file) != 1)
                .Select(file => $"{ArchitectureTestPaths.ToRelativePortablePath(file)} declares {DeclaredTypeCount(file)} top-level types.")
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEverySourceFile_WhenFileNameIsComparedToTypeName_ThenTheyMatchExactly()
        {
            string[] violations = AllSourceFiles()
                .Where(file => DeclaredTypeCount(file) == 1)
                .Where(file => !string.Equals(
                    SoleTypeName(file),
                    Path.GetFileNameWithoutExtension(file),
                    StringComparison.Ordinal))
                .Select(file => $"{ArchitectureTestPaths.ToRelativePortablePath(file)} declares '{SoleTypeName(file)}'.")
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEverySourceFile_WhenNamespaceIsInspected_ThenItIsDeclaredAsABlock()
        {
            string[] violations = AllSourceFiles()
                .Where(file => !HasBlockNamespace(Parse(file)))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEverySourceFile_WhenUsingsAreInspected_ThenNoNullableDirectiveIsPresent()
        {
            string[] violations = AllSourceFiles()
                .Where(file => HasNullableDirective(Parse(file)))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEverySourceFile_WhenStructureIsInspected_ThenExactlyOneBlankLinePrecedesTheNamespace()
        {
            string[] violations = [];

            foreach (string file in AllSourceFiles())
            {
                string[] fileLines = File.ReadAllLines(file);
                int namespaceIndex = Array.FindIndex(fileLines, line => line.TrimStart().StartsWith("namespace ", StringComparison.Ordinal));

                if (namespaceIndex < 1)
                {
                    violations = [.. violations, $"{ArchitectureTestPaths.ToRelativePortablePath(file)} has no namespace."];
                    continue;
                }

                bool previousIsBlank = string.IsNullOrWhiteSpace(fileLines[namespaceIndex - 1]);
                bool twoBackIsBlank = namespaceIndex >= 2 && string.IsNullOrWhiteSpace(fileLines[namespaceIndex - 2]);

                if (!previousIsBlank || twoBackIsBlank)
                {
                    violations = [.. violations, $"{ArchitectureTestPaths.ToRelativePortablePath(file)} namespace spacing is invalid."];
                }
            }

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEverySourceFile_WhenUsingsAreInspected_ThenEachGroupIsAlphabetised()
        {
            string[] violations = [];

            foreach (string file in AllSourceFiles())
            {
                IReadOnlyList<string> lines = File.ReadAllLines(file);
                string[] sections = CollectUsingSections(lines);

                foreach (string section in sections)
                {
                    string[] namespaces = section
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Trim().Replace("using ", string.Empty, StringComparison.Ordinal).TrimEnd(';'))
                        .ToArray();

                    if (!namespaces.SequenceEqual(namespaces.OrderBy(entry => entry, StringComparer.Ordinal)))
                    {
                        violations = [.. violations, $"{ArchitectureTestPaths.ToRelativePortablePath(file)} has unsorted usings."];
                    }
                }
            }

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEverySourceFile_WhenUsingsAreInspected_ThenNoInlineFullyQualifiedTypeIsUsed()
        {
            string[] violations = [];

            foreach (string file in AllSourceFiles())
            {
                CompilationUnitSyntax root = Parse(file);

                foreach (QualifiedNameSyntax qualifiedName in root.DescendantNodes().OfType<QualifiedNameSyntax>())
                {
                    if (qualifiedName.Ancestors().OfType<UsingDirectiveSyntax>().Any()
                        || qualifiedName.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Any())
                    {
                        continue;
                    }

                    string text = qualifiedName.ToString();

                    if (QualifiedNamespaces().Any(candidate => text.StartsWith($"{candidate}.", StringComparison.Ordinal)))
                    {
                        violations = [.. violations, $"{ArchitectureTestPaths.ToRelativePortablePath(file)}:{LineNumber(root, qualifiedName)} {text}"];
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Add a using directive and use the short type name.");
        }

        [Test]
        public void GivenEveryInterface_WhenNamed_ThenItUsesTheIPascalCaseConvention()
        {
            string[] violations = [];

            foreach (string file in AllSourceFiles())
            {
                CompilationUnitSyntax root = Parse(file);

                foreach (InterfaceDeclarationSyntax interfaceDeclaration in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                {
                    string name = interfaceDeclaration.Identifier.ValueText;

                    if (name.Length < 2 || name[0] != 'I' || !char.IsUpper(name[1]))
                    {
                        violations = [.. violations, $"{ArchitectureTestPaths.ToRelativePortablePath(file)}:{LineNumber(root, interfaceDeclaration)} {name}"];
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Interfaces must use the I-prefixed PascalCase convention.");
        }

        private static string[] CollectUsingSections(IReadOnlyList<string> lines)
        {
            List<string> sections = [];
            List<string> current = [];

            foreach (string line in lines)
            {
                if (line.TrimStart().StartsWith("namespace ", StringComparison.Ordinal))
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (current.Count > 0)
                    {
                        sections.Add(string.Join('\n', current));
                        current.Clear();
                    }

                    continue;
                }

                if (line.TrimStart().StartsWith("using ", StringComparison.Ordinal))
                {
                    current.Add(line);
                }
            }

            if (current.Count > 0)
            {
                sections.Add(string.Join('\n', current));
            }

            return [.. sections];
        }

        private static IReadOnlyList<string> QualifiedNamespaces() =>
        [
            "System", "Microsoft", "NuciXNA", "SixLabors", "NUnit",
        ];

        private static IReadOnlyList<string> AllSourceFiles()
            => ArchitectureTestPaths.ProductionSourceFiles
                .Concat(ArchitectureTestPaths.UnitTestsSourceFiles)
                .Concat(ArchitectureTestPaths.ArchitectureTestsSourceFiles)
                .ToArray();

        private static CompilationUnitSyntax Parse(string filePath) =>
            CSharpSyntaxTree.ParseText(File.ReadAllText(filePath)).GetCompilationUnitRoot();

        private static TypeDeclarationSyntax[] TopLevelTypeDeclarations(string filePath)
            => Parse(filePath)
                .DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .Where(declaration => !(declaration.Parent is TypeDeclarationSyntax))
                .ToArray();

        private static int LineNumber(CompilationUnitSyntax root, SyntaxNode node)
            => root.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line + 1;

        private static int DeclaredTypeCount(string filePath) => TopLevelTypeDeclarations(filePath).Length;

        private static string SoleTypeName(string filePath) => TopLevelTypeDeclarations(filePath)[0].Identifier.ValueText;

        private static bool HasBlockNamespace(CompilationUnitSyntax root)
            => root.Members.Any(member => member is BaseNamespaceDeclarationSyntax);

        private static bool HasNullableDirective(CompilationUnitSyntax root)
            => root
                .DescendantTrivia()
                .Any(trivia => trivia.ToString().TrimStart().StartsWith("#nullable", StringComparison.Ordinal));
    }
}
