using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    #region GUNPLAYER
    [field: Header("GUNPLAYER")]
    [field: SerializeField] public EventReference playerRevolverShot { get; private set; }
    [field: SerializeField] public EventReference playerRevolverAltShot { get; private set; }
    [field: SerializeField] public EventReference playerRevolverImpact { get; private set; }
    [field: SerializeField] public EventReference playerRevolverNotReady { get; private set; }
    [field: SerializeField] public EventReference playerRevolverSelect { get; private set; }
    [field: SerializeField] public EventReference playerShotgunShot { get; private set; }
    [field: SerializeField] public EventReference playerShotgunSelect { get; private set; }
    [field: SerializeField] public EventReference playerMachinegunShot { get; private set; }
    [field: SerializeField] public EventReference playerMachinegunOverheat { get; private set; }
    [field: SerializeField] public EventReference playerMachinegunSelect { get; private set; }
    [field: SerializeField] public EventReference playerGrenadeThrow { get; private set; }
    [field: SerializeField] public EventReference playerGrenadeExplode { get; private set; }
    

    #endregion

    #region SWORDPLAYER
    [field: Header("SWORDPLAYER")]

    [field: SerializeField] public EventReference playerSwordSlash { get; private set; }

    [field: SerializeField] public EventReference playerSwordHit { get; private set; }

    [field: SerializeField] public EventReference playerSwordParry { get; private set; }
    
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
