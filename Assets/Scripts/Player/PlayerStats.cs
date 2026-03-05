using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private static PlayerStats instance = null;
    public static PlayerStats Instance => instance;
    
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
    [SerializeField] private float superDuration = 5f;
    private Coroutine awaitingSuperCoroutine = null;
    [SerializeField] private float activateSuperWaitTime = 0.1f;
    private bool gunSuperPrepared = false;
    private bool swordSuperPrepared = false;

    public void Awake()
    {
        instance = this;
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
        if(awaitingSuperCoroutine == null)
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
        currentHealth -= damage;
        healthPercent = currentHealth / maxHealth;
    }

    public void AddGunSuper(float amount)
    {
        currentGunSuper += amount;
        currentGunSuper = Mathf.Clamp(currentGunSuper, 0f, statDenominator);
        gunSuperPercent = currentGunSuper / statDenominator;
    }

    public void AddSwordSuper(float amount)
    {
        currentSwordSuper += amount;
        currentSwordSuper = Mathf.Clamp(currentSwordSuper, 0f, statDenominator);
        swordSuperPercent = currentSwordSuper / statDenominator;
    }

    public void ActivateSuper()
    {
        isSuperActive = true;
        superCoroutine = StartCoroutine(SuperDuration());
        if(awaitingSuperCoroutine != null)
        {
            StopCoroutine(awaitingSuperCoroutine);
            awaitingSuperCoroutine = null;
        }
    }

    private IEnumerator AwaitingSuper()
    {
        float timer = 0;
        Debug.Log("Started Awaiting Super Coroutine!");
        while(timer < activateSuperWaitTime)
        {
            timer += Time.deltaTime;
            Debug.Log("gunSuperPrepared: "+gunSuperPrepared+", swordSuperPrepared: "+swordSuperPrepared+", !IsSuperActive: +!"+!IsSuperActive());
            if(gunSuperPrepared && swordSuperPrepared && !isSuperActive)
            {
                Debug.Log("Activating Super from Awaiting Coroutine!");
                ActivateSuper();
                yield break;
            }
        }
        awaitingSuperCoroutine = null;
    }

    private IEnumerator SuperDuration()
    {
        float timer = superDuration;
        while(timer >= 0)
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
