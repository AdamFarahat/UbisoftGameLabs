using UnityEngine;
using UnityEngine.Events;

public class Stunner : MonoBehaviour
{
    public float stunTime = 1f;
    public UnityAction OnStun;

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            player.Stun(stunTime);
            OnStun?.Invoke();
        }
    }
}
