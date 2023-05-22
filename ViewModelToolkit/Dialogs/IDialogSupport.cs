namespace ViewModelToolkit.Dialogs;

public interface IDialogSupport { }

public interface IDialogSupport<T> : IDialogSupport
{
    DialogManager<T> DialogManager { get; init; }
}
