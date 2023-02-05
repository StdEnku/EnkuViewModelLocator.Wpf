namespace SampleApp;

using Microsoft.Extensions.DependencyInjection;
using SampleApp.Services;
using System;
using System.Reflection;
using System.Windows;
using EnkuViewModelLocator.Wpf;

/// <summary>
/// App.xaml用のコードビハインド
/// </summary>
public partial class App : Application, IDiApplication
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public App()
    {
        this.Services = ConfigureServices();
        this.InitializeComponent();
    }

    /// <summary>
    /// DIコンテナのIServiceProvierを取得するためのプロパティ
    /// </summary>
    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();
        var vmWithLifeTime = SearchViewModelService.FromAssembly(assembly);

        // 以下DIコンテナへのViewModelやサービスやユースケース層クラスの追加
        services.AddTransient<INavigationService, NavigationService>();
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
