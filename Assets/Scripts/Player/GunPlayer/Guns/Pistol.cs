using FMODUnity;
using UnityEngine;

public class Pistol : Gun
{
    public override void StartFiring()
    {
        if (!PreStartFiring())
            return;

        Debug.Log("StartFiring pistol!");
        InstantiateBullet();

        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerPistolShot, transform.position);
    }
}
