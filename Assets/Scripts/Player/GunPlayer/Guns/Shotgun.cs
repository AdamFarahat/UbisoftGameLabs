using UnityEngine;

public class Shotgun : Gun
{
    public override bool Fire()
    {
        if (base.Fire())
        {
            Debug.Log("Fire shotgun!");
            return true;
        }
        return false;
    }
}
