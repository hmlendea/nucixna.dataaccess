using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using NUnit.Framework;

namespace NuciXNA.DataAccess.ArchitectureTests
{
    /// <summary>
    /// Enforces the project and solution configuration conventions of the repository.
    /// </summary>
    [TestFixture]
    public sealed class ProjectConventionTests
    {
        private static string Net10TargetFramework => "net10.0";

        [Test]
        public void GivenEveryProject_WhenTargetFrameworkIsRead_ThenItIsNet10()
        {
            string[] violations = ProjectFiles()
                .Where(projectFile => !string.Equals(TargetFramework(projectFile), Net10TargetFramework, StringComparison.Ordinal))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEveryProject_WhenImplicitUsingsAreRead_ThenTheyAreDisabled()
        {
            string[] violations = ProjectFiles()
                .Where(projectFile => !string.Equals(Property(projectFile, "ImplicitUsings"), "disable", StringComparison.Ordinal))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEveryProject_WhenLanguageVersionIsRead_ThenItIsLatest()
        {
            string[] violations = ProjectFiles()
                .Where(projectFile => !string.Equals(Property(projectFile, "LangVersion"), "latest", StringComparison.Ordinal))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEveryProject_WhenNullableReferenceTypesAreRead_ThenTheyAreEnabled()
        {
            string[] violations = ProjectFiles()
                .Where(projectFile => !string.Equals(Property(projectFile, "Nullable"), "enable", StringComparison.Ordinal))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenProductionProject_WhenPackabilityIsRead_ThenItIsPackable()
            => Assert.That(
                Property(ArchitectureTestPaths.ProductionProjectFile, "IsPackable"),
                Is.Not.EqualTo("false"));

        [Test]
        public void GivenTestProjects_WhenTestFlagsAreRead_ThenTheyAreMarkedAndExcluded()
        {
            string[] testProjectFiles =
            [
                ArchitectureTestPaths.UnitTestsProjectFile,
                ArchitectureTestPaths.ArchitectureTestsProjectFile,
            ];

            string[] violations = testProjectFiles
                .Where(projectFile => !string.Equals(Property(projectFile, "IsPackable"), "false", StringComparison.Ordinal))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenTestProjects_WhenCoverageFlagsAreRead_ThenTheyAreExcludedFromCoverage()
        {
            string[] testProjectFiles =
            [
                ArchitectureTestPaths.UnitTestsProjectFile,
                ArchitectureTestPaths.ArchitectureTestsProjectFile,
            ];

            string[] violations = testProjectFiles
                .Where(projectFile => !string.Equals(Property(projectFile, "ExcludeFromCodeCoverage"), "true", StringComparison.Ordinal))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenEveryProject_WhenPackageReferencesAreRead_ThenTheyAreAlphabeticallyOrdered()
        {
            string[] violations = ProjectFiles()
                .Where(projectFile => !PackageReferencesAreOrdered(projectFile))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void GivenSolutionFile_WhenStructureIsInspected_ThenItIsSdkStyleWithoutDeclarationOrAttributes()
        {
            string content = File.ReadAllText(ArchitectureTestPaths.SolutionFile).Trim();

            Assert.That(content.StartsWith("<Solution>", StringComparison.Ordinal), Is.True);
            Assert.That(content.EndsWith("</Solution>", StringComparison.Ordinal), Is.True);
            Assert.That(content.Contains("<?xml", StringComparison.Ordinal), Is.False);
            Assert.That(content.Contains("<Version>", StringComparison.Ordinal), Is.False);
        }

        [Test]
        public void GivenSolutionFile_WhenProjectOrderIsInspected_ThenMainPrecedesTests()
        {
            string[] projectPaths = XDocument.Load(ArchitectureTestPaths.SolutionFile)
                .Descendants("Project")
                .Select(element => element.Attribute("Path")?.Value)
                .Where(path => path is not null)
                .Select(path => path!)
                .ToArray();

            int productionIndex = Array.FindIndex(projectPaths, path => path.Contains("NuciXNA.DataAccess/NuciXNA.DataAccess.csproj", StringComparison.Ordinal));
            int unitTestsIndex = Array.FindIndex(projectPaths, path => path.Contains("UnitTests", StringComparison.Ordinal));

            Assert.That(productionIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(productionIndex, Is.LessThan(unitTestsIndex));
        }

        [Test]
        public void GivenRepository_WhenSolutionFilesAreInspected_ThenOnlySlnxIsPresent()
        {
            string[] legacySolutions = Directory
                .GetFiles(ArchitectureTestPaths.RepositoryRoot, "*.sln", SearchOption.TopDirectoryOnly);

            Assert.That(
                legacySolutions.Select(ArchitectureTestPaths.ToRelativePortablePath),
                Is.Empty,
                ".slnx solutions are preferred; legacy .sln files are prohibited.");
        }

        [Test]
        public void GivenEveryProject_WhenFolderIsInspected_ThenItIsASiblingFolderNamedAfterTheProject()
        {
            string[] violations = ProjectFiles()
                .Where(projectFile => !string.Equals(
                    Path.GetFileNameWithoutExtension(projectFile),
                    Path.GetFileName(Path.GetDirectoryName(projectFile)),
                    StringComparison.Ordinal))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty, "Each project must reside in a sibling folder named after it.");
        }

        [Test]
        public void GivenRepository_WhenSourceNestingIsInspected_ThenNoSrcFolderExists()
        {
            Assert.That(
                Directory.Exists(Path.Combine(ArchitectureTestPaths.RepositoryRoot, "src")),
                Is.False,
                "A 'src' nesting layer is prohibited.");
        }

        [Test]
        public void GivenEveryProject_WhenRootNamespaceIsInspected_ThenItMatchesTheProjectName()
        {
            string[] violations = ProjectFiles()
                .Where(projectFile => !string.Equals(
                    Property(projectFile, "RootNamespace"),
                    Path.GetFileNameWithoutExtension(projectFile),
                    StringComparison.Ordinal))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(violations, Is.Empty, "RootNamespace must match the project (and assembly) name.");
        }

        [Test]
        public void GivenSolutionFile_WhenProjectsAreInspected_ThenEachEntryIsAPathOnlyElement()
        {
            XDocument solution = XDocument.Load(ArchitectureTestPaths.SolutionFile);

            XElement[] projectsWithExtraAttributes = solution
                .Descendants("Project")
                .Where(element => element.Attributes().Any(attribute => attribute.Name != "Path"))
                .ToArray();

            Assert.That(projectsWithExtraAttributes, Is.Empty, "Solution project entries may declare only 'Path'.");
        }

        [Test]
        public void GivenSolutionFile_WhenOrderIsInspected_ThenTestsFollowSourceProjectsAlphabetically()
        {
            string[] projectPaths = XDocument.Load(ArchitectureTestPaths.SolutionFile)
                .Descendants("Project")
                .Select(element => element.Attribute("Path")?.Value)
                .Where(path => path is not null)
                .Select(path => path!)
                .ToArray();

            int firstTestIndex = Array.FindIndex(projectPaths, path => path.Contains("Tests", StringComparison.Ordinal));

            Assert.That(firstTestIndex, Is.GreaterThan(0), "The solution must contain test projects after the source project.");

            string[] sourceProjects = projectPaths[..firstTestIndex];
            string[] testProjects = projectPaths[firstTestIndex..];

            Assert.That(
                sourceProjects.SequenceEqual(sourceProjects.OrderBy(path => path, StringComparer.OrdinalIgnoreCase)),
                Is.True,
                "Source projects must be alphabetically ordered.");

            Assert.That(
                testProjects.SequenceEqual(testProjects.OrderBy(path => path, StringComparer.OrdinalIgnoreCase)),
                Is.True,
                "Test projects must be alphabetically ordered.");
        }

        private static string[] ProjectFiles()
        {
            string[] projectFiles =
            [
                ArchitectureTestPaths.ProductionProjectFile,
                ArchitectureTestPaths.UnitTestsProjectFile,
                ArchitectureTestPaths.ArchitectureTestsProjectFile,
            ];

            return projectFiles;
        }

        private static string TargetFramework(string projectFile)
            => Property(projectFile, "TargetFramework");

        private static string Property(string projectFile, string propertyName)
            => XDocument.Load(projectFile)
                .Descendants(propertyName)
                .Select(element => element.Value.Trim())
                .FirstOrDefault() ?? string.Empty;

        private static bool PackageReferencesAreOrdered(string projectFile)
        {
            string[] packages = XDocument.Load(projectFile)
                .Descendants("PackageReference")
                .Select(element => element.Attribute("Include")?.Value)
                .Where(include => include is not null)
                .Select(include => include!)
                .ToArray();

            string[] orderedPackages = packages
                .OrderBy(package => package, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return packages.SequenceEqual(orderedPackages);
        }
    }
}
