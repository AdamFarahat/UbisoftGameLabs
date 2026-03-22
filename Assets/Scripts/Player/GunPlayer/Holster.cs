using UnityEngine;
using UnityEngine.Assertions;

public class Holster : MonoBehaviour
{
    private Gun[] guns;
    public int NumberOfGuns => guns.Length;

    private int activeGunIndex = 0;
    public int ActiveGunIndex => activeGunIndex;

    private GunAnimationManager animator;

    private void Awake()
    {
        guns = GetComponents<Gun>();
        Assert.IsTrue(guns.Length > 0);

        animator = GetComponentInParent<GunAnimationManager>();
        Assert.IsNotNull(animator);
    }

    public void StartFiring()
    {
        guns[activeGunIndex].StartFiring();
    }

    public void KeepFiring()
    {
        guns[activeGunIndex].KeepFiring();
    }

    public void StopFiring()
    {
        guns[activeGunIndex].StopFiring();
    }

    public void CancelFiring()
    {
        guns[activeGunIndex].CancelFiring();
    }

    public void ToggleUp()
    {
        activeGunIndex = (activeGunIndex + 1) % guns.Length;
        animator.GunIndex = activeGunIndex;
        switch (activeGunIndex)
        {
            case 0:
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerRevolverSelect, transform.position);
                break;
            case 1:
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerShotgunSelect, transform.position);
                break;
            case 2:
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerMachinegunSelect, transform.position);
                break;
        }
    }

    public void ToggleDown()
    {
        activeGunIndex = (activeGunIndex - 1 + guns.Length) % guns.Length;
        animator.GunIndex = activeGunIndex;
        switch (activeGunIndex)
        {
            case 0:
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerRevolverSelect, transform.position);
                break;
            case 1:
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerShotgunSelect, transform.position);
                break;
            case 2:
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerMachinegunSelect, transform.position);
                break;
        }
    }
}
