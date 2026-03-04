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
}
