namespace ViewModelToolkitSample.Views;

public partial class PickANumberPage : ContentPage
{
    public PickANumberPage() {
        InitializeComponent();
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        Dispatcher.Dispatch(() => NumberEntry.Focus());
    }
}
