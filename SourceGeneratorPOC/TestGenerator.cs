using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGeneratorPOC
{
    [Generator]
    public class TestGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var builder = new StringBuilder();
            builder.AppendLine(@"
namespace StartupExtensions
{
    public static class StartupRunner
    {
        public static void Run()
        {");

            var interfaceSymbol = context.Compilation.GetTypeByMetadataName(typeof(IStartup).FullName);
            if (context.SyntaxReceiver is StartupReceiver startupReceiver)
            {
                foreach (var classDeclaration in startupReceiver.Candidates)
                {
                    var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);

                    var symbolInfo = model.GetDeclaredSymbol(classDeclaration);
                    if (symbolInfo is ITypeSymbol typeSymbol && typeSymbol.AllInterfaces.Any(t => t.Equals(interfaceSymbol, SymbolEqualityComparer.Default)))
                    {
                        var variableName = classDeclaration.Identifier.Text.ToLowerInvariant();
                        builder.AppendLine($"var {variableName}= new {symbolInfo.ContainingNamespace.Name}.{symbolInfo.Name}();");
                        builder.AppendLine($"{variableName}.Execute();");
                    }
                }
            }
            builder.AppendLine(@"
        }
    }
}");
            context.AddSource("startupRunner.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new StartupReceiver());
        }
    }

    public class StartupReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Candidates { get; } = new List<ClassDeclarationSyntax>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax)
            {
                Candidates.Add(syntaxNode as ClassDeclarationSyntax);
            }
        }
    }
}
