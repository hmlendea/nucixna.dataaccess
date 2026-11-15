using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciXNA.DataAccess.ArchitectureTests
{
    /// <summary>
    /// Enforces the unit-test and architecture-test source conventions.
    /// </summary>
    [TestFixture]
    public sealed class TestSourceConventionTests
    {
        [Test]
        public void GivenTestSources_WhenInspected_ThenNoNestedTypeIsDeclared()
        {
            string[] violations = TestSourceViolations(root => root.DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>()
                .Any(declaration => declaration.Parent is BaseTypeDeclarationSyntax));

            Assert.That(violations, Is.Empty, "Test helper types must be top-level internal types in separate files.");
        }

        [Test]
        public void GivenTestSources_WhenInspected_ThenTestClassesEndWithTests()
        {
            string[] violations = TestSourceViolations(root => root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(IsFixtureClass)
                .Any(classDeclaration => !classDeclaration.Identifier.ValueText.EndsWith("Tests", StringComparison.Ordinal)));

            Assert.That(violations, Is.Empty, "Test classes must be named '<Subject>Tests'.");
        }

        [Test]
        public void GivenTestSources_WhenInspected_ThenFixtureClassesAreSealed()
        {
            string[] violations = TestSourceViolations(root => root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(IsFixtureClass)
                .Any(classDeclaration => !classDeclaration.Modifiers.Any(SyntaxKind.SealedKeyword)));

            Assert.That(violations, Is.Empty, "Test fixture classes must be sealed.");
        }

        [Test]
        public void GivenTestSources_WhenInspected_ThenTestMethodsFollowTheGivenWhenThenForm()
        {
            string[] violations = [];

            foreach (string file in TestSourceFiles())
            {
                CompilationUnitSyntax root = SourceFileIndex.Root(file);

                foreach (MethodDeclarationSyntax method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    if (!IsTestMethod(method))
                    {
                        violations =
                        [
                            .. violations,
                            $"{SourceFileIndex.Display(file)}:{SourceFileIndex.LineNumber(root, method)} {method.Identifier.ValueText}",
                        ];
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Unit-test methods must follow Given[x]_When[y]_Then[z].");
        }

        [Test]
        public void GivenTestSources_WhenInspected_ThenParameterisedTestsDeclareAtLeastTwoCases()
        {
            string[] violations = [];

            foreach (string file in TestSourceFiles())
            {
                CompilationUnitSyntax root = SourceFileIndex.Root(file);

                foreach (MethodDeclarationSyntax method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    int caseCount = method.AttributeLists
                        .SelectMany(list => list.Attributes)
                        .Count(attribute => attribute.Name.ToString().EndsWith("TestCase", StringComparison.Ordinal));

                    if (caseCount == 1)
                    {
                        violations =
                        [
                            .. violations,
                            $"{SourceFileIndex.Display(file)}:{SourceFileIndex.LineNumber(root, method)} {method.Identifier.ValueText}",
                        ];
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Parameterised tests must declare at least two cases.");
        }

        [Test]
        public void GivenTestSources_WhenInspected_ThenNoLegacyAssertionApiIsUsed()
        {
            string[] legacyAssertions =
            [
                "Assert.Are" + "Equal", "Assert.Are" + "NotEqual", "Assert.Is" + "True", "Assert.Is" + "False",
                "Assert.Is" + "Null", "Assert.Is" + "NotNull", "Assert." + "Greater", "Assert." + "Less",
            ];

            string[] violations = [];

            foreach (string file in TestSourceFiles())
            {
                SourceText text = SourceFileIndex.Text(file);

                for (int line = 1; line <= text.Lines.Count; line += 1)
                {
                    string content = text[line];

                    string[] used = legacyAssertions
                        .Where(assertion => content.Contains(assertion, StringComparison.Ordinal))
                        .ToArray();

                    if (used.Length > 0)
                    {
                        violations = [.. violations, $"{SourceFileIndex.Display(file)}:{line} uses {string.Join(", ", used)}"];
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Use the 'Assert.That(...)' constraint model only.");
        }

        [Test]
        public void GivenTestSources_WhenInspected_ThenReflectionIsNotUsed()
        {
            string[] reflectionApis =
            [
                "BindingFlags", "GetMethod(", "GetField(", "GetProperty(", "Activator.CreateInstance",
            ];

            string[] violations = [];

            foreach (string file in ArchitectureTestPaths.UnitTestsSourceFiles)
            {
                SourceText text = SourceFileIndex.Text(file);

                for (int line = 1; line <= text.Lines.Count; line += 1)
                {
                    string content = text[line];

                    string[] used = reflectionApis
                        .Where(api => content.Contains(api, StringComparison.Ordinal))
                        .ToArray();

                    if (used.Length > 0)
                    {
                        violations = [.. violations, $"{SourceFileIndex.Display(file)}:{line}"];
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Unit tests must not use reflection to reach non-public code.");
        }

        [Test]
        public void GivenUnitTestProject_WhenInspected_ThenNoInternalsVisibleToIsRequired()
        {
            Assert.That(
                ArchitectureTestPaths.UnitTestsSourceFiles.Any(file =>
                    SourceFileIndex.Text(file).Lines.Any(line => line.Contains("InternalsVisibleTo", StringComparison.Ordinal))),
                Is.False,
                "Unit tests must not require InternalsVisibleTo.");
        }

        private static string[] TestSourceViolations(Func<CompilationUnitSyntax, bool> predicate)
            => TestSourceFiles()
                .Where(file => predicate(SourceFileIndex.Root(file)))
                .Select(SourceFileIndex.Display)
                .ToArray();

        private static IReadOnlyList<string> TestSourceFiles()
            => ArchitectureTestPaths.UnitTestsSourceFiles
                .Concat(ArchitectureTestPaths.ArchitectureTestsSourceFiles)
                .ToArray();

        private static bool IsFixtureClass(ClassDeclarationSyntax classDeclaration)
            => classDeclaration.AttributeLists
                .SelectMany(list => list.Attributes)
                .Any(attribute => attribute.Name.ToString().EndsWith("TestFixture", StringComparison.Ordinal));

        private static bool IsTestMethod(MethodDeclarationSyntax method)
        {
            if (!method.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                return true;
            }

            string name = method.Identifier.ValueText;

            bool isTheoryAttribute = method.AttributeLists
                .SelectMany(list => list.Attributes)
                .Any(attribute => attribute.Name.ToString().EndsWith("Test", StringComparison.Ordinal));

            if (!isTheoryAttribute)
            {
                return true;
            }

            return name.Contains("_When", StringComparison.Ordinal)
                && name.Contains("_Then", StringComparison.Ordinal)
                && name.StartsWith("Given", StringComparison.Ordinal);
        }
    }
}