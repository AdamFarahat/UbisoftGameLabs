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

    public void Fire()
    {
        guns[activeGunIndex].Fire();
    }

    public void ToggleUp()
    {
        activeGunIndex = (activeGunIndex + 1) % guns.Length;
        Debug.Log("Switch to " + guns[activeGunIndex]);
    }
}
