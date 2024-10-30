using System.Windows;

namespace PresentationHelp.WpfViewParts;

public static class FreezableExtension
{
    public static T AsFrozen<T>(this T item) where T: Freezable
    {
        item.Freeze();
        return item;
    }
}