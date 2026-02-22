using UnityEngine;

public class ObjectsManager : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        other.gameObject.SetActive(false);
    }
}
