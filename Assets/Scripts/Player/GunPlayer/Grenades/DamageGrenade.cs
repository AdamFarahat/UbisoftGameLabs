using UnityEngine;

public class DamageGrenade : GrenadeLauncher
{
    public override void Throw()
    {
        if (!PreThrow())
            return;

        Debug.Log("Throw damage grenade!");
        InstantiateGrenade();
    }
}
