using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    private float healthPercent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthPercent = currentHealth / maxHealth;
        Debug.Log("Player health initialized to: " + currentHealth);
    }

    public float GetHealthPercentage()
    {
        return healthPercent;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthPercent = currentHealth / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
