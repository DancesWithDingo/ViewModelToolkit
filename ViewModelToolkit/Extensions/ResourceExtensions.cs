using System.Diagnostics;

namespace ViewModelToolkit;

public static class ResourceExtensions
{
#pragma warning disable CA1841 // Prefer Dictionary.Contains methods

    [DebuggerNonUserCode()]
    public static T FindResource<T>(this ResourceDictionary dictionary, string key) {
        // Hack because ResourceDictionary.ContainsKey is not working right now
        //if ( dictionary.ContainsKey(key) )
        if ( dictionary.Keys.Contains(key) )
            return (T)dictionary[key];

        foreach ( var dict in dictionary.MergedDictionaries ) {
            // Hack because ResourceDictionary.ContainsKey is not working right now
            //if ( dictionary.ContainsKey(key) )
            if ( dict.Keys.Contains(key) )
                return (T)dict[key];

            if ( dict.MergedDictionaries.Any() )
                return FindResource<T>(dict, key);
        }
        Debug.WriteLine($"No resource was found named [{key}]");
        return default;
    }
#pragma warning restore CA1841 // Prefer Dictionary.Contains methods
}
