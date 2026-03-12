using UnityEngine;

public static class ComponentExtensions
{
    public static T GetComponentInHierarchy<T>(this Component component) where T : Component
    {
        if (component.TryGetComponent(out T self))
            return self;

        T parent = component.GetComponentInParent<T>();
        if (parent != null)
            return parent;

        return component.GetComponentInChildren<T>();
    }

    public static bool TryGetComponentInHierarchy<T>(this Component component, out T result) where T : Component
    {
        if (component.TryGetComponent(out result))
            return true;

        result = component.GetComponentInParent<T>();
        if (result != null)
            return true;

        result = component.GetComponentInChildren<T>();
        return result != null;
    }
}
