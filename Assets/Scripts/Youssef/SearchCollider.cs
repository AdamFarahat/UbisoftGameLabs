using System.Collections.Generic;
using UnityEngine;

public class SearchCollider : MonoBehaviour
{

    public HashSet<GameObject> players = new HashSet<GameObject>();
    void OnTriggerEnter(Collider other) {
        //to be changed with playerController
        Collider playerController = other.GetComponent<Collider>();
        if (playerController != null) {
            players.Add(playerController.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        //to be changed with playerController Component
        Collider playerController = other.GetComponent<Collider>();
        if (playerController != null)
        {
            players.Remove(playerController.gameObject);
        }
    }
}
