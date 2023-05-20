using ViewModelToolkit.ViewModels;
using ViewModelToolkitSample.Models;

namespace ViewModelToolkitSample.ViewModels.Base;

public abstract class PersonViewModelBase : ViewModelBase<Person>
{
    public override void Initialize(Person item) {
        base.Initialize(item);
        FirstName = item.FirstName;
        LastName = item.LastName;
    }

    public override Person Update() {
        return new Person {
            FirstName = FirstName?.Trim() ?? string.Empty,
            LastName = LastName?.Trim() ?? string.Empty,
            BirthDate = Source.BirthDate,
        };
    }

    public override bool Validate() {
        FirstNameErrorText = string.Empty;
        LastNameErrorText = string.Empty;

        var result = Update();

        if ( string.IsNullOrWhiteSpace(result.FirstName) ) FirstNameErrorText = "First name is required.";
        if ( string.IsNullOrWhiteSpace(result.LastName) ) LastNameErrorText = "Last name is required.";

        bool hasError = (FirstNameErrorText + LastNameErrorText).Any();

        return base.Validate(!hasError);
    }

    public string FirstName { get => _FirstName; set => Set(ref _FirstName, value, SetFullName, shouldValidate: true); }
    string _FirstName;

    public string FirstNameErrorText { get => _FirstNameErrorText; set => Set(ref _FirstNameErrorText, value, setIsDirty: false); }
    string _FirstNameErrorText;

    public string FullName { get => _FullName; set => Set(ref _FullName, value); }
    string _FullName;

    public string LastName { get => _LastName; set => Set(ref _LastName, value, SetFullName, shouldValidate: true); }
    string _LastName;

    public string LastNameErrorText { get => _LastNameErrorText; set => Set(ref _LastNameErrorText, value, setIsDirty: false); }
    string _LastNameErrorText;

    void SetFullName(string _) => FullName = $"{Update().FullName}";
}
