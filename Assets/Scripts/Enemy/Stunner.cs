using UnityEngine;

public class Stunner : MonoBehaviour
{
    public float stunTime = 1f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            player.Stun(stunTime);
    }
}
