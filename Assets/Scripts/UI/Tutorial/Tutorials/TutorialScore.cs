using System.Collections;
using UnityEngine;

public class TutorialScore : TutorialBase
{
    [SerializeField] private float scoreGainDuration = 6f;
    [SerializeField] private float multiplierGainPerSecond = 2f;

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
        {
            gunPlayer.SetContinuousMultiplier(1f);
            gunPlayer.ResetScore();
            manager.GunPlayerMultiplierUI.SetActive(true);
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.SetContinuousMultiplier(1f);
            swordPlayer.ResetScore();
            manager.SwordPlayerMultiplierUI.SetActive(true);
        }

        manager.ScoreUI.SetActive(true);

        IEnumerator Routine()
        {
            for (float t = 0f; t < scoreGainDuration; t += Time.deltaTime)
            {
                if (gunPlayer != null)
                {
                    gunPlayer.AddContinuousMultiplier(multiplierGainPerSecond * Time.deltaTime);
                    gunPlayer.AddScore(1);
                }

                if (swordPlayer != null)
                {
                    swordPlayer.AddContinuousMultiplier(multiplierGainPerSecond * Time.deltaTime);
                    swordPlayer.AddScore(1);
                }

                yield return null;
            }
        }

        StartCoroutine(Routine());
    }

    public override void OnStartPressed()
    {
        EndTutorial();
    }
}
