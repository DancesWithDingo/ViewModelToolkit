using ViewModelToolkit.ViewModels;

namespace ViewModelToolkitSample.ViewModels;

public class PickANumberPageViewModel : ModalViewModelBase<int>
{
    public override void Initialize(int item) {
        base.Initialize(item);
        NumberString = string.Empty;
    }

    public override int Update() {
        return int.TryParse(NumberString?.Trim(), out int result) ? result : 0;
    }

    public override bool Validate() {
        NumberStringErrorText = string.Empty;

        if ( string.IsNullOrWhiteSpace(NumberString) ) {
            NumberStringErrorText = "This field is required";
        } else {
            var result = Update();
            if ( result < 1 || result > 10 )
                NumberStringErrorText = $"{NumberString} is not an integer between 1 and 10.";
        }

        bool noErrors = string.IsNullOrEmpty(NumberStringErrorText);
        return base.Validate(noErrors);
    }

    public string NumberString { get => _NumberString; set => Set(ref _NumberString, value, shouldValidate: true); }
    string _NumberString;

    public string NumberStringErrorText { get => _NumberStringErrorText; set => Set(ref _NumberStringErrorText, value, setIsDirty: false); }
    string _NumberStringErrorText;
}
