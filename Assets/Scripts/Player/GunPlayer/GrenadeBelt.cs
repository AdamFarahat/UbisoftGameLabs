using UnityEngine;
using UnityEngine.Assertions;

public class GrenadeBelt : MonoBehaviour
{
    private GrenadeLauncher[] launchers;
    private int activeLauncherIndex = 0;

    private void Awake()
    {
        launchers = GetComponents<GrenadeLauncher>();
        Assert.IsTrue(launchers.Length > 0);
    }

    public void Throw()
    {
        launchers[activeLauncherIndex].Throw();
    }

    public void HoldInput()
    {
        launchers[activeLauncherIndex].HoldInput();
    }

    public void ReleaseInput()
    {
        launchers[activeLauncherIndex].ReleaseInput();
    }

    public void Toggle()
    {
        activeLauncherIndex = (activeLauncherIndex + 1) % launchers.Length;
        Debug.Log("Switch to " + launchers[activeLauncherIndex]);
    }
}
