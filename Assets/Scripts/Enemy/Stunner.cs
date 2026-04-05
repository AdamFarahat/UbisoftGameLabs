using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stunner : MonoBehaviour
{
    public float stunTime = 1f;
    public UnityAction OnStun;
    private readonly HashSet<PlayerController> playersStunned = new();

    private void Awake()
    {
        if (this.TryGetComponentInHierarchy(out Enemy enemy))
            enemy.OnTakeFromPool += ResetState;
    }

    public void ResetState()
    {
        playersStunned.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null && !playersStunned.Contains(player))
        {
            player.Stun(stunTime);
            OnStun?.Invoke();
            playersStunned.Add(player);
        }
    }
}
