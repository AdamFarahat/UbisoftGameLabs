using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private static PlayerStats instance = null;
    public static PlayerStats Instance => instance;
    
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private float currentGunSuper;
    private float currentSwordSuper;

    private float healthPercent;
    private float gunSuperPercent;
    private float swordSuperPercent;

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
        gunSuperPercent = currentGunSuper / 100f;
        swordSuperPercent = currentSwordSuper / 100f;
        Debug.Log("Player health initialized to: " + currentHealth);
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthPercent = currentHealth / maxHealth;
    }

    public void AddGunSuper(float amount)
    {
        currentGunSuper += amount;
        currentGunSuper = Mathf.Clamp(currentGunSuper, 0f, 100f);
        gunSuperPercent = currentGunSuper / 100f;
    }

    public void AddSwordSuper(float amount)
    {
        currentSwordSuper += amount;
        currentSwordSuper = Mathf.Clamp(currentSwordSuper, 0f, 100f);
        swordSuperPercent = currentSwordSuper / 100f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
