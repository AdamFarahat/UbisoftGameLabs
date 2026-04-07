using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialSwitchGuns : TutorialBase
{
    [Header("General")]
    [SerializeField] private GameObject holdForLaserGruntsRoot;
    [SerializeField] private GameObject holdToBreakShieldGruntsRoot;
    [SerializeField] private GameObject useShotgunGruntsRoot;

    [Header("Descriptions")]
    [SerializeField] private HologramText holdForLaserText;
    [SerializeField] private HologramText downForMachineGunText;
    [SerializeField] private HologramText holdToBreakShieldText;
    [SerializeField] private HologramText downForShotgunText;
    [SerializeField] private HologramText holdToHitMoreEnemiesText;
    [SerializeField] private HologramText generalToggleText;

    enum State
    {
        HoldForLaser,
        DownForMachineGun,
        HoldToBreakShield,
        DownForShotgun,
        HoldToHitMoreEnemies,
        GeneralToggle
    }

    private State state = State.HoldForLaser;

    private Enemy[] laserEnemies;
    private Enemy[] machineGunEnemies;
    private Enemy[] shotgunEnemies;

    private GunPlayerController gunPlayer;
    
    private bool switchedToMachineGun = false;
    private bool switchedToShotgun = false;
    private bool switchedAgain = false;

    protected override void Awake()
    {
        base.Awake();
        
        Assert.IsNotNull(holdForLaserGruntsRoot);
        Assert.IsNotNull(holdToBreakShieldGruntsRoot);
        Assert.IsNotNull(useShotgunGruntsRoot);

        Assert.IsNotNull(holdForLaserText);
        Assert.IsNotNull(downForMachineGunText);
        Assert.IsNotNull(holdToBreakShieldText);
        Assert.IsNotNull(downForShotgunText);
        Assert.IsNotNull(holdToHitMoreEnemiesText);
        Assert.IsNotNull(generalToggleText);

        downForMachineGunText.gameObject.SetActive(false);
        holdToBreakShieldText.gameObject.SetActive(false);
        downForShotgunText.gameObject.SetActive(false);
        holdToHitMoreEnemiesText.gameObject.SetActive(false);
        generalToggleText.gameObject.SetActive(false);

        holdForLaserGruntsRoot.SetActive(false);
        laserEnemies = holdForLaserGruntsRoot.GetComponentsInChildren<Enemy>();

        holdToBreakShieldGruntsRoot.SetActive(false);
        machineGunEnemies = holdToBreakShieldGruntsRoot.GetComponentsInChildren<Enemy>();

        useShotgunGruntsRoot.SetActive(false);
        shotgunEnemies = useShotgunGruntsRoot.GetComponentsInChildren<Enemy>();
    }

    protected override void StartTutorial()
    {
        gunPlayer = GunPlayerController.Instance;

        if (gunPlayer == null)
        {
            EndTutorial();
            return;
        }

        gunPlayer.PressedToggleDown += PressedToggleDown;
        gunPlayer.PressedToggleUp += PressedToggleUp;

        float age = Time.time;
        IEnumerator Routine()
        {
            // HoldForLaser
            state = State.HoldForLaser;
            holdForLaserGruntsRoot.SetActive(true);
            yield return new WaitUntil(() => laserEnemies.All(g => g == null || g.Dead));
            yield return holdForLaserText.DespawnRoutine();

            // DownForMachine
            state = State.DownForMachineGun;
            yield return downForMachineGunText.SpawnRoutine();
            gunPlayer.toggleGunDownEnabled = true;
            yield return new WaitUntil(() => switchedToMachineGun);
            yield return downForMachineGunText.DespawnRoutine();

            // HoldToBreakShield
            state = State.HoldToBreakShield;
            yield return holdToBreakShieldText.SpawnRoutine();
            holdToBreakShieldGruntsRoot.SetActive(true);
            yield return new WaitUntil(() => machineGunEnemies.All(g => g == null || g.Dead));
            yield return holdToBreakShieldText.DespawnRoutine();

            // DownForShotgun
            state = State.DownForShotgun;
            yield return downForShotgunText.SpawnRoutine();
            gunPlayer.toggleGunDownEnabled = true;
            yield return new WaitUntil(() => switchedToShotgun);
            yield return downForShotgunText.DespawnRoutine();

            // HoldToHitMoreEnemies
            state = State.HoldToHitMoreEnemies;
            yield return holdToHitMoreEnemiesText.SpawnRoutine();
            useShotgunGruntsRoot.SetActive(true);
            yield return new WaitUntil(() => shotgunEnemies.All(g => g == null || g.Dead));
            yield return holdToHitMoreEnemiesText.DespawnRoutine();

            // GeneralToggle
            state = State.GeneralToggle;
            yield return generalToggleText.SpawnRoutine();
            gunPlayer.toggleGunUpEnabled = true;
            gunPlayer.toggleGunDownEnabled = true;
            yield return new WaitUntil(() => switchedAgain);

            EndTutorial();
        }

        StartCoroutine(Routine());
    }

    private void PressedToggleDown()
    {
        if (state == State.DownForMachineGun)
        {
            gunPlayer.toggleGunDownEnabled = false;
            switchedToMachineGun = true;
        }
        else if (state == State.DownForShotgun)
        {
            gunPlayer.toggleGunDownEnabled = false;
            switchedToShotgun = true;
        }
        else if (state == State.GeneralToggle)
        {
            switchedAgain = true;
        }
    }

    private void PressedToggleUp()
    {
        if (state == State.GeneralToggle)
            switchedAgain = true;
    }
}
