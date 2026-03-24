using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelect : MonoBehaviour
{
    public static PlayerSelect Instance { get; private set; }

    public static InputDevice gunPlayerDevice;
    public static InputDevice swordPlayerDevice;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static void OnGameEnd()
    {
        gunPlayerDevice = null;
        swordPlayerDevice = null;
        Destroy(Instance.gameObject);
    }
}
