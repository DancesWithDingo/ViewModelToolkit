using ViewModelToolkit.ViewModels;

namespace ViewModelToolkitSample.ViewModels
{
    public class SimpleNavigationPageViewModel : ViewModelBase<string>
    {
        public override void Initialize(string item) {
            base.Initialize(item);
            PassedText = item;
        }

        public string PassedText { get => _PassedText; set => Set(ref _PassedText, value); }
        string _PassedText;
    }
}
