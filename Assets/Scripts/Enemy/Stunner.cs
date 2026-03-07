using UnityEngine;
using UnityEngine.Events;

public class Stunner : MonoBehaviour
{
    [SerializeField] private float stunTime = 1f;
    public UnityAction OnStun;

    public float StunTime => stunTime;

    public void SetStunTime(float time)
    {
        stunTime = time;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            player.Stun(stunTime);
            OnStun?.Invoke();
        }
    }
}
