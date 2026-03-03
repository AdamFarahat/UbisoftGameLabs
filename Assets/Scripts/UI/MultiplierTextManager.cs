using UnityEngine;

public class MultiplierTextManager : MonoBehaviour
{
    [SerializeField] private MultiplierText x2Text;
    [SerializeField] private MultiplierText x4Text;
    [SerializeField] private MultiplierText x6Text;
    [SerializeField] private MultiplierText x8Text;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // subscribe to multiplier activate/disactivate event
        // onActivate += EnableObject
        // onDisactivate += HideText
    }
}
