using ViewModelToolkit.Dialogs;
using ViewModelToolkit.Services;
using ViewModelToolkit.ViewModels;
using ViewModelToolkit.Views;

namespace ViewModelToolkit;

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
    static SaveBarDisplayMode DefaultSaveBarDisplayMode { get; set; } = SaveBarDisplayMode.Default;

    public static IDependencyResolver CurrentDependencyResolver { get; private set; } = new DefaultDependencyResolver();
    internal static AlertDetails DefaultCancelWhenDirtyAlertDetails { get; private set; } = new();
    public static IExceptionService CurrentExceptionService { get; private set; } = new DefaultExceptionService();

    public static void ConfigureDefaultButtonBarDisplayMode(SaveBarDisplayMode mode) => DefaultSaveBarDisplayMode = mode;
    public static void ConfigureDefaultCancelWhenDirtyAlertDetails(AlertDetails details) => DefaultCancelWhenDirtyAlertDetails = details ?? new();
    public static void ConfigureExceptionHandler(IExceptionService handler) => CurrentExceptionService = handler ?? new DefaultExceptionService();
    public static void ConfigureDependencyResolver(IDependencyResolver resolver) => CurrentDependencyResolver = resolver ?? new DefaultDependencyResolver();

    /// <summary>
    /// Provides navigation to a ContentPage of type TView, with a non-generic ViewModel of type ViewModelBase
    /// initialized using either the default Initialize() method or the optional <paramref name="initialization"/> action.
    /// </summary>
    /// <typeparam name="TView">Type</typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="useTransitionAnimation">An optional boolean indicating whether to use the default animation (default: <see langword="true"/>)</param>
    /// <param name="initialization">An optional <![CDATA[Action<TView, TViewModel>]]> initializer</param>
    /// <exception cref="NullReferenceException">Thrown if the DependencyResolver couldn't resolve the ViewModel type.</exception>
    public static void NavigateToPage<TView, TViewModel>(
            bool useTransitionAnimation = true,
            Action<TView, TViewModel> initialization = null
        )
            where TView : ContentPage, new()
            where TViewModel : ViewModelBase {
        try {
            var page = CurrentDependencyResolver.Resolve<TView>() ?? new TView();
            var vm = CurrentDependencyResolver.Resolve<TViewModel>()
                ?? throw new NullReferenceException($"Could not resolve and construct the type specified by TViewModel");

            vm.ExecuteCleanly(() => {
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
            CurrentExceptionService.HandleException(ex);
        }
    }

    /// <summary>
    /// Provides navigation to a ContentPage of type <typeparamref name="TView"/>, with a generic ViewModel of type ViewModelBase<typeparamref name="TModel"/>
    /// initialized using either the default Initialize() method or the optional <paramref name="initialization"/> action.
    /// </summary>
    /// <typeparam name="TModel">Type of the Model object passed in as <paramref name="input"/> and returned.</typeparam>
    /// <typeparam name="TView">Type for the page to be navigated to. Must derive from ContentPage.</typeparam>
    /// <typeparam name="TViewModel">Type for the ViewModel to be navigated to. Must derive from ModalViewModelBase<typeparamref name="TModel"/>></typeparam>
    /// <param name="input">Model of type <typeparamref name="TModel"/> used to initialize the ViewModel</param>
    /// <param name="useTransitionAnimation">An optional boolean indicating whether to use the default animation (default: <see langword="true"/>)</param>
    /// <param name="initialization">An optional <![CDATA[Action<TView, TViewModel>]]> initializer</param>
    /// <exception cref="NullReferenceException">Thown if the DependencyResolver can't resolve the ViewModel type.</exception>
    public static void NavigateToPage<TModel, TView, TViewModel>(TModel input, bool useTransitionAnimation = true, Action<TView, TViewModel> initialization = null)
            where TView : ContentPage, new()
            where TViewModel : ViewModelBase<TModel> {
        try {
            var page = CurrentDependencyResolver.Resolve<TView>() ?? new TView();
            var vm = CurrentDependencyResolver.Resolve<TViewModel>()
                ?? throw new NullReferenceException($"Could not resolve and construct the type specified by TViewModel ({typeof(TViewModel).FullName})");

            vm.ExecuteCleanly(() => {
                if ( initialization == null )
                    vm.Initialize(input);
                else
                    initialization.Invoke(page, vm);

                page.BindingContext = vm;
                vm.IsDirty = false;
            });

            Navigation.PushAsync(page, useTransitionAnimation);
        } catch ( Exception ex ) {
            CurrentExceptionService.HandleException(ex);
        }
    }

    /// <summary>
    /// Provides navigation to a ContentPage of type <typeparamref name="TView"/>, with a generic ViewModel of type
    /// ViewModelBase<typeparamref name="TModel"/> initialized using either the default Initialize() method or the
    /// optional <paramref name="initialization"/> action.
    /// </summary>
    /// <typeparam name="TModel">Type of the Model object passed in as <paramref name="input"/> and returned.</typeparam>
    /// <typeparam name="TView">Type for the page to be navigated to. Must derive from ContentPage.</typeparam>
    /// <typeparam name="TViewModel">Type for the ViewModel to be navigated to. Must derive from ModalViewModelBase<typeparamref name="TModel"/>></typeparam>
    /// <param name="input">Model of type <typeparamref name="TModel"/> used to initialize the ViewModel</param>
    /// <param name="useTransitionAnimation">An optional boolean indicating whether to use the default animation (default: <see langword="true"/>)</param>
    /// <param name="nullResultHandling">An optional value of enum type NullResultHandling indicating whether to return <see langword="default"/> or the value of <paramref name="input"/> (default: ReturnDefault)</param>
    /// <param name="initialization">An optional <![CDATA[Action<TView, TViewModel>]]> initializer</param>
    /// <param name="saveBarInjector">An optional Func<![CDATA[<ContentPage, ISaveBarView>]]> that permits custom creation and insertion of an ISaveBarView instance into the visual tree.</param>
    /// <param name="shouldSuppressReturnNavigationAnimation"></param>
    /// <returns>Result of the modal action of type <typeparamref name="TModel"/></returns>
    /// <exception cref="NullReferenceException">Thown if the DependencyResolver couldn't resolve the ViewModel type.</exception>
    public static async Task<TModel> NavigateToModalPageAsync<TModel, TView, TViewModel>(
            TModel input,
            bool useTransitionAnimation = true,
            NullResultHandling nullResultHandling = NullResultHandling.ReturnDefault,
            Action<TView, TViewModel> initialization = null,
            Func<TView, ISaveBarView> saveBarInjector = null,
            Func<TModel, bool> shouldSuppressReturnNavigationAnimation = null
        )
            where TView : ContentPage, new()
            where TViewModel : ViewModelBase<TModel>, IDialogSupport<TModel> {
        try {
            var page = CurrentDependencyResolver.Resolve<TView>() ?? new TView();
            var vm = CurrentDependencyResolver.Resolve<TViewModel>()
                ?? throw new NullReferenceException($"Could not resolve and construct the type specified by TViewModel");

            vm.DialogManager.Configure(
                page,
                DefaultSaveBarDisplayMode,
                saveBarInjector,
                DefaultCancelWhenDirtyAlertDetails,
                CurrentExceptionService);

            vm.ExecuteCleanly(() => {
                if ( initialization is not null )
                    initialization.Invoke(page, vm);
                else
                    vm.Initialize(input);

                page.BindingContext = vm;

                vm.IsDirty = false;
            });

            var navPage = new NavigationPage(page);
            await Navigation.PushModalAsync(navPage, useTransitionAnimation);

            TModel result = await vm.DialogManager.ExecuteDialogTaskAsync();

            bool shouldAnimate = useTransitionAnimation
                && !(shouldSuppressReturnNavigationAnimation?.Invoke(result) ?? false);

            await Navigation.PopModalAsync(shouldAnimate);

            return result.IsDefault()
                ? nullResultHandling == NullResultHandling.ReturnInput
                    ? vm.Source
                    : default
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
