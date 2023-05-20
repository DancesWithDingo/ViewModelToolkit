using System.Windows.Input;

namespace ViewModelToolkit.Views;

public interface ISaveBarView : IView, IElement
{
    ICommand CancelButtonCommand { get; set; }
    object CancelButtonCommandParameter { get; set; }
    string CancelButtonText { get; set; }

    ICommand SaveButtonCommand { get; set; }
    object SaveButtonCommandParameter { get; set; }
    string SaveButtonText { get; set; }

    bool IsCancelButtonVisible { get; set; }
    bool IsSaveButtonVisible { get; set; }

    Style ButtonStyle { get; set; }
    bool IsVisible { get; set; }
}


public interface ISaveBarButtonView : IView, IElement
{
    ICommand Command { get; set; }
    object CommandParameter { get; set; }
    bool IsVisible { get; set; }
    Style Style { get; set; }
    string Text { get; set; }
}