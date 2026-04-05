using System.Runtime.InteropServices;
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    #region PLAYER
    [field: Header("PLAYER")]
    [field: SerializeField] public EventReference PlayerStunned { get; private set; }

    [field: SerializeField] public EventReference PlayerDash { get; private set; }

    #endregion
    
    #region GUNPLAYER
    [field: Header("GUNPLAYER")]
    [field: SerializeField] public EventReference PlayerRevolverShot { get; private set; }
    [field: SerializeField] public EventReference PlayerRevolverAltShot { get; private set; }
    [field: SerializeField] public EventReference PlayerRevolverImpact { get; private set; }
    [field: SerializeField] public EventReference PlayerRevolverNotReady { get; private set; }
    [field: SerializeField] public EventReference PlayerRevolverSelect { get; private set; }
    [field: SerializeField] public EventReference PlayerShotgunShot { get; private set; }
    [field: SerializeField] public EventReference PlayerShotgunSelect { get; private set; }
    [field: SerializeField] public EventReference PlayerMachinegunShot { get; private set; }
    [field: SerializeField] public EventReference PlayerMachinegunOverheat { get; private set; }
    [field: SerializeField] public EventReference PlayerMachinegunSelect { get; private set; }
    [field: SerializeField] public EventReference PlayerGrenadeThrow { get; private set; }
    [field: SerializeField] public EventReference PlayerGrenadeExplode { get; private set; }


    #endregion

    #region SWORDPLAYER
    [field: Header("SWORDPLAYER")]

    [field: SerializeField] public EventReference PlayerSwordSlash { get; private set; }

    [field: SerializeField] public EventReference PlayerSwordHit { get; private set; }

    [field: SerializeField] public EventReference PlayerSwordParry { get; private set; }

    [field: SerializeField] public EventReference PlayerJump { get; private set; }

    #endregion

    #region ENEMIES
    [field: Header("GENERAL ENEMIES")]
    [field: SerializeField] public EventReference EnemyWeaponShot { get; private set; }

    [field: SerializeField] public EventReference EnemyHurt { get; private set; }
    [field: SerializeField] public EventReference EnemySpawn { get; private set; }
    [field: SerializeField] public EventReference EnemyDeath { get; private set; }
    [field: SerializeField] public EventReference EnemyShieldBroken { get; private set; }
    [field: SerializeField] public EventReference EnemyShieldHit { get; private set; }

    [field: Header("SAMURAI ENEMY")]
    [field: SerializeField] public EventReference SamuraiSwordSlash { get; private set; }
    [field: SerializeField] public EventReference SamuraiSwordParry { get; private set; }
    [field: SerializeField] public EventReference SamuraiStunned { get; private set; }

    [field: Header("Kamikaze Enemy")]
    [field: SerializeField] public EventReference KamikazeExplode { get; private set; }

    #endregion

    #region UI
    [field: Header("UI")]
    [field: SerializeField] public EventReference UIHover { get; private set; }
    [field: SerializeField] public EventReference UIPress { get; private set; }
    [field: SerializeField] public EventReference UICancel { get; private set; }
    [field: SerializeField] public EventReference UICountdown { get; private set; }
    [field: SerializeField] public EventReference UITip { get; private set; }

    #endregion

    #region OST
    [field: Header("OST")]

    [field: SerializeField] public EventReference OSTGameStart { get; private set; }
    [field: SerializeField] public EventReference OSTGame { get; private set; }
    [field: SerializeField] public EventReference OSTGameUlt { get; private set; }
    [field: SerializeField] public EventReference OSTMenu { get; private set; }

    [field: SerializeField] public EventReference OSTutorial { get; private set; }
    [field: SerializeField] public EventReference OSTGameOver { get; private set; }

    #endregion

    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FMODEvents in the scene.");
        }

        Instance = this;
    }
}
