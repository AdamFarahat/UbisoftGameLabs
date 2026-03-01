using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun")]
    [SerializeField] private float spreadAngle = 45f;

    public override void StartFiring()
    {
        if (!PreStartFiring())
            return;

        Debug.Log("StartFiring shotgun!");
        InstantiateShotgunBlast().coneAngle = spreadAngle;
    }
}
