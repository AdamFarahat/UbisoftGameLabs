using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    private SwordPlayerController playerController;

    void Awake()
    {
        playerController = GetComponentInParent<SwordPlayerController>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(playerController.IsAttacking())
            {
                other.GetComponentInParent<DemoEnemy>().Death();
            }
        }
    }
}
