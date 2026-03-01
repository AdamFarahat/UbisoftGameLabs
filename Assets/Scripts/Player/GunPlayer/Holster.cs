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

    public void StartFiring(GunPlayerController gunPlayerController)
    {
        guns[activeGunIndex].StartFiring(gunPlayerController);
    }

    public void KeepFiring(GunPlayerController gunPlayerController)
    {
        guns[activeGunIndex].KeepFiring(gunPlayerController);
    }

    public void StopFiring()
    {
        guns[activeGunIndex].StopFiring();
    }

    public void ToggleUp()
    {
        activeGunIndex = (activeGunIndex + 1) % guns.Length;
        Debug.Log("Switch to " + guns[activeGunIndex]);
    }

    public void ToggleDown()
    {
        activeGunIndex = (activeGunIndex - 1 + guns.Length) % guns.Length;
        Debug.Log("Switch to " + guns[activeGunIndex]);
    }
}
