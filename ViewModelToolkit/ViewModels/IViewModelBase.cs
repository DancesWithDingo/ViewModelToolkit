using ViewModelToolkit.Dialogs;

namespace ViewModelToolkit.ViewModels;

public interface IViewModelBase
{
    /// <summary>
    /// Bindable Property IsDirty, set when a Set override is called without overriding the
    /// </summary>
    bool IsDirty { get; set; }

    /// <summary>
    /// Basic View Model initializer (should not be called from ViewModelBase<![CDATA[<T>]]> descendents)
    /// </summary>
    void Initialize();

    /// <summary>
    /// Provides support for executing actions without changing IsDirty
    /// </summary>
    /// <param name="action">Action to execute cleanly</param>
    void InitializeCleanly(Action action);
}


public interface IViewModelBase<T> : IViewModelBase
{
    /// <summary>
    /// Backing store property for the ViewModel. Holds the original value that was used to initialize
    /// the View Model. The Source property should not be modified after initialization.
    /// </summary>
    T Source { get; }

    /// <summary>
    /// A boolean value indicating if the last call to Validate() returned <see langword="true"/>.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Base View Model initializer, sets the value for Source
    /// </summary>
    /// <param name="item">The item to be assigned to the Source variable of type <typeparamref name="T"/></param>
    /// <exception cref="ArgumentNullException"></exception>
    void Initialize(T item);

    /// <summary>
    /// A <see langword="virtual"/> method creates and returns an object of type <typeparamref name="T"/>
    /// containing the current property values from the View Model.
    /// </summary>
    /// <returns>An object of type <typeparamref name="T"/> containing the current property values
    /// from the View Model.</returns>
    T Update();


    /// <summary>
    /// A virtual function that can be overriden by inheritors to provide logic for validating
    /// the state of the View Model properties. Sets the IsValid flag before returning the result.
    /// </summary>
    /// <returns>A boolean value indicating if the validation was successful</returns>
    bool Validate();
}


public interface IModalViewModelBase<T> : IViewModelBase<T>, IDialogSupport<T> { }

/// <summary>
/// Useful extension interface to provide a static Init method for simpler instantiation.
/// </summary>
public interface IViewModelInit<T, VM> : IViewModelInit
{
    static abstract VM Init(T item);
}
public interface IViewModelInit { }
