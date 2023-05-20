namespace ViewModelToolkit;

public interface IDependencyResolver
{
    T Resolve<T>();
}


public class DefaultDependencyResolver : IDependencyResolver
{
    public T Resolve<T>() => (T)Activator.CreateInstance<T>();
}
