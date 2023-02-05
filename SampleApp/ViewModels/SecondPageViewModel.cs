namespace SampleApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SampleApp.Services;
using EnkuViewModelLocator.Wpf;

[ViewModel]
public partial class SecondPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public SecondPageViewModel(INavigationService navigationService)
    {
        this._navigationService = navigationService;
    }

    [ObservableProperty]
    private string _mainText = "SecondPage";

    [RelayCommand]
    private void Clicked()
    {
        this._navigationService.GoBack();
    }
}