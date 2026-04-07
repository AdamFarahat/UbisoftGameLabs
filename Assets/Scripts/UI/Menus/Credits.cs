using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private Button firstSelected;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventSystem.SetSelectedGameObject(firstSelected.gameObject);
    }
}
