using System.Windows.Input;
using ViewModelToolkit.Views;

namespace ViewModelToolkit.Dialogs;

sealed class ToolbarManager : BindableObject
{
    const string CANCEL_BUTTON_TEXT = "Cancel";
    const string SAVE_BUTTON_TEXT = "Save";

    #region  Properties

    ToolbarItem CancelToolbarItem { get; } = new ToolbarItem { Text = CANCEL_BUTTON_TEXT, Priority = int.MinValue };
    ToolbarItem SaveToolbarItem { get; } = new ToolbarItem { Text = SAVE_BUTTON_TEXT, Priority = int.MaxValue };

    ISaveBarView SaveBar { get; set; }
    IList<ToolbarItem> ToolbarItems { get; set; }

    #endregion

    #region Bindable Properties

    public SaveBarDisplayMode DisplayMode { get => (SaveBarDisplayMode)GetValue(DisplayModeProperty); set => SetValue(DisplayModeProperty, value); }
    public static readonly BindableProperty DisplayModeProperty =
        BindableProperty.Create(nameof(DisplayMode), typeof(SaveBarDisplayMode), typeof(ToolbarManager), SaveBarDisplayMode.Default, propertyChanged: OnDisplayModePropertyChanged);
    static void OnDisplayModePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as ToolbarManager;
        o.UpdateButtons();
    }

    public bool IsCancelButtonVisible { get => (bool)GetValue(IsCancelButtonVisibleProperty); set => SetValue(IsCancelButtonVisibleProperty, value); }
    public static readonly BindableProperty IsCancelButtonVisibleProperty =
        BindableProperty.Create(nameof(IsCancelButtonVisible), typeof(bool), typeof(ToolbarManager), true, propertyChanged: OnIsCancelButtonVisiblePropertyChanged);
    static void OnIsCancelButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as ToolbarManager;
        o.SaveBar.IsCancelButtonVisible = (bool)newValue;
        o.UpdateButtons();
    }

    public bool IsCancelToolbarItemVisible { get => (bool)GetValue(IsCancelToolbarItemVisibleProperty); set => SetValue(IsCancelToolbarItemVisibleProperty, value); }
    public static readonly BindableProperty IsCancelToolbarItemVisibleProperty =
        BindableProperty.Create(nameof(IsCancelToolbarItemVisible), typeof(bool), typeof(ToolbarManager), true, propertyChanged: OnIsCancelToolbarItemVisiblePropertyChanged);
    static void OnIsCancelToolbarItemVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as ToolbarManager;
        o.UpdateButtons();
    }

    public bool IsSaveButtonVisible { get => (bool)GetValue(IsSaveButtonVisibleProperty); set => SetValue(IsSaveButtonVisibleProperty, value); }
    public static readonly BindableProperty IsSaveButtonVisibleProperty =
        BindableProperty.Create(nameof(IsSaveButtonVisible), typeof(bool), typeof(ToolbarManager), true, propertyChanged: OnIsSaveButtonVisiblePropertyChanged);
    static void OnIsSaveButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as ToolbarManager;
        o.SaveBar.IsSaveButtonVisible = (bool)newValue;
        o.UpdateButtons();
    }

    #endregion

    #region Public Methods

    public void Configure<TModel, TView>(
            TView page,
            SaveBarDisplayMode displayMode,
            Func<TView, ISaveBarView> saveBarInjector = null
    ) where TView : ContentPage {
        _ = page ?? throw new ArgumentNullException(nameof(page));

        SaveBar = page.FindFirstDescendent<ISaveBarView>() ??
            saveBarInjector?.Invoke(page) ??
            DefaultBuildSaveBarInjector(page);

        ToolbarItems = page.ToolbarItems;

        DisplayMode = displayMode;
        
        ConfigureButtons(CancelToolbarItem, SaveBar, false);
        ConfigureButtons(SaveToolbarItem, SaveBar, true);
        UpdateButtons();
    }

    public void SetCancelButtonCommand(ICommand command) {
        CancelToolbarItem.Command = command;
        if ( SaveBar is not null )
            SaveBar.CancelButtonCommand = command;
    }

    public void SetCancelButtonCommandParameter(object parameter) {
        CancelToolbarItem.CommandParameter = parameter;
        if ( SaveBar is not null )
            SaveBar.CancelButtonCommandParameter = parameter;
    }

    public void SetCancelButtonText(string text) {
        CancelToolbarItem.Text = text;
        if ( SaveBar is not null )
            SaveBar.CancelButtonText = text;
    }

    public void SetSaveButtonCommand(ICommand command) {
        SaveToolbarItem.Command = command;
        if ( SaveBar is not null )
            SaveBar.SaveButtonCommand = command;
    }

    public void SetSaveButtonCommandParameter(object parameter) {
        SaveToolbarItem.CommandParameter = parameter;
        if ( SaveBar is not null )
            SaveBar.SaveButtonCommandParameter = parameter;
    }

    public void SetSaveButtonText(string text) {
        SaveToolbarItem.Text = text;
        if ( SaveBar is not null )
            SaveBar.SaveButtonText = text;
    }

    #endregion

    #region Locals

    static void ConfigureButtons(ToolbarItem toolbarItem, ISaveBarView saveBar, bool isSaveButton) {
        var text = isSaveButton ? saveBar.SaveButtonText : saveBar.CancelButtonText;
        if ( string.IsNullOrEmpty(text) )
            if ( isSaveButton )
                saveBar.SaveButtonText = toolbarItem.Text;
            else
                saveBar.CancelButtonText = toolbarItem.Text;
        else
            toolbarItem.Text = text;

        var cmd = isSaveButton ? saveBar.SaveButtonCommand : saveBar.CancelButtonCommand;
        if ( cmd is null )
            if ( isSaveButton )
                saveBar.SaveButtonCommand = toolbarItem.Command;
            else
                saveBar.CancelButtonCommand = toolbarItem.Command;
        else
            toolbarItem.Command = cmd;

        var parm = isSaveButton ? saveBar.SaveButtonCommandParameter : saveBar.CancelButtonCommandParameter;
        if ( parm is null )
            if ( isSaveButton )
                saveBar.SaveButtonCommandParameter = toolbarItem.CommandParameter;
            else
                saveBar.CancelButtonCommandParameter = toolbarItem.CommandParameter;
        else
            toolbarItem.CommandParameter = parm;
    }

    static SaveBarView DefaultBuildSaveBarInjector<T>(T page) where T : ContentPage {
        var bar = new SaveBarView();
        var firstLayout = page.FindFirstDescendent<Layout>();
        firstLayout?.Add(bar);
        return bar;
    }

    void UpdateButtons() {
        ToolbarItems.Remove(CancelToolbarItem);
        ToolbarItems.Remove(SaveToolbarItem);

        if ( ShouldDisplayToolbar(DisplayMode) ) {
            if ( IsCancelButtonVisible && IsCancelToolbarItemVisible ) ToolbarItems.Add(CancelToolbarItem);
            if ( IsSaveButtonVisible ) ToolbarItems.Add(SaveToolbarItem);
        }

        SaveBar.IsVisible = ShouldDisplaySaveBar(DisplayMode);
        SaveBar.IsCancelButtonVisible = IsCancelButtonVisible;
        SaveBar.IsSaveButtonVisible = IsSaveButtonVisible;
    }

    static bool ShouldDisplaySaveBar(SaveBarDisplayMode mode) =>
        mode == SaveBarDisplayMode.SaveBarOnly ||
        mode == SaveBarDisplayMode.BothToolBarAndSaveBar ||
       (mode == SaveBarDisplayMode.Default && DeviceInfo.Idiom == DeviceIdiom.Desktop);

    static bool ShouldDisplayToolbar(SaveBarDisplayMode mode) =>
        mode == SaveBarDisplayMode.ToolBarOnly ||
        mode == SaveBarDisplayMode.BothToolBarAndSaveBar ||
       (mode == SaveBarDisplayMode.Default && DeviceInfo.Idiom != DeviceIdiom.Desktop);

    #endregion
}
