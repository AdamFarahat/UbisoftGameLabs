using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun")]
    [SerializeField] private float spreadAngle = 5f;

    public override void FirePrimary()
    {
        if (!PreFire())
            return;

        Debug.Log("Fire shotgun!");
        InstantiateBullet();
        InstantiateBullet().transform.forward = Quaternion.Euler(0f, spreadAngle, 0f) * transform.forward;
        InstantiateBullet().transform.forward = Quaternion.Euler(0f, -spreadAngle, 0f) * transform.forward;
    }
}
