namespace ViewModelToolkit.Services;

public interface IExceptionService
{
    void HandleException(Exception exception);
}


public class ExceptionService : IExceptionService
{
    public void HandleException(Exception exception) {
        System.Diagnostics.Debug.WriteLine($"DefaultExceptionHandler: ex => {exception}");
        throw exception;
    }
}
