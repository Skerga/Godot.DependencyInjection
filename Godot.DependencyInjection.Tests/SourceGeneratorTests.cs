using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Skerga.Godot.DependencyInjection;
using Xunit;

namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public class SourceGeneratorTests
{
    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            [CSharpSyntaxTree.ParseText(source)],
            [MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    
    [Fact]
    public void GenerateClassesBasedOnDDDRegistry()
    {
        // Create an instance of the source generator.
        var ctorGenerator = new CtorGenerator();
        var methodGenerator = new DiMethodGenerator();
        
        GeneratorDriver driver = CSharpGeneratorDriver.Create(ctorGenerator, methodGenerator);
        
        var compilation = CreateCompilation(@"
namespace Godot.DependencyInjection
{
    [global::System.AttributeUsage(global::System.AttributeTargets.Constructor | global::System.AttributeTargets.Parameter)]
    public class InjectAttribute : global::System.Attribute
    {
    }
}

namespace MyCode
{
    public partial class Program
    {
        private readonly String _messageBusNode;
        
        [Godot.DependencyInjection.Inject]
        private Main(String messageBusNode)
        {
            _messageBusNode = messageBusNode;
        }

        public void msg(int a, [Godot.DependencyInjection.Inject] String mb, [Godot.DependencyInjection.Inject] bool bb)
        {}
    }
}
");
        
        // Run generators. Don't forget to use the new compilation rather than the previous one.
        driver.RunGeneratorsAndUpdateCompilation(compilation, 
            out var newCompilation, 
            out var diagnostics, 
            TestContext.Current.CancellationToken);

        diagnostics.ToList().ForEach(d => Console.WriteLine(d.GetMessage()));
        
        // Retrieve all files in the compilation.
        var generatedFiles = newCompilation.SyntaxTrees
            .Select(t => Path.GetFileName(t.FilePath))
            .ToArray();
        
        Assert.Equivalent(new[]
        {
             "Program.g.cs",
             "Programmsgint a.g.cs"
        }, generatedFiles);
    }
}