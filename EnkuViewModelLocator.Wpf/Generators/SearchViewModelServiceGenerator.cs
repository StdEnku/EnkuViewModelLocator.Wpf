namespace EnkuViewModelLocator.Wpf.Generators;

using Microsoft.CodeAnalysis;

/// <summary>
/// あるアセンブリ内からViewModel属性の付いたクラスとその生存期間を取得するための
/// 静的クラスを生成するためのジェネレータ
/// </summary>
[Generator]
public class SearchViewModelServiceGenerator : IIncrementalGenerator
{
    /// <summary>
    /// クライアント側のコードが修正されるたびに実行されるメソッド
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(CreateClasses);
    }

    private static void CreateClasses(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("SearchViewModelServiceGenerator.g.cs", @"
namespace EnkuViewModelLocator.Wpf
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;

    public class VmTypeWithServiceLifeTime
    {
        public Type ViewModelType { get; }

        public ViewModelAttribute.ServiceLifeTime LifeTime { get; }

        public VmTypeWithServiceLifeTime(Type viewModelType, ViewModelAttribute.ServiceLifeTime lifeTime)
        {
            this.ViewModelType = viewModelType;
            this.LifeTime = lifeTime;
        }
    }

    public static class SearchViewModelService
    {
        public static IEnumerable<VmTypeWithServiceLifeTime> FromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach(var i in types)
            {
                var attrib = i.GetCustomAttribute<ViewModelAttribute>();
                if (attrib is not null)
                    yield return new VmTypeWithServiceLifeTime(i, attrib.LifeTime);
            }
        }
    }
}");
    }
}