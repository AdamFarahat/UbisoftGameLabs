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
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerRevolverSelect, transform.position);
                break;
            case 1:
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerShotgunSelect, transform.position);
                break;
            case 2:
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerMachinegunSelect, transform.position);
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
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerRevolverSelect, transform.position);
                break;
            case 1:
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerShotgunSelect, transform.position);
                break;
            case 2:
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerMachinegunSelect, transform.position);
                break;
        }
    }
}
