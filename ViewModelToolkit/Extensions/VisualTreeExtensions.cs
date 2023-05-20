namespace ViewModelToolkit;

public static class VisualTreeExtensions
{
    public static ContentPage FindContentPage(this Element view) {
        Element parent = view?.Parent;
        while ( parent is not null )
            if ( parent is ContentPage page )
                return page;
            else
                return FindContentPage(parent);
        return null;
    }

    public static T FindFirstDescendent<T>(this Element element) where T : IElement {
        return FindDescendents<T>(element).FirstOrDefault();
    }

    public static IList<T> FindDescendents<T>(this Element element) where T : IElement {
        return element
            .GetVisualTreeDescendants()
            .Where(d => d is T)
            .Select(d => (T)d)
            .ToList();
    }
}
