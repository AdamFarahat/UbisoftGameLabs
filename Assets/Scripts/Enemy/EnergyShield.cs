using UnityEngine;

public class EnergyShield : MonoBehaviour
{
    [SerializeField] private float probabilityToSpawn = 0.05f;

    private void Awake()
    {
        Enemy enemy = GetComponentInParent<Enemy>();
        if (enemy != null)
            enemy.OnTakeFromPool += TakeFromPool;
    }

    private void TakeFromPool()
    {
        gameObject.SetActive(Random.value < probabilityToSpawn);
    }
}
