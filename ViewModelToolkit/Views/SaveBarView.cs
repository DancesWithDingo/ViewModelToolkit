using System.Windows.Input;

namespace ViewModelToolkit.Views;

public class SaveBarButton : Button, ISaveBarButtonView { }

public class SaveBarView : ContentView, ISaveBarView
{
    readonly bool isMacOS = OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();
    readonly bool isWindows = OperatingSystem.IsWindows();

    readonly Grid buttonBar;

    public SaveBarView() {
        HorizontalOptions = LayoutOptions.End;

        Content = new Grid {
            HeightRequest = isMacOS ? 36 : isWindows ? 36 : 60,
            Children = {
                (buttonBar = new Grid {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    ColumnSpacing = 6,
                    Margin = OperatingSystem.IsWindows() ? 2 : 0,
                    ColumnDefinitions = {
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                    },
                })
            }
        };

        CancelButton = new SaveBarButton {
            Text = CancelButtonText,
            Command = CancelButtonCommand,
            CommandParameter = CancelButtonCommandParameter,
            IsVisible = IsCancelButtonVisible,
            Style = ButtonStyle,
        };

        SaveButton = new SaveBarButton {
            Text = SaveButtonText,
            Command = SaveButtonCommand,
            CommandParameter = SaveButtonCommandParameter,
            IsVisible = IsSaveButtonVisible,
            Style = ButtonStyle,
        };

        buttonBar.Add(CancelButton);
        buttonBar.Insert(OperatingSystem.IsWindows() ? 0 : 1, SaveButton);

        buttonBar.SetColumn(SaveButton, OperatingSystem.IsWindows() ? 0 : 1);
        buttonBar.SetColumn(CancelButton, OperatingSystem.IsWindows() ? 1 : 0);

        PropertyChanged += (s, e) => {
            if ( e.PropertyName == Grid.HorizontalOptionsProperty.PropertyName )
                buttonBar.HorizontalOptions = HorizontalOptions;
        };
    }

    ISaveBarButtonView CancelButton { get; init; }
    ISaveBarButtonView SaveButton { get; init; }

    #region Bindable Properties

    public Style ButtonStyle { get => (Style)GetValue(ButtonStyleProperty); set => SetValue(ButtonStyleProperty, value); }
    public static readonly BindableProperty ButtonStyleProperty =
        BindableProperty.Create(nameof(ButtonStyle), typeof(Style), typeof(SaveBarView), propertyChanged: OnButtonStylePropertyChanged);
    static void OnButtonStylePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        if ( newValue is Style style ) {
            if ( o.CancelButton is not null ) o.CancelButton.Style = style;
            if ( o.SaveButton is not null ) o.SaveButton.Style = style;
        }
    }

    public bool IsCancelButtonVisible { get => (bool)GetValue(IsCancelButtonVisibleProperty); set => SetValue(IsCancelButtonVisibleProperty, value); }
    public static readonly BindableProperty IsCancelButtonVisibleProperty =
        BindableProperty.Create(nameof(IsCancelButtonVisible), typeof(bool), typeof(SaveBarView), true, propertyChanged: OnIsCancelButtonVisiblePropertyChanged);
    static void OnIsCancelButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        if ( o.CancelButton is not null )
            o.CancelButton.IsVisible = (bool)newValue;
    }

    public bool IsSaveButtonVisible { get => (bool)GetValue(IsSaveButtonVisibleProperty); set => SetValue(IsSaveButtonVisibleProperty, value); }
    public static readonly BindableProperty IsSaveButtonVisibleProperty =
        BindableProperty.Create(nameof(IsSaveButtonVisible), typeof(bool), typeof(SaveBarView), true, propertyChanged: OnIsSaveButtonVisiblePropertyChanged);
    static void OnIsSaveButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        if ( o.SaveButton is not null )
            o.SaveButton.IsVisible = (bool)newValue;
    }


    public ICommand CancelButtonCommand { get => (ICommand)GetValue(CancelButtonCommandProperty); set => SetValue(CancelButtonCommandProperty, value); }
    public static readonly BindableProperty CancelButtonCommandProperty =
        BindableProperty.Create(nameof(CancelButtonCommand), typeof(ICommand), typeof(SaveBarView), propertyChanged: OnCancelButtonCommandPropertyChanged);
    static void OnCancelButtonCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        o.CancelButton.Command = (ICommand)newValue;
    }

    public object CancelButtonCommandParameter { get => (object)GetValue(CancelButtonCommandParameterProperty); set => SetValue(CancelButtonCommandParameterProperty, value); }
    public static readonly BindableProperty CancelButtonCommandParameterProperty =
        BindableProperty.Create(nameof(CancelButtonCommandParameter), typeof(object), typeof(SaveBarView), propertyChanged: OnCancelButtonCommandParameterPropertyChanged);
    static void OnCancelButtonCommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        o.CancelButton.CommandParameter = newValue;
    }

    public string CancelButtonText { get => (string)GetValue(CancelButtonTextProperty); set => SetValue(CancelButtonTextProperty, value); }
    public static readonly BindableProperty CancelButtonTextProperty =
        BindableProperty.Create(nameof(CancelButtonText), typeof(string), typeof(SaveBarView), "Cancel", propertyChanged: OnCancelButtonTextChanged);
    static void OnCancelButtonTextChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        o.CancelButton.Text = (string)newValue;
    }

    public ICommand SaveButtonCommand { get => (ICommand)GetValue(SaveButtonCommandProperty); set => SetValue(SaveButtonCommandProperty, value); }
    public static readonly BindableProperty SaveButtonCommandProperty =
        BindableProperty.Create(nameof(SaveButtonCommand), typeof(ICommand), typeof(SaveBarView), propertyChanged: OnSaveButtonCommandPropertyChanged);
    static void OnSaveButtonCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        o.SaveButton.Command = (ICommand)newValue;
    }

    public object SaveButtonCommandParameter { get => (object)GetValue(SaveButtonCommandParameterProperty); set => SetValue(SaveButtonCommandParameterProperty, value); }
    public static readonly BindableProperty SaveButtonCommandParameterProperty =
        BindableProperty.Create(nameof(SaveButtonCommandParameter), typeof(object), typeof(SaveBarView), propertyChanged: OnSaveButtonCommandParameterPropertyChanged);
    static void OnSaveButtonCommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        o.SaveButton.CommandParameter = newValue;
    }

    public string SaveButtonText { get => (string)GetValue(SaveButtonTextProperty); set => SetValue(SaveButtonTextProperty, value); }
    public static readonly BindableProperty SaveButtonTextProperty =
        BindableProperty.Create(nameof(SaveButtonText), typeof(string), typeof(SaveBarView), "Save", propertyChanged: OnSaveButtonTextChanged);
    static void OnSaveButtonTextChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as SaveBarView;
        o.SaveButton.Text = (string)newValue;
    }

    #endregion
}
