using System.Collections;
using UnityEngine;

public class HologramText : MonoBehaviour
{
    public IEnumerator SpawnRoutine()
    {
        // TODO flicker on
        gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator DespawnRoutine()
    {
        // TODO flicker off
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
    }
}
