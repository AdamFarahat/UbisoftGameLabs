using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    private SwordPlayerController swordPlayerController;

    private void Awake()
    {
        swordPlayerController = GetComponentInParent<SwordPlayerController>();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("ColliderCheck Trigger Entered with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Ground"))
        {
            swordPlayerController.IsGrounded = true;
        }
    }
}

