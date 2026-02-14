using UnityEngine;

public class MachineGun : MonoBehaviour, IGun
{
    public void Fire()
    {
        Debug.Log("Fire machine gun!");
    }
}
