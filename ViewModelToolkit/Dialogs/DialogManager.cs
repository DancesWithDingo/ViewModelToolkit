using System.ComponentModel;
using ViewModelToolkit.Services;
using ViewModelToolkit.ViewModels;
using ViewModelToolkit.Views;

namespace ViewModelToolkit.Dialogs;

public enum SaveBarDisplayMode { None, Default, SaveBarOnly, ToolBarOnly, BothToolBarAndSaveBar }

public sealed partial class DialogManager<TResult> : BindableObject
{
    public DialogManager(ViewModelBase<TResult> vm) {
        ViewModel = vm;

        if ( OperatingSystem.IsAndroid() )
            IsCancelToolbarItemVisible = false;
    }

    bool isConfigured = false;

    #region Properties

    ToolbarManager ToolbarManager { get; } = new();
    ViewModelBase<TResult> ViewModel { get; init; }
    IExceptionService ExceptionService { get; set; }

    #endregion

    #region Bindable Properties

    public Command CancelButtonCommand { get => (Command)GetValue(CancelButtonCommandProperty); set => SetValue(CancelButtonCommandProperty, value); }
    public static readonly BindableProperty CancelButtonCommandProperty =
        BindableProperty.Create(nameof(CancelButtonCommand), typeof(Command), typeof(DialogManager<TResult>), propertyChanged: OnCancelButtonCommandPropertyChanged);
    static void OnCancelButtonCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        var cmd = (Command)newValue;
        o.ToolbarManager.SetCancelButtonCommand(cmd);
        cmd?.ChangeCanExecute();
    }

    public object CancelButtonCommandParameter { get => (object)GetValue(CancelButtonCommandParameterProperty); set => SetValue(CancelButtonCommandParameterProperty, value); }
    public static readonly BindableProperty CancelButtonCommandParameterProperty =
        BindableProperty.Create(nameof(CancelButtonCommandParameter), typeof(object), typeof(DialogManager<TResult>), 0, propertyChanged: OnCancelButtonCommandParameterPropertyChanged);
    static void OnCancelButtonCommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        o.ToolbarManager.SetCancelButtonCommandParameter(newValue);
    }

    public string CancelButtonText { get => (string)GetValue(CancelButtonTextProperty); set => SetValue(CancelButtonTextProperty, value); }
    public static readonly BindableProperty CancelButtonTextProperty =
        BindableProperty.Create(nameof(CancelButtonText), typeof(string), typeof(DialogManager<TResult>), "Cancel", propertyChanged: OnCancelButtonTextPropertyChanged);
    static void OnCancelButtonTextPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        o.ToolbarManager.SetCancelButtonText((string)newValue);
    }

    public AlertDetails CancelWhenDirtyAlertDetails { get => (AlertDetails)GetValue(CancelWhenDirtyAlertDetailsProperty); set => SetValue(CancelWhenDirtyAlertDetailsProperty, value); }
    public static readonly BindableProperty CancelWhenDirtyAlertDetailsProperty =
        BindableProperty.Create(nameof(CancelWhenDirtyAlertDetails), typeof(AlertDetails), typeof(DialogManager<TResult>));

    public SaveBarDisplayMode DisplayMode { get => (SaveBarDisplayMode)GetValue(DisplayModeProperty); set => SetValue(DisplayModeProperty, value); }
    public static readonly BindableProperty DisplayModeProperty =
        BindableProperty.Create(nameof(DisplayMode), typeof(SaveBarDisplayMode), typeof(DialogManager<TResult>), SaveBarDisplayMode.Default, propertyChanged: OnButtonBarDisplayModePropertyChanged);
    static void OnButtonBarDisplayModePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        var mode = (SaveBarDisplayMode)newValue;
        if ( !o.isConfigured ) throw new InvalidOperationException($"{nameof(DialogManager<TResult>)} must be configured using the Configure(ContentPage, SaveBarDisplayMode) method.");
        o.ToolbarManager.DisplayMode = mode;
    }

    public bool IsCancelButtonVisible { get => (bool)GetValue(IsCancelButtonVisibleProperty); set => SetValue(IsCancelButtonVisibleProperty, value); }
    public static readonly BindableProperty IsCancelButtonVisibleProperty =
        BindableProperty.Create(nameof(IsCancelButtonVisible), typeof(bool), typeof(DialogManager<TResult>), true, propertyChanged: OnIsCancelButtonVisiblePropertyChanged);
    static void OnIsCancelButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        o.ToolbarManager.IsCancelButtonVisible = (bool)newValue;
    }

    public bool IsCancelToolbarItemVisible { get => (bool)GetValue(IsCancelToolbarItemVisibleProperty); set => SetValue(IsCancelToolbarItemVisibleProperty, value); }
    public static readonly BindableProperty IsCancelToolbarItemVisibleProperty =
        BindableProperty.Create(nameof(IsCancelToolbarItemVisible), typeof(bool), typeof(DialogManager<TResult>), true, propertyChanged: OnIsCancelToolbarItemVisiblePropertyChanged);
    static void OnIsCancelToolbarItemVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        o.ToolbarManager.IsCancelToolbarItemVisible = (bool)newValue;
    }

    public bool IsSaveButtonVisible { get => (bool)GetValue(IsSaveButtonVisibleProperty); set => SetValue(IsSaveButtonVisibleProperty, value); }
    public static readonly BindableProperty IsSaveButtonVisibleProperty =
        BindableProperty.Create(nameof(IsSaveButtonVisible), typeof(bool), typeof(DialogManager<TResult>), true, propertyChanged: OnIsSaveButtonVisiblePropertyChanged);
    static void OnIsSaveButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        o.ToolbarManager.IsSaveButtonVisible = (bool)newValue;
    }

    public bool IsSaveButtonAlwaysEnabled { get => (bool)GetValue(IsSaveButtonAlwaysEnabledProperty); set => SetValue(IsSaveButtonAlwaysEnabledProperty, value); }
    public static readonly BindableProperty IsSaveButtonAlwaysEnabledProperty =
        BindableProperty.Create(nameof(IsSaveButtonAlwaysEnabled), typeof(bool), typeof(DialogManager<TResult>));

    public Command SaveButtonCommand { get => (Command)GetValue(SaveButtonCommandProperty); set => SetValue(SaveButtonCommandProperty, value); }
    public static readonly BindableProperty SaveButtonCommandProperty =
        BindableProperty.Create(nameof(SaveButtonCommand), typeof(Command), typeof(DialogManager<TResult>), propertyChanged: OnSaveButtonCommandPropertyChanged);
    static void OnSaveButtonCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        var cmd = (Command)newValue;
        o.ToolbarManager.SetSaveButtonCommand(cmd);
        cmd?.ChangeCanExecute();
    }

    public object SaveButtonCommandParameter { get => (object)GetValue(SaveButtonCommandParameterProperty); set => SetValue(SaveButtonCommandParameterProperty, value); }
    public static readonly BindableProperty SaveButtonCommandParameterProperty =
        BindableProperty.Create(nameof(SaveButtonCommandParameter), typeof(object), typeof(DialogManager<TResult>), propertyChanged: OnSaveButtonCommandParameterPropertyChanged);
    static void OnSaveButtonCommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        o.ToolbarManager.SetCancelButtonCommandParameter(newValue);
    }

    public string SaveButtonText { get => (string)GetValue(SaveButtonTextProperty); set => SetValue(SaveButtonTextProperty, value); }
    public static readonly BindableProperty SaveButtonTextProperty =
        BindableProperty.Create(nameof(SaveButtonText), typeof(string), typeof(DialogManager<TResult>), "Save", propertyChanged: OnSaveButtonTextPropertyChanged);
    static void OnSaveButtonTextPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as DialogManager<TResult>;
        o.ToolbarManager.SetSaveButtonText((string)newValue);
    }

    public bool ShouldCancelIgnoreIsDirty { get => (bool)GetValue(ShouldButtonIgnoreIsDirtyProperty); set => SetValue(ShouldButtonIgnoreIsDirtyProperty, value); }
    public static readonly BindableProperty ShouldButtonIgnoreIsDirtyProperty =
        BindableProperty.Create(nameof(ShouldCancelIgnoreIsDirty), typeof(bool), typeof(DialogManager<TResult>));

    #endregion

    #region Public Methods

    /// <summary>
    /// Call ChangeCanExecute() on SaveButtonCommand and CancelButtonCommand.
    /// </summary>
    public void ChangeCommandsCanExecute() {
        SaveButtonCommand?.ChangeCanExecute();
        CancelButtonCommand?.ChangeCanExecute();
    }

    public void Configure<TPage>(
        TPage page,
        SaveBarDisplayMode displayMode = SaveBarDisplayMode.Default,
        Func<TPage, ISaveBarView> saveBarInjector = null,
        AlertDetails cancelWhenDirtyAlertDetails = null,
        IExceptionService exceptionService = null
    ) where TPage : ContentPage {
        CancelButtonCommand = DefaultCancelButtonCommand;
        SaveButtonCommand = DefaultSaveButtonCommand;

        ToolbarManager.Configure<TResult, TPage>(page, displayMode, saveBarInjector);

        CancelWhenDirtyAlertDetails = cancelWhenDirtyAlertDetails;
        ExceptionService = exceptionService ?? new ExceptionService();

        ViewModel.PropertyChanged -= ViewModelIsDirtyChangedHandler;
        ViewModel.PropertyChanged += ViewModelIsDirtyChangedHandler;

        isConfigured = true;

        void ViewModelIsDirtyChangedHandler(object sender, PropertyChangedEventArgs e) {
            if ( e.PropertyName == ViewModelBase.IsDirtyProperty.PropertyName )
                ChangeCommandsCanExecute();
        }
    }

    /// <summary>
    /// Executes the DefaultCancelButtonCommand
    /// </summary>
    /// <param name="forceIfDirty">An optional boolean value indicating whether to set the IsDirty flag to true before execution. (default: <see langword="false"/>)</param>
    public void ExecuteDefaultCancelButtonCommand(bool forceIfDirty = false) {
        if ( forceIfDirty )
            ViewModel.IsDirty = false;
        DefaultCancelButtonCommand?.Execute(null);
    }

    /// <summary>
    /// Executes the DefaultSaveButtonCommand
    /// </summary>
    /// <param name="executeSafely">An optional boolean value indicating whether to check the CanExecute parameter of the command (default: <see langword="true"/>)</param>
    public void ExecuteDefaultSaveButtonCommand(object commandParameter = null, bool executeSafely = true) {
        if ( !executeSafely
                || (SaveButtonCommand is not null && SaveButtonCommand.CanExecute(commandParameter)) )
            DefaultSaveButtonCommand?.Execute(commandParameter);
    }

    #endregion

    #region Task Completion

    TaskCompletionSource<TResult> tcs;

    public Task<TResult> ExecuteModalTaskAsync() {
        tcs = new TaskCompletionSource<TResult>();
        return tcs.Task;
    }

    void SetDialogResult(TResult result) => tcs.SetResult(result);

    #endregion

    #region Commands

    public Command DefaultCancelButtonCommand => _DefaultCancelButtonCommand ??= new Command(async _ => {
        if ( !ViewModel.IsDirty ||
                (ViewModel.IsDirty &&
                    (ShouldCancelIgnoreIsDirty ||
                        await ConfirmIsDirtyCancelAsync(CancelWhenDirtyAlertDetails)))
           )
            SetDialogResult(default);
    });
    Command _DefaultCancelButtonCommand;

    public Command DefaultSaveButtonCommand => _DefaultSaveButtonCommand ??= new Command(p => {
        if ( !ViewModel.Validate() ) return;

        try {
            TResult result = ViewModel.Update();
            SetDialogResult(result);
            if ( result != null )
                ViewModel.IsDirty = false;

        } catch ( Exception ex ) {
            CoreNavigation.CurrentExceptionService.HandleException(ex);
        }
    }, p => IsSaveButtonAlwaysEnabled || (ViewModel?.IsDirty ?? false) && (ViewModel?.IsValid ?? false));
    Command _DefaultSaveButtonCommand;

    #endregion

    #region Locals

    static async Task<bool> ConfirmIsDirtyCancelAsync(AlertDetails alertDetails = null) {
        var nav = Application.Current.MainPage.Navigation;
        var currentPage = nav.NavigationStack.Union(nav.ModalStack).LastOrDefault();
        var details = alertDetails ?? new AlertDetails();
        return await currentPage?.DisplayAlert(details.Title, details.Description, details.YesText, details.NoText);
    }

    #endregion
}
