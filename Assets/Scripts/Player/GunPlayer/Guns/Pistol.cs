using UnityEngine;

public class Pistol : Gun
{
    public override bool Fire()
    {
        if (base.Fire())
        {
            Debug.Log("Fire pistol!");
            return true;
        }
        return false;
    }
}
