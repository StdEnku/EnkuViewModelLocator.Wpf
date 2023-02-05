# EnkuViewModelLocator.Wpf

This library was developed to solve the problem of using a DI container in a WPF application, as shown in the [example here](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/ioc), where a code-behind is required to attach a ViewModel to a View's DataContext. [IncrementalSourceGenerator](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md) to solve this problem.

[日本語版README.md](./README-jp.md)

# How to use

## 1. reserve

> This sample contains all code in the SampleApp folder in the repository.
>
> Also, the DI container uses Microsoft.Extensions.DependencyInjection.

First, please install

- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)
- [EnkuViewModelLocator.Wpf](https://www.nuget.org/packages/EnkuViewModelLocator.Wpf)

at Nuget.

Then, modify the code-behind of the App class as shown below, referring to the [example here](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/ioc).

```c#
// ellipsis usings
using Microsoft.Extensions.DependencyInjection;

public sealed partial class App : Application
{
    public App()
    {
        Services = ConfigureServices();
        this.InitializeComponent();
    }
    
    public new static App Current => (App)Application.Current;
    
    public IServiceProvider Services { get; }
    
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
    }
}
```

## 2. App class inherits IDiApplication interface

Make the App class modified above inherit the `EnkuViewModelLocator.Wpf.IDiApplication` interface and include in the ConfigureServices method a process to search for all ViewModels and register them in the DI container Please use the `ConfigureServices` method.

```c#
// ellipsis usings
using Microsoft.Extensions.DependencyInjection;
using EnkuViewModelLocator.Wpf; // ← Add

public sealed partial class App : Application, IDiApplication // ← Add
{
    public App()
    {
        Services = ConfigureServices();
        this.InitializeComponent();
    }
    
    public new static App Current => (App)Application.Current;
    
    public IServiceProvider Services { get; }
    
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // ↓Add
        var assembly = Assembly.GetExecutingAssembly();
        var vmWithLifeTime = SearchViewModelService.FromAssembly(assembly);
        foreach (var i in vmWithLifeTime)
        {
            if (i.LifeTime == ViewModelAttribute.ServiceLifeTime.Transient)
                services.AddTransient(i.ViewModelType);
            else
                services.AddSingleton(i.ViewModelType);
        }
    }
}
```

## 3. Attaching ViewModel attributes

After creating the ViewModel, attach the EnkuViewModelLocator.Wpf.ViewModelAttribute as follows.

```c#
using System;
using EnkuViewModelLocator.Wpf; //←Add

[ViewModel] //←Add
public class FirstPageViewModel : INotifyPropertyChanged
{
    // ellipsis
}
```

This will automatically register the ViewModel in the DI container and generate a ViewModels class in the same namespace to obtain an instance of that ViewModel.

Also, the lifespan of a ViewModel is set to Transient by default, but it can be set to Singleton by writing the following.

```c#
using System;
using EnkuViewModelLocator.Wpf;

[ViewModel(ViewModelAttribute.ServiceLifeTime.Singleton)]
public class FirstPageViewModel : INotifyPropertyChanged
{
    // ellipsis
}
```

## 4. Specify ViewModel for the View's DataContext

To attach the ViewModel created above to a View, specify the value of the property with the same name as the ViewModel in the ViewModels class generated in the same namespace as the ViewModel in the View's DataContext as shown below.

```xaml
<Page ~ellipsis~
      xmlns:vm="clr-namespace:To.ViewModel.Namespace"
      DataContext="{x:Static vm:ViewModels.FirstPageViewModel}">

    <!--ellipsis-->
</Page>
```

> [important point]
>
> In this library, it is not possible to register a ViewModel that uses the Generic syntax because the ViewModel class is registered in the DI container using attributes at the definition stage.

# Classes and interfaces appearing in this library

## IDiApplication Interface

namespace : EnkuViewModelLocator.Wpf<br/>remarks : An interface that has only the Services property of the System.IServiceProvider type without a setter, and is intended to be implemented in the App class of the WPF application template.

## ViewModel Attribute

namespace : EnkuViewModelLocator.Wpf<br/>remarks : Marker attribute to generate the following ViewModels class. ServiceLifeTime of the target ViewModel can be specified in the constructor. The default value of the constructor is ServiceLifeTime.Transient.

## ServiceLifeTime Enum

namespace : EnkuViewModelLocator.Wpf.ViewModelAttribute<br/>remarks : Enum value representing the ServiceLifeTime of the target ViewModel. Members are Transient and Singleton.

## SearchViewModelService Static Class

namespace : EnkuViewModelLocator.Wpf<br/>remarks : A static class with methods to find all classes with attached ViewModel attributes in the assembly specified by the argument.

Method Signatures : 

> public static System.Collections.Generic.IEnumerable<VmTypeWithServiceLifeTime> FromAssembly(System.Assembly assembly)

## VmTypeWithServiceLifeTime Class

namespace : EnkuViewModelLocator.Wpf<br/>remarks : DTO class that holds the ViewModel type and lifetime information used as the return value of the SearchViewModelService.FromAssembly method.

| Property Name  | Type                                                          | Remarks                           |
| ------------- | ----------------------------------------------------------- | ------------------------------ |
| ViewModelType | System.Type                                                 | ViewModel type information     |
| LifeTime      | EnkuViewModelLocator.Wpf.ViewModelAttribute.ServiceLifeTime | ViewModel Lifetime Information |

## ViewModels Static Class

namespace : Same namespace as the target ViewModel<br/>remarks : Static class for retrieving classes with ViewModel attributes. Members are successively added with the same static property as the ViewModel name that returns the object generated from the DI container.



# Dependency Libraries License

```txt
The MIT License (MIT)

Copyright (c) .NET Foundation and Contributors

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

