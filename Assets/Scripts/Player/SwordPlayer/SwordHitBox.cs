using UnityEngine;

public class SwordHitBox : MonoBehaviour
{
    private SwordPlayerController swordPlayerController;

    [SerializeField] private GameObject swordWavePrefab; // Assign this in the inspector with the prefab for the sword wave projectile

    private void Awake()
    {
        swordPlayerController = FindFirstObjectByType<SwordPlayerController>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.position = swordPlayerController.transform.position + swordPlayerController.transform.forward * 2.5f + swordPlayerController.transform.up * 2.5f; // Position the hitbox in front of the player and in the middle of the player's height
        gameObject.SetActive(false); // Start with the hitbox disabled
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = swordPlayerController.transform.position + swordPlayerController.transform.forward * 2.5f + swordPlayerController.transform.up * 2.5f; // Keep the hitbox in front of the player and in the middle of the player's height
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("SwordHitBox triggered by " + collider.name);
        swordPlayerController.OnSwordHitBoxTriggerEnter(collider);
    }

    public void ShootSwordWave()
    {
        // Implement logic to shoot a sword wave projectile that can hit multiple enemies in the same lane
        Debug.Log("Shooting sword wave!");
        GameObject swordWave = Instantiate(swordWavePrefab, transform.position, transform.rotation);
    }
}
