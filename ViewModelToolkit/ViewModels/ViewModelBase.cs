using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ViewModelToolkit.Dialogs;

namespace ViewModelToolkit.ViewModels;

/// <summary>
/// Base class for all View Models. Provides initialization and manages the IsDirty flag.
/// </summary>
public abstract class ViewModelBase : BindableObject, IViewModelBase
{
    protected bool isInitializing { get; set; } = false;

    /// <inheritdoc/>
    public virtual void Initialize() { }

    /// <inheritdoc/>
    [DebuggerNonUserCode]
    public void InitializeCleanly(Action action) {
        isInitializing = true;
        action?.Invoke();
        isInitializing = false;
    }

    /// <inheritdoc/>
    public bool IsDirty { get => (bool)GetValue(IsDirtyProperty); set => SetValue(IsDirtyProperty, value); }
    public static readonly BindableProperty IsDirtyProperty =
        BindableProperty.Create(nameof(IsDirty), typeof(bool), typeof(ViewModelBase), propertyChanged: OnIsDirtyPropertyChanged);
    static void OnIsDirtyPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        var o = bindable as ViewModelBase;
        if ( !o.isInitializing && oldValue != newValue )
            o.IsDirtyChanged((bool)newValue);
    }

    /// <summary>
    /// Called in overrides to notified when the IsDirty flag changes
    /// </summary>
    /// <param name="isDirty">New state of the IsDirty flag</param>
    protected virtual void IsDirtyChanged(bool isDirty) { }

    /// <summary>
    /// Set the value of the backing store to the new value, optionally calling
    /// a provided action and an optional flag whether to change the IsDirty property
    /// </summary>
    /// <typeparam name="T">The type of the property and backing store</typeparam>
    /// <param name="field">Backing field</param>
    /// <param name="newValue">New value to assign to property</param>
    /// <param name="setAction">Optional action to invoke after property is set (default: <see langword="null"/>)</param>
    /// <param name="setIsDirty">Optional to disable automatically setting the IsDirty flag (default: true)</param>
    /// <param name="propertyName">Name of this property (for internal use, do not assign)</param>
    /// <returns>Boolean indicating whether the property was set with a different value</returns>
    protected bool Set<T>(ref T field, T newValue, Action<T> setAction = null, bool setIsDirty = true, [CallerMemberName] string propertyName = null) {
        if ( EqualityComparer<T>.Default.Equals(field, newValue) )
            return false;

        field = newValue;
        OnPropertyChanged(propertyName);
        setAction?.Invoke(field);
        if ( setIsDirty )
            IsDirty = true;

        return true;
    }
}


/// <summary>
/// Base class for View Model classes, providing a generic Source property, an Update <see langword="virtual"/> method to fetch the
/// current property values from the View Model, and an optional Validate <see langword="virtual"/> method that can be
/// overriden if validation is required.
/// </summary>
/// <typeparam name="T">Type for the Source property</typeparam>
public abstract class ViewModelBase<T> : ViewModelBase, IViewModelBase<T>
{
    /// <inheritdoc/>
    public virtual void Initialize(T item) => Source = item ?? throw new ArgumentNullException(nameof(item), "Parameter item cannot be null.");

    /// <inheritdoc/>
    public T Source { get; private set; }

    /// <inheritdoc/>
    public virtual T Update() => default;

    /// <inheritdoc/>
    public bool IsValid { get; private set; } = true;

    /// <inheritdoc/>
    public virtual bool Validate() => Validate(true);

    /// <summary>
    /// Sets and returns the value of the IsValid flag. Call this method at the end of the
    /// Validate() function.
    /// </summary>
    /// <param name="isValid">A boolean indicating whether the validation was successful or not.</param>
    /// <returns>The value passed in through the isValid parameter</returns>
    protected bool Validate(bool isValid) {
        IsValid = isValid;
        if ( this is IDialogSupport<T> ds )
            ds.DialogManager.ChangeCommandsCanExecute();
        return isValid;
    }

    /// <summary>
    /// Set the value of the backing store to the new value, optionally calling
    /// a provided action and an optional flag whether to change the IsDirty property
    /// </summary>
    /// <typeparam name="TResult">The type of the property and backing store</typeparam>
    /// <param name="field">Backing field</param>
    /// <param name="newValue">New value to assign to property</param>
    /// <param name="setAction">Optional action to invoke after property is set</param>
    /// <param name="setIsDirty">Optional to specify if the IsDirty flag is set (default: true)</param>
    /// <param name="shouldValidate">Optional to specify if Validate() should be called after the backing store is set</param>
    /// <param name="propertyName">Name of this property (for internal use, do not assign)</param>
    /// <returns>Boolean indicating whether the property was set with a different value</returns>
    protected virtual bool Set<TResult>(ref TResult field, TResult newValue, Action<TResult> setAction = null, bool setIsDirty = true, bool shouldValidate = false, [CallerMemberName] string propertyName = null) {
        var wasSet = base.Set<TResult>(ref field, newValue, setAction, setIsDirty, propertyName);
        if ( !isInitializing && shouldValidate )
            Validate();
        return wasSet;
    }

    /// <summary>
    /// A <see langword="sealed"/> override of the base class method. This SHOULD NOT BE INVOKED as it
    /// simply calls the <see langword="virtual"/> base Initialize<typeparamref name="T"/> method.
    /// </summary>
    [Obsolete("This method is defined to prevent propogation of the base class Initialize(). It should not be used.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public sealed override void Initialize() => Initialize(default(T));
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
}


/// <summary>
/// Base class for modal View Model classes, implementing a <![CDATA[DialogManager<T>]]>
/// </summary>
/// <typeparam name="T">Type for the Source property</typeparam>
public abstract class ModalViewModelBase<T> : ViewModelBase<T>, IDialogSupport<T>, IModalViewModelBase<T>
{
    public ModalViewModelBase() {
        DialogManager = new(this);
    }

    public DialogManager<T> DialogManager { get; init; }
}
