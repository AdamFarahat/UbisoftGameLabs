using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    private static PlayerStats instance = null;
    public static PlayerStats Instance => instance;
    private UIManager uiManager;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private float currentGunSuper;
    private float currentSwordSuper;
    private float healthPercent;
    private float statDenominator = 100f;

    [Header("Super")]
    private float gunSuperPercent;
    private float swordSuperPercent;
    private bool isSuperActive = false;
    Coroutine superCoroutine = null;
    public float superDuration = 5f;
    private Coroutine awaitingSuperCoroutine = null;
    [SerializeField] private float activateSuperWaitTime = 0.1f;
    private bool gunSuperPrepared = false;
    private bool swordSuperPrepared = false;

    // Begin tutorial settings
    public bool damageEnabled = true;
    public bool superEnabled = true;
    public UnityAction SuperStarted;
    public UnityAction SuperEnded;
    // End tutorial settings

    public void Awake()
    {
        instance = this;
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
            Debug.LogWarning("UIManager instance not found. Player stats will not be shown.");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        currentGunSuper = 0f;
        currentSwordSuper = 0f;
        healthPercent = currentHealth / maxHealth;
        gunSuperPercent = currentGunSuper / statDenominator;
        swordSuperPercent = currentSwordSuper / statDenominator;
        Debug.Log("Player health initialized to: " + currentHealth);

        currentGunSuper = 100f;
        gunSuperPercent = currentGunSuper / statDenominator;
        currentSwordSuper = 100f;
        swordSuperPercent = currentSwordSuper / statDenominator;
    }

    public float GetHealthPercentage()
    {
        return healthPercent;
    }

    public float GetGunSuperPercent()
    {
        return gunSuperPercent;
    }

    public float GetSwordSuperPercent()
    {
        return swordSuperPercent;
    }

    public bool IsSuperActive()
    {
        return isSuperActive;
    }

    public void PrepareGunSuperReady(bool isReady)
    {
        gunSuperPrepared = isReady;
        Debug.Log("Gun Super Prepared: " + gunSuperPrepared);
        if (awaitingSuperCoroutine == null)
        {
            awaitingSuperCoroutine = StartCoroutine(AwaitingSuper());
        }
    }

    public void PrepareSwordSuperReady(bool isReady)
    {
        swordSuperPrepared = isReady;
        Debug.Log("Sword Super Prepared: " + swordSuperPrepared);
        if (awaitingSuperCoroutine == null)
        {
            awaitingSuperCoroutine = StartCoroutine(AwaitingSuper());
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsSuperActive())
        {
            return;
        }

        currentHealth -= damage;
        healthPercent = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthPercent = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        // Pause the game, disable the characters, show game over screen (UIManager.Instance.ShowGameOverScreen()).
        Time.timeScale = 0f;
        GameObject.Find("GunPlayer").SetActive(false);
        GameObject.Find("SwordPlayer").SetActive(false);
        uiManager.ShowGameOverScreen();

    }

    public void ResetGunSuper()
    {
        currentGunSuper = 0f;
        gunSuperPercent = 0f;
    }

    public void FillGunSuper(float duration)
    {
        IEnumerator Routine()
        {
            currentGunSuper = 0f;
            gunSuperPercent = 0f;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                gunSuperPercent = Mathf.Clamp01(t / duration);
                currentGunSuper = gunSuperPercent * statDenominator;

                yield return null;
            }
            currentGunSuper = statDenominator;
            gunSuperPercent = 1f;
        }

        StartCoroutine(Routine());
    }

    public void AddGunSuper(float amount)
    {
        if (!superEnabled)
            return;

        if (IsSuperActive())
            return;

        currentGunSuper += amount;
        currentGunSuper = Mathf.Clamp(currentGunSuper, 0f, statDenominator);
        gunSuperPercent = currentGunSuper / statDenominator;
    }

    public void ResetSwordSuper()
    {
        currentSwordSuper = 0f;
        swordSuperPercent = 0f;
    }

    public void FillSwordSuper(float duration)
    {
        IEnumerator Routine()
        {
            currentSwordSuper = 0f;
            swordSuperPercent = 0f;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                swordSuperPercent = Mathf.Clamp01(t / duration);
                currentSwordSuper = swordSuperPercent * statDenominator;

                yield return null;
            }
            currentSwordSuper = statDenominator;
            swordSuperPercent = 1f;
        }

        StartCoroutine(Routine());
    }

    public void AddSwordSuper(float amount)
    {
        if (!superEnabled)
            return;

        if (IsSuperActive())
            return;

        currentSwordSuper += amount;
        currentSwordSuper = Mathf.Clamp(currentSwordSuper, 0f, statDenominator);
        swordSuperPercent = currentSwordSuper / statDenominator;
    }

    [ContextMenu("Test Activate Super")]
    public void ActivateSuper()
    {
        isSuperActive = true;
        superCoroutine = StartCoroutine(SuperDuration());
        SuperStarted?.Invoke();
        if (awaitingSuperCoroutine != null)
        {
            StopCoroutine(awaitingSuperCoroutine);
            awaitingSuperCoroutine = null;
        }
    }

    private IEnumerator AwaitingSuper()
    {
        float timer = 0.0f;
        Debug.Log("Started Awaiting Super Coroutine!");
        Debug.Log("timer: " + timer + ", activateSuperWaitTime: " + activateSuperWaitTime);
        while (timer < activateSuperWaitTime)
        {
            timer += Time.deltaTime;
            Debug.Log("gunSuperPrepared: " + gunSuperPrepared + ", swordSuperPrepared: " + swordSuperPrepared + ", !IsSuperActive: " + !IsSuperActive());
            if (gunSuperPrepared && swordSuperPrepared && !isSuperActive)
            {
                Debug.Log("Activating Super from Awaiting Coroutine!");
                ActivateSuper();
                yield break;
            }

            yield return null;
        }
        Debug.Log("Finished Awaiting Super Coroutine without activating super.");
        swordSuperPrepared = false;
        gunSuperPrepared = false;
        awaitingSuperCoroutine = null;
    }

    private IEnumerator SuperDuration()
    {
        float timer = superDuration;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            //Show the bars going down over time
            currentGunSuper -= (statDenominator / superDuration) * Time.deltaTime;
            currentSwordSuper -= (statDenominator / superDuration) * Time.deltaTime;
            gunSuperPercent = currentGunSuper / statDenominator;
            swordSuperPercent = currentSwordSuper / statDenominator;
            yield return null;
        }
        isSuperActive = false;
        gunSuperPrepared = false;
        swordSuperPrepared = false;
        superCoroutine = null;

        SuperEnded?.Invoke();
    }
}
