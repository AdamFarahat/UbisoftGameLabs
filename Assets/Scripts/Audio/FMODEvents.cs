using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    #region PLAYER
    [field: Header("PLAYER")]
    [field: SerializeField] public EventReference playerPistolShot { get; private set; }
    [field: SerializeField] public EventReference playerShotgunShot { get; private set; }
    [field: SerializeField] public EventReference playerMachinegunShot { get; private set; }
    [field: SerializeField] public EventReference playerGrenadeThrow { get; private set; }
    [field: SerializeField] public EventReference playerGrenadeExplode { get; private set; }

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
