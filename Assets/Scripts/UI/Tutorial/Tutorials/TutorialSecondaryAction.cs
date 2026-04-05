using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

// TODO show second description only after pressing the throw/block - be more descriptive about how block cooldown works
public class TutorialSecondaryAction : TutorialBase
{
    [SerializeField] private GameObject grenadeEntitiesRoot;
    [SerializeField] private GameObject parryEntitiesRoot;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float cooldownLength = 1.5f;

    private TutorialEnemyLife[] meleeGrunts;
    private TutorialEnemyLife[] flyerGrunts;

    private bool pressedThrow = true;
    private bool pressedBlock = true;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(grenadeEntitiesRoot);
        Assert.IsNotNull(parryEntitiesRoot);

        meleeGrunts = grenadeEntitiesRoot.GetComponentsInChildren<TutorialEnemyLife>();
        flyerGrunts = parryEntitiesRoot.GetComponentsInChildren<TutorialEnemyLife>();
    }

    private void OnDisable()
    {
        foreach (TutorialEnemyLife meleeGrunt in meleeGrunts)
            if (meleeGrunt != null)
                meleeGrunt.gameObject.SetActive(false);

        foreach (TutorialEnemyLife flyerGrunt in flyerGrunts)
            if (flyerGrunt != null)
                flyerGrunt.gameObject.SetActive(false);
    }

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
        {
            gunPlayer.throwEnabled = true;
            gunPlayer.GrenadeBelt.SetThrowCooldown(cooldownLength);
            pressedThrow = false;
            gunPlayer.PressedThrow += () => { pressedThrow = true; };

            manager.GunPlayerCooldownUI.SetActive(true);
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.blockEnabled = true;
            swordPlayer.blockCooldown = cooldownLength;
            pressedBlock = false;
            swordPlayer.PressedBlock += () => { pressedBlock = true; };

            manager.SwordPlayerCooldownUI.SetActive(true);
        }

        IEnumerator Routine()
        {
            while (!pressedThrow || !pressedBlock)
                yield return null;

            if (gunPlayer != null)
            {
                foreach (TutorialEnemyLife meleeGrunt in meleeGrunts)
                {
                    meleeGrunt.fadeInDuration = fadeInDuration;
                    meleeGrunt.gameObject.SetActive(true);
                }
            }

            if (swordPlayer != null)
            {
                foreach (TutorialEnemyLife flyerGrunt in flyerGrunts)
                {
                    flyerGrunt.fadeInDuration = fadeInDuration;
                    flyerGrunt.gameObject.SetActive(true);
                }
            }

            while (meleeGrunts.Any(g => g != null && g.isActiveAndEnabled) || flyerGrunts.Any(g => g != null && g.isActiveAndEnabled))
                yield return null;

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
