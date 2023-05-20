using System.Windows.Input;
using ViewModelToolkit.Views;

namespace ViewModelToolkitSample.Views;

public class CustomSaveBarButton : Button, ISaveBarButtonView
{
    protected override void OnSizeAllocated(double width, double height) {
        base.OnSizeAllocated(width, height);
        CornerRadius = (int)(height / 2);
    }
}


public class CustomSaveBarView : ContentView, ISaveBarView
{
    readonly Grid grid;

    public CustomSaveBarView() {
        Content = (grid = new Grid {
            RowSpacing = 6,
            RowDefinitions = new() {
                new RowDefinition(),
                new RowDefinition(),
                new RowDefinition(),
            },
            Children = {
                (CancelButton = new CustomSaveBarButton() {
                    Text = CancelButtonText,
                    Command = CancelButtonCommand,
                    CommandParameter = CancelButtonCommandParameter,
                }),
                (SaveButton = new CustomSaveBarButton() {
                    Text = SaveButtonText,
                    Command = SaveButtonCommand,
                    CommandParameter = SaveButtonCommandParameter,
                }),
                (HelpButton = new CustomSaveBarButton() { Text = "Help" })
            }
        });

        grid.SetRow(SaveButton, 0);
        grid.SetRow(CancelButton, 1);
        grid.SetRow(HelpButton, 2);

        SaveButtonText = "Submit";
    }

    public ISaveBarButtonView CancelButton { get; private set; }
    public ISaveBarButtonView SaveButton { get; private set; }
    public ISaveBarButtonView HelpButton { get; private set; }

    public Style ButtonStyle { get; set; }
    public bool IsCancelButtonVisible { get; set; }
    public bool IsSaveButtonVisible { get; set; }

    public ICommand CancelButtonCommand { get => (ICommand)GetValue(CancelButtonCommandProperty); set => SetValue(CancelButtonCommandProperty, value); }
    public static readonly BindableProperty CancelButtonCommandProperty =
        BindableProperty.Create(nameof(CancelButtonCommand), typeof(ICommand), typeof(CustomSaveBarView), propertyChanged: OnCancelButtonCommandPropertyChanged);
    static void OnCancelButtonCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as CustomSaveBarView;
        o.CancelButton.Command = (ICommand)newValue;
    }

    public object CancelButtonCommandParameter { get => (object)GetValue(CancelButtonCommandParameterProperty); set => SetValue(CancelButtonCommandParameterProperty, value); }
    public static readonly BindableProperty CancelButtonCommandParameterProperty =
        BindableProperty.Create(nameof(CancelButtonCommandParameter), typeof(object), typeof(CustomSaveBarView), propertyChanged: OnCancelButtonCommandParameterPropertyChanged);
    static void OnCancelButtonCommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as CustomSaveBarView;
        o.CancelButton.CommandParameter = newValue;
    }

    public string CancelButtonText { get => (string)GetValue(CancelButtonTextProperty); set => SetValue(CancelButtonTextProperty, value); }
    public static readonly BindableProperty CancelButtonTextProperty =
        BindableProperty.Create(nameof(CancelButtonText), typeof(string), typeof(CustomSaveBarView), "Cancel", propertyChanged: OnCancelButtonChanged);
    static void OnCancelButtonChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as CustomSaveBarView;
        o.CancelButton.Text = (string)newValue;
    }

    public ICommand SaveButtonCommand { get => (ICommand)GetValue(SaveButtonCommandProperty); set => SetValue(SaveButtonCommandProperty, value); }
    public static readonly BindableProperty SaveButtonCommandProperty =
        BindableProperty.Create(nameof(SaveButtonCommand), typeof(ICommand), typeof(CustomSaveBarView), propertyChanged: OnSaveButtonCommandPropertyChanged);
    static void OnSaveButtonCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as CustomSaveBarView;
        o.SaveButton.Command = (ICommand)newValue;
    }

    public object SaveButtonCommandParameter { get => (object)GetValue(SaveButtonCommandParameterProperty); set => SetValue(SaveButtonCommandParameterProperty, value); }
    public static readonly BindableProperty SaveButtonCommandParameterProperty =
        BindableProperty.Create(nameof(SaveButtonCommandParameter), typeof(object), typeof(CustomSaveBarView), propertyChanged: OnSaveButtonCommandParameterPropertyChanged);
    static void OnSaveButtonCommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as CustomSaveBarView;
        o.SaveButton.CommandParameter = newValue;
    }

    public string SaveButtonText { get => (string)GetValue(SaveButtonTextProperty); set => SetValue(SaveButtonTextProperty, value); }
    public static readonly BindableProperty SaveButtonTextProperty =
        BindableProperty.Create(nameof(SaveButtonText), typeof(string), typeof(CustomSaveBarView), "Save", propertyChanged: OnSaveButtonChanged);
    static void OnSaveButtonChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as CustomSaveBarView;
        o.SaveButton.Text = (string)newValue;
    }
}
