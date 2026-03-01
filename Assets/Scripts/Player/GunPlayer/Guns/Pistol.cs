using UnityEngine;

public class Pistol : Gun
{
    public override void StartFiring(GunPlayerController gunPlayerController)
    {
        if (!PreStartFiring())
            return;

        Debug.Log("StartFiring pistol!");
        InstantiateBullet(gunPlayerController);
    }
}
