using ViewModelToolkit.Services;
using ViewModelToolkit.ViewModels;
using ViewModelToolkit.Views;

namespace ViewModelToolkit.Dialogs;

public enum NullResultHandling { ReturnDefault, ReturnInput }

public static partial class CoreNavigation
{
    static CoreNavigation() {
        var resources = Application.Current.Resources;
        var list = resources.MergedDictionaries.ToList();
        resources.MergedDictionaries.Clear();
        resources.MergedDictionaries.Add(new Styles());
        list.ForEach(d =>
            resources.MergedDictionaries.Add(d));
    }

    static INavigation Navigation => Application.Current.MainPage?.Navigation;
    public static IDependencyResolver CurrentDependencyResolver { get; private set; } = new DefaultDependencyResolver();
    static SaveBarDisplayMode DefaultSaveBarDisplayMode { get; set; } = SaveBarDisplayMode.Default;

    public static void ConfigureDefaultButtonBarDisplayMode(SaveBarDisplayMode mode) => DefaultSaveBarDisplayMode = mode;
    public static void ConfigureDefaultCancelWhenDirtyAlertDetails(AlertDetails details) => DefaultCancelWhenDirtyAlertDetails = details;
    public static void ConfigureExceptionHandler(IExceptionService handler) => CurrentExceptionService = handler;
    public static void ConfigureDependencyResolver(IDependencyResolver resolver) => CurrentDependencyResolver = resolver;

    internal static AlertDetails DefaultCancelWhenDirtyAlertDetails { get; set; } = new();
    public static IExceptionService CurrentExceptionService { get; set; } = new DefaultExceptionService();

    /// <summary>
    /// Provides navigation to a ContentPage of type TPage, with a non-generic View Model of type ViewModelBase
    /// initialized using either the default Initialize() method or the optional <paramref name="initialization"/> action.
    /// </summary>
    /// <typeparam name="TPage">Type</typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="useTransitionAnimation">An optional boolean indicating whether to use the default animation (default: <see langword="true"/>)</param>
    /// <param name="initialization">An optional <![CDATA[Action<TPage, TViewModel>]]> initializer</param>
    /// <exception cref="NullReferenceException">Thrown if the DependencyResolver couldn't resolve the View Model type.</exception>
    public static void NavigateToPage<TPage, TViewModel>(
            bool useTransitionAnimation = true,
            Action<TPage, TViewModel> initialization = null
        )
            where TPage : ContentPage, new()
            where TViewModel : ViewModelBase {
        try {
            var page = new TPage();
            var vm = CurrentDependencyResolver.Resolve<TViewModel>()
                ?? throw new NullReferenceException($"Could not resolve and construct the type specified by TViewModel");

            vm.InitializeCleanly(() => {
                if ( initialization == null )
                    vm.Initialize();
                else
                    initialization.Invoke(page, vm);

                page.BindingContext = vm;
                vm.IsDirty = false;
            });

            if ( page != null )
                Navigation.PushAsync(page, useTransitionAnimation);
        } catch ( Exception ex ) {
            System.Diagnostics.Debug.WriteLine($"NavigateToCorePage ex: {ex}");
            CurrentExceptionService.HandleException(ex);
        }
    }

    /// <summary>
    /// Provides navigation to a ContentPage of type <typeparamref name="TPage"/>, with a generic View Model of type ViewModelBase<typeparamref name="TResult"/>
    /// initialized using either the default Initialize() method or the optional <paramref name="initialization"/> action.
    /// </summary>
    /// <typeparam name="TResult">Type of the Model object passed in as <paramref name="input"/> and returned.</typeparam>
    /// <typeparam name="TPage">Type for the page to be navigated to. Must derive from ContentPage.</typeparam>
    /// <typeparam name="TViewModel">Type for the View Model to be navigated to. Must derive from ModalViewModelBase<typeparamref name="TResult"/>></typeparam>
    /// <param name="input">Model of type <typeparamref name="TResult"/> used to initialize the View Model</param>
    /// <param name="useTransitionAnimation">An optional boolean indicating whether to use the default animation (default: <see langword="true"/>)</param>
    /// <param name="initialization">An optional <![CDATA[Action<TPage, TViewModel>]]> initializer</param>
    /// <exception cref="NullReferenceException">Thown if the DependencyResolver couldn't resolve the View Model type.</exception>
    public static void NavigateToPage<TResult, TPage, TViewModel>(TResult input, bool useTransitionAnimation = true, Action<TPage, TViewModel> initialization = null)
            where TPage : ContentPage, new()
            where TViewModel : ViewModelBase<TResult> {
        try {
            var page = new TPage();
            var vm = CurrentDependencyResolver.Resolve<TViewModel>()
                ?? throw new NullReferenceException($"Could not resolve and construct the type specified by TViewModel ({typeof(TViewModel).FullName})");

            vm.InitializeCleanly(() => {
                if ( initialization == null )
                    vm.Initialize(input);
                else
                    initialization.Invoke(page, vm);

                page.BindingContext = vm;
                vm.IsDirty = false;
            });

            Navigation.PushAsync(page, useTransitionAnimation);
        } catch ( Exception ex ) {
            System.Diagnostics.Debug.WriteLine($"NavigateToCorePage ex: {ex}");
            CurrentExceptionService.HandleException(ex);
        }
    }

    /// <summary>
    /// Provides navigation to a ContentPage of type <typeparamref name="TPage"/>, with a generic View Model of type
    /// ViewModelBase<typeparamref name="TResult"/> initialized using either the default Initialize() method or the
    /// optional <paramref name="initialization"/> action.
    /// </summary>
    /// <typeparam name="TResult">Type of the Model object passed in as <paramref name="input"/> and returned.</typeparam>
    /// <typeparam name="TPage">Type for the page to be navigated to. Must derive from ContentPage.</typeparam>
    /// <typeparam name="TViewModel">Type for the View Model to be navigated to. Must derive from ModalViewModelBase<typeparamref name="TResult"/>></typeparam>
    /// <param name="input">Model of type <typeparamref name="TResult"/> used to initialize the View Model</param>
    /// <param name="useTransitionAnimation">An optional boolean indicating whether to use the default animation (default: <see langword="true"/>)</param>
    /// <param name="nullResultHandling">An optional value of enum type NullResultHandling indicating whether to return <see langword="default"/> or the value of <paramref name="input"/> (default: ReturnDefault)</param>
    /// <param name="initialization">An optional <![CDATA[Action<TPage, TViewModel>]]> initializer</param>
    /// <param name="saveBarInjector">An optional Func<![CDATA[<ContentPage, ISaveBarView>]]> that permits custom creation and insertion of an ISaveBarView instance into the visual tree.</param>
    /// <param name="shouldSuppressReturnNavigationAnimation"></param>
    /// <returns>Result of the modal action of type <typeparamref name="TResult"/></returns>
    /// <exception cref="NullReferenceException">Thown if the DependencyResolver couldn't resolve the View Model type.</exception>
    public static async Task<TResult> NavigateToModalPageAsync<TResult, TPage, TViewModel>(
            TResult input,
            bool useTransitionAnimation = true,
            NullResultHandling nullResultHandling = NullResultHandling.ReturnDefault,
            Action<TPage, TViewModel> initialization = null,
            Func<TPage, ISaveBarView> saveBarInjector = null,
            Func<TResult, bool> shouldSuppressReturnNavigationAnimation = null
        )
            where TPage : ContentPage, new()
            where TViewModel : ViewModelBase<TResult>, IDialogSupport<TResult> {
        try {
            var page = CurrentDependencyResolver.Resolve<TPage>() ?? new TPage();
            var vm = CurrentDependencyResolver.Resolve<TViewModel>()
                ?? throw new NullReferenceException($"Could not resolve and construct the type specified by TViewModel");

            vm.DialogManager.Configure(
                page,
                DefaultSaveBarDisplayMode,
                saveBarInjector,
                DefaultCancelWhenDirtyAlertDetails,
                CurrentExceptionService);

            vm.InitializeCleanly(() => {
                if ( initialization is not null )
                    initialization.Invoke(page, vm);
                else
                    vm.Initialize(input);

                page.BindingContext = vm;

                vm.IsDirty = false;
            });

            var navPage = new NavigationPage(page);
            await Navigation.PushModalAsync(navPage, useTransitionAnimation);

            TResult result = await vm.DialogManager.ExecuteModalTaskAsync();

            bool shouldAnimate = useTransitionAnimation
                && !(shouldSuppressReturnNavigationAnimation?.Invoke(result) ?? false);

            await Navigation.PopModalAsync(shouldAnimate);

            return result.IsDefault()
                ? nullResultHandling == NullResultHandling.ReturnInput ? vm.Source : default
                : result;

        } catch ( Exception ex ) {
            CurrentExceptionService.HandleException(ex);
            return default;
        }
    }
}


public static partial class CoreNavigation
{
    public static bool IsDefault<T>(this T value) {
        return EqualityComparer<T>.Default.Equals(value, default(T));
    }
}
