namespace ViewModelToolkit;

public interface IDependencyResolver
{
    T Resolve<T>() where T : class;
}


public class DefaultDependencyResolver : IDependencyResolver
{
    public T Resolve<T>() where T : class => (T)Activator.CreateInstance<T>();
}
