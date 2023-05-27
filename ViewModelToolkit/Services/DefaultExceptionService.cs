namespace ViewModelToolkit.Services;

public class DefaultExceptionService : IExceptionService
{
    public void HandleException(Exception exception) {
        System.Diagnostics.Debug.WriteLine($"DefaultExceptionHandler: ex => {exception}");
        throw exception;
    }
}
