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
    /// Enforces statically decidable C# coding conventions across the handwritten production sources.
    /// </summary>
    [TestFixture]
    public sealed class ProductionCodeConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoRegionDirectiveIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantTrivia().Any(trivia =>
                    trivia.IsKind(SyntaxKind.RegionDirectiveTrivia)
                    || trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia)),
                "Regions are prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoGlobalUsingDirectiveIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<UsingDirectiveSyntax>()
                    .Any(usingDirective => usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword)),
                "Global using directives are prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoNullableDirectiveIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantTrivia().Any(trivia =>
                    trivia.ToString().TrimStart().StartsWith("#nullable", StringComparison.Ordinal)),
                "#nullable directives are prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenOnlyBlockNamespacesAreUsed()
            => AssertNoProductionViolations(
                root => !root.Members
                    .OfType<BaseNamespaceDeclarationSyntax>()
                    .Any(namespaceDeclaration => namespaceDeclaration is NamespaceDeclarationSyntax),
                "Block namespaces are required.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenVarIsNeverUsed()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<VariableDeclarationSyntax>()
                    .Any(declaration => declaration.Type.IsVar),
                "Explicit types are required; 'var' is prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoTernaryConditionalExpressionIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<ConditionalExpressionSyntax>().Any(),
                "Ternary expressions are prohibited; use if/else.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoConstDeclarationIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                    .Any(field => field.Modifiers.Any(SyntaxKind.ConstKeyword)),
                "Constants must be static expression-bodied properties.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoInitAccessorIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<AccessorDeclarationSyntax>()
                    .Any(accessor => accessor.IsKind(SyntaxKind.InitAccessorDeclaration)),
                "init accessors are prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoRefOrOutParameterIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<ParameterSyntax>()
                    .Any(parameter =>
                        parameter.Modifiers.Any(SyntaxKind.RefKeyword)
                        || parameter.Modifiers.Any(SyntaxKind.OutKeyword)),
                "ref and out parameters are prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoOptionalParameterIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<ParameterSyntax>()
                    .Any(parameter => parameter.Default is not null),
                "Optional parameters are prohibited; use overloads.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoTupleTypeOrExpressionIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().Any(node =>
                    node is TupleTypeSyntax || node is TupleExpressionSyntax),
                "Tuples are prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoNullableEqualityComparisonIsPresent()
            => AssertNoProductionViolations(
                ContainsNullEqualityComparison,
                "Use 'is null' / 'is not null' instead of '== null' / '!= null'.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoNullCoalescingOperatorIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<BinaryExpressionSyntax>()
                    .Any(binary => binary.IsKind(SyntaxKind.CoalesceExpression)),
                "The '??' operator is prohibited; '??=' remains permitted.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoEmptyCatchBlockIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<CatchClauseSyntax>()
                    .Any(catchClause => catchClause.Block.Statements.Count == 0),
                "Empty catch blocks are prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoCatchRethrowsTheCaughtException()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<CatchClauseSyntax>().Any(RethrowsCaughtException),
                "Use a bare 'throw;' to preserve the stack trace.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoPartialTypeIsPresent()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                    .Any(declaration => declaration.Modifiers.Any(SyntaxKind.PartialKeyword)),
                "Partial types are prohibited.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenFieldsDoNotUseUnderscorePrefix()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                    .SelectMany(field => field.Declaration.Variables)
                    .Any(variable => variable.Identifier.ValueText.StartsWith('_')),
                "Private fields must use camelCase without an underscore prefix.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenEveryMemberDeclaresAccessibility()
            => AssertNoProductionViolations(
                HasMemberWithoutAccessibility,
                "Every member must declare explicit accessibility.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenClassesAreSealedOrDesignedForInheritance()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Any(classDeclaration => !IsSealedAbstractOrStatic(classDeclaration)),
                "Classes must be sealed unless designed for inheritance.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenPublicTypesAndMembersHaveXmlDocumentation()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<MemberDeclarationSyntax>()
                    .Where(member => member is BaseTypeDeclarationSyntax declaration
                        && declaration.Modifiers.Any(SyntaxKind.PublicKeyword)
                        || IsPublicApiMember(member))
                    .Any(member => !HasXmlDocumentation(member)),
                "Public types and API members require XML documentation.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenBuiltInAliasesAreUsedForSimpleTypes()
        {
            List<string> violations = [];

            foreach (string file in ArchitectureTestPaths.ProductionSourceFiles)
            {
                SourceText text = SourceFileIndex.Text(file);

                for (int line = 1; line <= text.Lines.Count; line += 1)
                {
                    string content = StripLineComment(text[line]);
                    string[] aliases = BuiltInAliasNames()
                        .Where(alias => ContainsWord(content, alias))
                        .ToArray();

                    if (aliases.Length > 0)
                    {
                        violations.Add($"{SourceFileIndex.Display(file)}:{line} uses {string.Join(", ", aliases)}");
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Use built-in type aliases instead of BCL type names.");
        }

        [Test]
        public void GivenProductionSources_WhenInspected_ThenControlFlowBodiesAreBraced()
            => AssertNoProductionViolations(
                HasUnbracedControlFlow,
                "Every control-flow body must use braces.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenUsingDirectivesPrecedeTheNamespace()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                    .Any(namespaceDeclaration => namespaceDeclaration.Usings.Count > 0),
                "Using directives must be outside the namespace.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenSingleStatementMethodsAreExpressionBodied()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Any(method => HasConvertibleSingleStatementBody(method)),
                "Single-statement methods must be expression-bodied.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoCustomExceptionTypeExists()
            => AssertNoProductionViolations(
                root => root.DescendantNodes().OfType<ClassDeclarationSyntax>().Any(InheritsFromException),
                "Custom exception types are prohibited; use BCL exceptions.");

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoBlankLinePrecedesAClosingDelimiter()
        {
            List<string> violations = [];

            foreach (string file in ArchitectureTestPaths.ProductionSourceFiles)
            {
                SourceText text = SourceFileIndex.Text(file);

                for (int line = 2; line <= text.Lines.Count; line += 1)
                {
                    string current = text[line].Trim();

                    if ((current is "}" or "]" or ")") && string.IsNullOrWhiteSpace(text[line - 1]))
                    {
                        violations.Add($"{SourceFileIndex.Display(file)}:{line}");
                    }
                }
            }

            Assert.That(violations, Is.Empty, "A blank line must not precede a closing delimiter.");
        }

        [Test]
        public void GivenProductionSources_WhenInspected_ThenNoTrailingWhitespaceIsPresent()
        {
            List<string> violations = [];

            foreach (string file in ArchitectureTestPaths.ProductionSourceFiles)
            {
                SourceText text = SourceFileIndex.Text(file);

                for (int line = 1; line <= text.Lines.Count; line += 1)
                {
                    string content = text[line];

                    if (content.Length > 0 && content.TrimEnd().Length != content.Length)
                    {
                        violations.Add($"{SourceFileIndex.Display(file)}:{line}");
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Trailing whitespace is prohibited.");
        }

        private static void AssertNoProductionViolations(
            Func<CompilationUnitSyntax, bool> predicate,
            string reason)
        {
            string[] violations = ArchitectureTestPaths.ProductionSourceFiles
                .Where(file => predicate(SourceFileIndex.Root(file)))
                .Select(SourceFileIndex.Display)
                .ToArray();

            Assert.That(violations, Is.Empty, reason);
        }

        private static bool IsSealedAbstractOrStatic(ClassDeclarationSyntax classDeclaration)
            => classDeclaration.Modifiers.Any(SyntaxKind.SealedKeyword)
            || classDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword)
            || classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);

        private static bool IsPublicApiMember(MemberDeclarationSyntax member)
            => member switch
            {
                MethodDeclarationSyntax method => method.Modifiers.Any(SyntaxKind.PublicKeyword),
                PropertyDeclarationSyntax property => property.Modifiers.Any(SyntaxKind.PublicKeyword),
                ConstructorDeclarationSyntax constructor => constructor.Modifiers.Any(SyntaxKind.PublicKeyword),
                IndexerDeclarationSyntax indexer => indexer.Modifiers.Any(SyntaxKind.PublicKeyword),
                EventFieldDeclarationSyntax eventField => eventField.Modifiers.Any(SyntaxKind.PublicKeyword),
                _ => false,
            };

        private static bool HasConvertibleSingleStatementBody(MethodDeclarationSyntax method)
        {
            if (method.Body is null || method.Body.Statements.Count != 1)
            {
                return false;
            }

            StatementSyntax statement = method.Body.Statements[0];

            if (statement is ReturnStatementSyntax returnStatement)
            {
                return returnStatement.Expression is not null;
            }

            if (statement is ExpressionStatementSyntax expressionStatement)
            {
                return expressionStatement.Expression is AssignmentExpressionSyntax or InvocationExpressionSyntax;
            }

            return false;
        }

        private static bool HasXmlDocumentation(MemberDeclarationSyntax member)
            => member.GetLeadingTrivia()
                .Any(trivia => trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));

        private static bool ContainsNullEqualityComparison(CompilationUnitSyntax root)
            => root.DescendantNodes().OfType<BinaryExpressionSyntax>()
                .Where(binary => binary.IsKind(SyntaxKind.EqualsExpression) || binary.IsKind(SyntaxKind.NotEqualsExpression))
                .Any(binary => binary.Right.IsKind(SyntaxKind.NullLiteralExpression)
                    || binary.Left.IsKind(SyntaxKind.NullLiteralExpression));

        private static bool RethrowsCaughtException(CatchClauseSyntax catchClause)
        {
            string? identifier = catchClause.Declaration?.Identifier.ValueText;

            if (identifier is null)
            {
                return false;
            }

            return catchClause.Block.DescendantNodes().OfType<ThrowStatementSyntax>()
                .Any(throwStatement => throwStatement.Expression is IdentifierNameSyntax thrown
                    && string.Equals(thrown.Identifier.ValueText, identifier, StringComparison.Ordinal));
        }

        private static bool HasMemberWithoutAccessibility(CompilationUnitSyntax root)
            => root.DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(member => member is FieldDeclarationSyntax or PropertyDeclarationSyntax
                    or MethodDeclarationSyntax or ConstructorDeclarationSyntax or EventFieldDeclarationSyntax)
                .Where(member => member.Parent is not InterfaceDeclarationSyntax)
                .Any(member => !member.Modifiers.Any(modifier =>
                    modifier.IsKind(SyntaxKind.PublicKeyword)
                    || modifier.IsKind(SyntaxKind.PrivateKeyword)
                    || modifier.IsKind(SyntaxKind.ProtectedKeyword)
                    || modifier.IsKind(SyntaxKind.InternalKeyword)));

        private static bool HasUnbracedControlFlow(CompilationUnitSyntax root)
            => root.DescendantNodes().OfType<StatementSyntax>()
                .Any(statement => statement switch
                {
                    IfStatementSyntax ifStatement => ifStatement.Statement is not BlockSyntax,
                    ForStatementSyntax forStatement => forStatement.Statement is not BlockSyntax,
                    ForEachStatementSyntax forEachStatement => forEachStatement.Statement is not BlockSyntax,
                    WhileStatementSyntax whileStatement => whileStatement.Statement is not BlockSyntax,
                    DoStatementSyntax doStatement => doStatement.Statement is not BlockSyntax,
                    _ => false,
                });

        private static bool InheritsFromException(ClassDeclarationSyntax classDeclaration)
            => classDeclaration.BaseList is not null
            && classDeclaration.BaseList.Types.Any(baseType => baseType.Type is IdentifierNameSyntax identifier
                && identifier.Identifier.ValueText.EndsWith("Exception", StringComparison.Ordinal));

        private static string StripLineComment(string line)
        {
            int index = line.IndexOf("//", StringComparison.Ordinal);

            return index < 0 ? line : line[..index];
        }

        private static IReadOnlyList<string> BuiltInAliasNames() =>
        [
            "String", "Int32", "Int64", "Int16", "Boolean", "Object",
            "Double", "Single", "Decimal", "Byte", "Char",
        ];

        private static bool ContainsWord(string content, string word)
        {
            int index = content.IndexOf(word, StringComparison.Ordinal);

            while (index >= 0)
            {
                bool startBoundary = index == 0 || !char.IsLetterOrDigit(content[index - 1]);
                int end = index + word.Length;
                bool endBoundary = end >= content.Length || !char.IsLetterOrDigit(content[end]);

                if (startBoundary && endBoundary)
                {
                    return true;
                }

                index = content.IndexOf(word, index + 1, StringComparison.Ordinal);
            }

            return false;
        }
    }
}