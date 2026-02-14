using UnityEngine;

public class MachineGun : Gun
{
    public override bool Fire()
    {
        if (base.Fire())
        {
            Debug.Log("Fire machine gun!");
            return true;
        }
        return false;
    }
}
