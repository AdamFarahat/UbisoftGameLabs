using System.Collections.Generic;
using UnityEngine;

public class SearchCollider : MonoBehaviour
{

    public HashSet<GameObject> players = new HashSet<GameObject>();
    void OnTriggerEnter(Collider other) {
        //to be changed with playerController
        Debug.Log("enter");
        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController != null) {
            Debug.Log("enter 2");

            players.Add(playerController.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
        //to be changed with playerController Component
        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController != null)
        {
            players.Remove(playerController.gameObject);
        }
    }
}
