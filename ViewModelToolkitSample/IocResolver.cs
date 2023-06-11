using ViewModelToolkit;
using ViewModelToolkit.Services;
using ViewModelToolkitSample.ViewModels;

namespace ViewModelToolkitSample;

public class IocResolver : IDependencyResolver
{
    public T Resolve<T>() where T : class => IocResolverRegistration.Resolve<T>();
}


public static class IocResolverRegistration
{
    public static T Resolve<T>() where T : class => App.Current.MainPage.Handler.MauiContext.Services.GetService<T>();

    public static MauiAppBuilder ConfigureIocContainer(this MauiAppBuilder builder) {
        IServiceCollection services = builder.Services;

        services.AddSingleton<IExceptionService, DefaultExceptionService>();

        services.AddTransient<SimpleNavigationPageViewModel>();
        services.AddTransient<PickANumberPageViewModel>();
        services.AddTransient<EditCustomerStep1PageViewModel>();
        services.AddTransient<EditCustomerStep2PageViewModel>();
        services.AddTransient<EditCustomerStep3PageViewModel>();
        services.AddTransient<CustomFormPageViewModel>();

        return builder;
    }
}

