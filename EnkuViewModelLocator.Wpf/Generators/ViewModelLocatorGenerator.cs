namespace EnkuViewModelLocator.Wpf.Generators;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// ViewModelをXamlから参照するためのViewModels静的クラスを生成するためのジェネレータ
/// </summary>
[Generator]
public class ViewModelLocatorGenerator : IIncrementalGenerator
{
    private class ViewModelInfo
    {
        public string ClassName { get; }

        public string NameSpace { get; }

        public ViewModelInfo(string className, string nameSpace)
        {
            ClassName = className;
            NameSpace = nameSpace;
        }
    }

    /// <summary>
    /// クライアント側のコードが修正されるたびに実行されるメソッド
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(CreateAttribute);

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            "EnkuViewModelLocator.Wpf.ViewModelAttribute", Predicate, Transform
        ).Where(static item => item is not null);

        context.RegisterSourceOutput(provider, Emit);
    }

    private static void CreateAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("ViewModelAttribute.g.cs", @"
namespace EnkuViewModelLocator.Wpf
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelAttribute : Attribute
    {
        public enum ServiceLifeTime
        {
            Transient,
            Singleton,
        }

        public ServiceLifeTime LifeTime { get; }

        public ViewModelAttribute(ServiceLifeTime lifeTime = ServiceLifeTime.Transient)
        {
            this.LifeTime = lifeTime;
        }
    }
}");
    }

    private static bool Predicate(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax cs && cs.AttributeLists.Count > 0;
    }

    private static ViewModelInfo Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        var symbol = context.TargetSymbol;
        var className = symbol.Name;
        var nameSpace = symbol.ToDisplayString().Replace("." + className, string.Empty);
        var viewModelInfo = new ViewModelInfo(className, nameSpace);

        return viewModelInfo;
    }

    private static void Emit(SourceProductionContext cxt, ViewModelInfo viewModelInfo)
    {
        cxt.AddSource($"{viewModelInfo.ClassName}_vml.g.cs", $$"""
namespace {{viewModelInfo.NameSpace}}
{
    using EnkuViewModelLocator.Wpf;

    public static partial class ViewModels
    {
        public static {{viewModelInfo.NameSpace}}.{{viewModelInfo.ClassName}} {{viewModelInfo.ClassName}}
        {
            get 
            {
                var app = (IDiApplication)System.Windows.Application.Current;
                var currentServices = app.Services;
                var vmObject = currentServices.GetService(typeof({{viewModelInfo.NameSpace}}.{{viewModelInfo.ClassName}})) as {{viewModelInfo.NameSpace}}.{{viewModelInfo.ClassName}};
                System.Diagnostics.Debug.Assert(vmObject is not null);
                return vmObject;
            }
        }
    }
}
""");
    }
}