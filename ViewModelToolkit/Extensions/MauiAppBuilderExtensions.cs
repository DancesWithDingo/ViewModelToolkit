namespace ViewModelToolkit;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder ConfigureViewModelToolkit(this MauiAppBuilder builder) {
        builder.ConfigureMauiHandlers(handlers => handlers.AddHandler(typeof(ContentPage), typeof(ModalPageHandler)));
        return builder;
    }
}
