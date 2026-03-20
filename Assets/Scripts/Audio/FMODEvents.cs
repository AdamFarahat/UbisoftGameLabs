using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    #region PLAYER
    [field: Header("PLAYER")]

    // ---------- GRENADE ----------
    [field: SerializeField] public EventReference playerGrenadeExplode { get; private set; }
    [field: SerializeField] public EventReference playerGrenadeThrow { get; private set; }

    // ---------- REVOLVER ----------
    [field: SerializeField] public EventReference playerRevolverBulletImpact { get; private set; }
    [field: SerializeField] public EventReference playerRevolverNotReady { get; private set; }
    [field: SerializeField] public EventReference playerRevolverReady { get; private set; }
    [field: SerializeField] public EventReference playerRevolverReload { get; private set; }
    [field: SerializeField] public EventReference playerRevolverSelect { get; private set; }
    [field: SerializeField] public EventReference playerRevolverShot { get; private set; }

    // ---------- RIFLE ----------
    [field: SerializeField] public EventReference playerRifleOverheat { get; private set; }
    [field: SerializeField] public EventReference playerRifleSelect { get; private set; }
    [field: SerializeField] public EventReference playerRifleShot { get; private set; }

    // ---------- SHOTGUN ----------
    [field: SerializeField] public EventReference playerShotgunShot { get; private set; }
    [field: SerializeField] public EventReference playerShotgunNotReady { get; private set; }
    [field: SerializeField] public EventReference playerShotgunReload { get; private set; }
    [field: SerializeField] public EventReference playerShotgunSelect { get; private set; }

    // ---------- SWORD ----------
    [field: SerializeField] public EventReference playerSwordHit { get; private set; }
    [field: SerializeField] public EventReference playerSwordParry { get; private set; }
    [field: SerializeField] public EventReference playerSwordSlash { get; private set; }

    #endregion

    #region ENEMIES

    [field: Header("ENEMIES")]
    [field: SerializeField] public EventReference enemyWeaponShot { get; private set; }

    #endregion

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMODEvents in the scene.");
        }

        instance = this;
    }
}
