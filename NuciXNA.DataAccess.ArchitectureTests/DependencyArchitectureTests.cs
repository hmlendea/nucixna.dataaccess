using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Slices;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;

using NUnit.Framework;

using NuciXNA.DataAccess.Content;

using Assembly = System.Reflection.Assembly;

namespace NuciXNA.DataAccess.ArchitectureTests
{
    /// <summary>
    /// Enforces the compiled dependency boundaries and reference rules of the production project.
    /// </summary>
    [TestFixture]
    public sealed class DependencyArchitectureTests
    {
        private static string ContentNamespace => "NuciXNA.DataAccess.Content";
        private static string IONamespace => "NuciXNA.DataAccess.IO";

        private static readonly Architecture ProductionArchitecture = new ArchLoader()
            .LoadAssemblies(ProductionAssembly())
            .Build();

        [Test]
        public void GivenProductionAssembly_WhenContentNamespaceSelected_ThenAtLeastOneTypeExists()
            => Assert.That(
                ContentTypeCount(),
                Is.GreaterThan(0),
                "No production type resides in the Content namespace.");

        [Test]
        public void GivenProductionAssembly_WhenIONamespaceSelected_ThenAtLeastOneTypeExists()
            => Assert.That(
                IONamespaceTypeCount(),
                Is.GreaterThan(0),
                "No production type resides in the IO namespace.");

        [Test]
        public void GivenProductionAssembly_WhenEveryTypeIsChecked_ThenItResidesInAnAllowedNamespace()
        {
            IArchRule rule = ArchRuleDefinition.Types()
                .That().HaveFullNameStartingWith("NuciXNA.DataAccess.")
                .Should().ResideInNamespace(ContentNamespace)
                .OrShould().ResideInNamespace(IONamespace)
                .Because("production types must reside in a documented subsystem namespace.");

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenContentTypes_WhenDependenciesAreChecked_ThenTheyDoNotDependOnIOTypes()
        {
            IArchRule rule = ArchRuleDefinition.Types()
                .That().ResideInNamespace(ContentNamespace)
                .Should().NotDependOnAnyTypesThat()
                .ResideInNamespace(IONamespace)
                .Because("the content subsystem must not depend on the bitmap subsystem.");

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenIOTypes_WhenDependenciesAreChecked_ThenTheyDoNotDependOnContentTypes()
        {
            IArchRule rule = ArchRuleDefinition.Types()
                .That().ResideInNamespace(IONamespace)
                .Should().NotDependOnAnyTypesThat()
                .ResideInNamespace(ContentNamespace)
                .Because("the bitmap subsystem must not depend on the content subsystem.");

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenProductionSubsystems_WhenCyclesAreChecked_ThenNoneExists()
        {
            SliceRule rule = SliceRuleDefinition.Slices()
                .Matching("NuciXNA.DataAccess.(*)")
                .Should().BeFreeOfCycles();

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenProductionAssembly_WhenInternalsVisibleToIsChecked_ThenNoneIsDeclared()
        {
            InternalsVisibleToAttribute[] internalsVisibleTo = ProductionAssembly()
                .GetCustomAttributes(typeof(InternalsVisibleToAttribute), false)
                .Cast<InternalsVisibleToAttribute>()
                .ToArray();

            Assert.That(
                internalsVisibleTo,
                Is.Empty,
                "Production code must not declare InternalsVisibleTo for test projects.");
        }

        [Test]
        public void GivenProductionProject_WhenProjectReferencesAreChecked_ThenNoTestProjectIsReferenced()
        {
            string[] projectReferences = ReadProjectReferences(ArchitectureTestPaths.ProductionProjectFile);

            string[] testReferences = projectReferences
                .Where(reference => reference.Contains("Tests", StringComparison.Ordinal))
                .ToArray();

            Assert.That(
                testReferences,
                Is.Empty,
                "Production code must not reference a test project.");
        }

        [Test]
        public void GivenProductionProject_WhenPackageReferencesAreChecked_ThenOnlyApprovedPackagesAreDeclared()
        {
            string[] approvedPackages =
            [
                "MonoGame.Framework.DesktopGL",
                "NuciXNA.Primitives",
                "SixLabors.ImageSharp",
            ];

            string[] declaredPackages = ReadPackageReferences(ArchitectureTestPaths.ProductionProjectFile);

            Assert.That(
                declaredPackages,
                Is.EqualTo(approvedPackages));
        }

        [Test]
        public void GivenTestProjects_WhenProjectReferencesAreChecked_ThenEachReferencesProductionProject()
        {
            string[] testProjectFiles =
            [
                ArchitectureTestPaths.UnitTestsProjectFile,
                ArchitectureTestPaths.ArchitectureTestsProjectFile,
            ];

            string[] withoutProductionReference = testProjectFiles
                .Where(projectFile => !ReadProjectReferences(projectFile)
                    .Any(reference => reference.Contains("NuciXNA.DataAccess.csproj", StringComparison.Ordinal)))
                .Select(ArchitectureTestPaths.ToRelativePortablePath)
                .ToArray();

            Assert.That(
                withoutProductionReference,
                Is.Empty,
                "Every test project must reference the production project.");
        }

        [Test]
        public void GivenProductionTypes_WhenDependenciesAreChecked_ThenNoTypeDependsOnTheTestAssemblies()
        {
            IArchRule rule = ArchRuleDefinition.Types()
                .That().ResideInNamespaceMatching("NuciXNA\\.DataAccess\\.(Content|IO)")
                .Should().NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching("NuciXNA\\.DataAccess\\.(UnitTests|ArchitectureTests)")
                .Because("production code must never depend on test code.");

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenProductionTypes_WhenDependenciesAreChecked_ThenNoTypeDependsOnFrameworkTestingTypes()
        {
            IArchRule rule = ArchRuleDefinition.Types()
                .That().ResideInNamespaceMatching("NuciXNA\\.DataAccess\\.(Content|IO)")
                .Should().NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching("^(NUnit|Moq)")
                .Because("production code must not depend on test frameworks.");

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenProductionTypes_WhenNamespaceIsChecked_ThenNoTypeResidesInTheRootNamespace()
        {
            IArchRule rule = ArchRuleDefinition.Types()
                .That().HaveFullNameStartingWith("NuciXNA.DataAccess.")
                .Should().NotResideInNamespace("NuciXNA.DataAccess")
                .Because("every production type must belong to a subsystem namespace.");

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenContentContract_WhenImplementationIsChecked_ThenEveryLoaderImplementsIContentLoader()
        {
            IArchRule rule = ArchRuleDefinition.Classes()
                .That().HaveNameEndingWith("ContentLoader")
                .And().AreNotAbstract()
                .Should().ImplementInterface(typeof(IContentLoader))
                .Because("every concrete loader must honour the content contract.");

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenProductionAssembly_WhenPublicTypesAreChecked_ThenNoTypeIsNested()
        {
            IArchRule rule = ArchRuleDefinition.Classes()
                .That().ArePublic()
                .Should().NotBeNested()
                .Because("public production types must be top-level.");

            ArchRuleAssert.FulfilsRule(ProductionArchitecture, rule);
        }

        [Test]
        public void GivenProductionAssembly_WhenDependenciesAreChecked_ThenNoUnapprovedNamespaceIsUsed()
        {
            string[] approvedRoots =
            [
                "NuciXNA.DataAccess",
                "NuciXNA.Primitives",
                "Microsoft.Xna.Framework",
                "SixLabors.ImageSharp",
                "System",
            ];

            string[] violations = ProductionArchitecture.Types
                .Where(type => type.Namespace.FullName.StartsWith("NuciXNA.DataAccess", StringComparison.Ordinal))
                .SelectMany(type => type.Dependencies
                    .Select(dependency => dependency.Target)
                    .OfType<IType>()
                    .Select(target => target.Namespace.FullName)
                    .Where(namespaceName => !approvedRoots.Any(root =>
                        string.Equals(namespaceName, root, StringComparison.Ordinal)
                        || namespaceName.StartsWith($"{root}.", StringComparison.Ordinal)))
                    .Select(namespaceName => $"{type.FullName} depends on {namespaceName}"))
                .Distinct()
                .ToArray();

            Assert.That(violations, Is.Empty, "Production types may depend only on approved namespaces.");
        }

        private static int ContentTypeCount()
            => ProductionArchitecture.Types
                .Count(type => string.Equals(type.Namespace.FullName, ContentNamespace, StringComparison.Ordinal));

        private static int IONamespaceTypeCount()
            => ProductionArchitecture.Types
                .Count(type => string.Equals(type.Namespace.FullName, IONamespace, StringComparison.Ordinal));

        private static string[] ReadProjectReferences(string projectFile)
            => XDocument.Load(projectFile)
                .Descendants("ProjectReference")
                .Select(element => element.Attribute("Include")?.Value)
                .Where(include => include is not null)
                .Select(include => include!)
                .ToArray();

        private static string[] ReadPackageReferences(string projectFile)
            => XDocument.Load(projectFile)
                .Descendants("PackageReference")
                .Select(element => element.Attribute("Include")?.Value)
                .Where(include => include is not null)
                .Select(include => include!)
                .OrderBy(package => package, StringComparer.Ordinal)
                .ToArray();

        private static Assembly ProductionAssembly()
            => typeof(NuciContentManager).Assembly;
    }
}
