using UnityEngine;

public class Pistol : MonoBehaviour, IGun
{
    public void Fire()
    {
        Debug.Log("Fire pistol!");
    }
}
