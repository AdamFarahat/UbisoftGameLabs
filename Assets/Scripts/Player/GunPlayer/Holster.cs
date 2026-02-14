using UnityEngine;
using UnityEngine.Assertions;

public class Holster : MonoBehaviour
{
    private Gun[] guns;
    private int activeGunIndex = 0;

    private void Awake()
    {
        guns = GetComponents<Gun>();
        Assert.IsTrue(guns.Length > 0);
    }

    public void FirePrimary()
    {
        guns[activeGunIndex].FirePrimary();
    }

    public void HoldPrimary()
    {
        guns[activeGunIndex].HoldPrimary();
    }

    public void ReleasePrimary()
    {
        guns[activeGunIndex].ReleasePrimary();
    }

    public void ToggleUp()
    {
        activeGunIndex = (activeGunIndex + 1) % guns.Length;
        Debug.Log("Switch to " + guns[activeGunIndex]);
    }
}
