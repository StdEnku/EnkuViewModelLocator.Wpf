namespace SampleApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SampleApp.Services;
using System;
using EnkuViewModelLocator.Wpf;

[ViewModel]
public partial class FirstPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public FirstPageViewModel(INavigationService navigationService)
    {
        this._navigationService = navigationService;
    }

    [ObservableProperty]
    private string _mainText = "FirstPage";

    [RelayCommand]
    private void Clicked()
    {
        var uri = new Uri("pack://application:,,,/Views/SecondPage.xaml", UriKind.Absolute);
        this._navigationService.Navigate(uri);
    }
}