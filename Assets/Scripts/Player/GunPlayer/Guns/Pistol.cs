using UnityEngine;

public class Pistol : Gun
{
    public override void Fire()
    {
        if (!PreFire())
            return;

        Debug.Log("Fire pistol!");
        InstantiateBullet();
    }
}
