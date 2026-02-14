using UnityEngine;

public class Shotgun : MonoBehaviour, IGun
{
    public void Fire()
    {
        Debug.Log("Fire shotgun!");
    }
}
