namespace EnkuViewModelLocator.Wpf.Generators;

using Microsoft.CodeAnalysis;

/// <summary>
/// WPFプロジェクト内のAppクラスにて実装するIDiApplicationインターフェースを生成するためのジェネレータ
/// </summary>
[Generator]
public class IDiApplicationGenerator : IIncrementalGenerator
{
    /// <summary>
    /// クライアント側のコードが修正されるたびに実行されるメソッド
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(CreateInterface);
    }

    private static void CreateInterface(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("IDiApplication.g.cs", @"
namespace EnkuViewModelLocator.Wpf
{
    using System;

    public interface IDiApplication
    {
        IServiceProvider Services { get; }
    }
}");
    }
}