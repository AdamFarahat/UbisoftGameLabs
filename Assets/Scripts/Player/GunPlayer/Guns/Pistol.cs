using UnityEngine;

public class Pistol : Gun
{
    public override void FirePrimary()
    {
        if (!PreFire())
            return;

        Debug.Log("Fire pistol!");
        InstantiateBullet();
    }
}
