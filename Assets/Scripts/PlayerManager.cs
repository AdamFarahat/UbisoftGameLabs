using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    [SerializeField] private GameObject playerG,playerS;
    private void Awake()
    {
        //Setting up PlayerInputManager and instantiating players
        playerInputManager = GetComponent<PlayerInputManager>();
        playerG = Instantiate(playerG, Vector3.zero, Quaternion.identity);
        playerS = Instantiate(playerS, Vector3.zero, Quaternion.identity);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Setting up players' initial lane and color
        playerG.GetComponent<LaneBound>().LaneIndex = 1;
        playerS.GetComponent<LaneBound>().LaneIndex = 3;
        // Set different colors for the players for Debugging purposes
        playerG.GetComponentInChildren<Renderer>().material.color = Color.red;
        playerS.GetComponentInChildren<Renderer>().material.color = Color.blue;
    }
}
