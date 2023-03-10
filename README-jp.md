# EnkuViewModelLocator.Wpf

本ライブラリは、WPFアプリケーションで[ここの例](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/ioc)のようにDIコンテナを使用する場合。<br/>ViewのDataContextにViewModelを紐付けるにはコードビハインドへの記述が<br/>必要になってしまう問題を解決するために[IncrementalSourceGenerator](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)を使用して開発されたライブラリです。

# 使用方法

## 1. 準備

> 本サンプルはリポジトリ内のSampleAppフォルダ内に全コードが含まれています。
>
> また、DIコンテナにはMicrosoft.Extensions.DependencyInjectionを使用しています。

まずNugetにて

- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)
- [EnkuViewModelLocator.Wpf](https://www.nuget.org/packages/EnkuViewModelLocator.Wpf)

をインストールして下さい。

その後、[ここの例](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/ioc)を参考にAppクラスのコードビハインドを下記のように修正して下さい。

```c#
// その他usingは省略
using Microsoft.Extensions.DependencyInjection;

public sealed partial class App : Application
{
    public App()
    {
        this.Services = ConfigureServices();
        this.InitializeComponent();
    }
    
    public new static App Current => (App)Application.Current;
    
    public IServiceProvider Services { get; }
    
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        // ViewModelやServiceの登録
        return services.BuildServiceProvider();
    }
}
```

## 2. AppクラスにIDiApplicationインターフェースを継承

上記で修正したAppクラスに<br/>`EnkuViewModelLocator.Wpf.IDiApplication`インターフェースを継承させて、<br/>ConfigureServicesメソッドにすべてのViewModelを検索して<br/>DIコンテナに登録する処理を記載してください。<br/>下記コード内にはすでにIDiApplicationのメンバであるServicesプロパティが実装されているので<br/>追記するメンバはありません。

```c#
// その他usingは省略
using Microsoft.Extensions.DependencyInjection;
using EnkuViewModelLocator.Wpf; // ← 追記

public sealed partial class App : Application, IDiApplication // ← 追記
{
    public App()
    {
        this.Services = ConfigureServices();
        this.InitializeComponent();
    }
    
    public new static App Current => (App)Application.Current;
    
    public IServiceProvider Services { get; }
    
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // ↓追記
        var assembly = Assembly.GetExecutingAssembly();
        var vmWithLifeTime = SearchViewModelService.FromAssembly(assembly);
        foreach (var i in vmWithLifeTime)
        {
            if (i.LifeTime == ViewModelAttribute.ServiceLifeTime.Transient)
                services.AddTransient(i.ViewModelType);
            else
                services.AddSingleton(i.ViewModelType);
        }
        
        return services.BuildServiceProvider();
    }
}
```

## 3. ViewModelの作成とViewModel属性の添付

ViewModelを作成後下記のようにEnkuViewModelLocator.Wpf.ViewModelAttributeを添付してください。

```c#
using System;
using EnkuViewModelLocator.Wpf; //←追記

[ViewModel] //←追記
public class FirstPageViewModel : INotifyPropertyChanged
{
    // 省略
}
```

こうすることで自動的にそのViewModelがDIコンテナに登録されて<br/>そのViewModelのインスタンスを取得するためのViewModelsクラスが同じ名前空間に生成されます。

また、ViewModelの寿命は規定値ではTransientとなっていますが<br/>下記のように記述することでSingletonにすることも可能です。

```c#
using System;
using EnkuViewModelLocator.Wpf;

[ViewModel(ViewModelAttribute.ServiceLifeTime.Singleton)]
public class FirstPageViewModel : INotifyPropertyChanged
{
    // 省略
}
```

## 4. ViewのDataContextにViewModelを指定

Viewに上記で作成したViewModelを紐付けるには<br/>下記のようにViewModelと同じ名前空間に生成されたViewModelsクラス内にある<br/>ViewModelと同名のプロパティの値をViewのDataContextに指定すれば可能です。

```xaml
<Page ~省略~
      xmlns:vm="clr-namespace:To.ViewModel.Namespace"
      DataContext="{x:Static vm:ViewModels.FirstPageViewModel}">

    <!--省略-->
</Page>
```

> [注意点]
>
> 本ライブラリではViewModelクラスを定義段階で属性を使用して登録する都合上
>
> Generic構文を使用したViewModelを登録することはできません。

# 本ライブラリで登場するクラスやインターフェース

## IDiApplication インターフェース

名前空間 : EnkuViewModelLocator.Wpf<br/>備考 : セッターのないSystem.IServiceProvider型のServicesプロパティのみを持ち、<br/>WPFアプリケーションのひな型のAppクラスにて実装することを想定したインターフェース。

## ViewModel 属性

名前空間 : EnkuViewModelLocator.Wpf<br/>備考 : 下記ViewModelsクラスを生成するためのマーカー属性。<br/>コンストラクタにてターゲットのViewModelのServiceLifeTimeを指定できます。<br/>コンストラクタの既定値はServiceLifeTime.Transientです。

## ServiceLifeTime Enum

名前空間 : EnkuViewModelLocator.Wpf.ViewModelAttribute<br/>備考 : ターゲットのViewModelのServiceLifeTimeを表すEnum値です。<br/>メンバはTransientとSingletonです。

## SearchViewModelService 静的クラス

名前空間 : EnkuViewModelLocator.Wpf<br/>備考 : 引数で指定したアセンブリ内からViewModel属性の添付された<br/>すべてのクラスを探すためのメソッドを持つ静的クラス。

メソッドのシグネチャ : 

> public static System.Collections.Generic.IEnumerable<VmTypeWithServiceLifeTime> FromAssembly(System.Assembly assembly)

## VmTypeWithServiceLifeTime クラス

名前空間 : EnkuViewModelLocator.Wpf<br/>備考 : SearchViewModelService.FromAssemblyメソッドの戻り値として使用される<br/>ViewModelの型情報と寿命情報を保持するDTOクラス。

| プロパティ名  | 型                                                          | 備考                |
| ------------- | ----------------------------------------------------------- | ------------------- |
| ViewModelType | System.Type                                                 | ViewModelの型情報   |
| LifeTime      | EnkuViewModelLocator.Wpf.ViewModelAttribute.ServiceLifeTime | ViewModelの寿命情報 |

## ViewModels 静的クラス

名前空間 : ターゲットのViewModelと同じ名前空間<br/>備考 : ViewModel属性の付いたクラスを取得するための静的クラス。<br/>メンバはDIコンテナから生成したオブジェクトを返す<br/>ViewModel名と同じstaticプロパティが逐次的に追加されていく。

