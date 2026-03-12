using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using NUnit.Framework;

public class PlayerSelectManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform mainCanvas;
    [SerializeField] private RectTransform[] allCharacterSlots;
    [SerializeField] private Image heartUIImage;

    [SerializeField] private GameObject dummyCursor;

    private readonly int leftAmountID = Shader.PropertyToID("_LeftAmount");
    private readonly int rightAmountID = Shader.PropertyToID("_RightAmount");

    [Header("Events")]
    public UnityEvent onReturnToMenuRequested;

    private PlayerInputManager inputManager;
    
    // The cursors check this to see if they are allowed to move or select
    public bool IsAcceptingInput { get; private set; } = false;

    void Awake()
    {
        Assert.IsNotNull(heartUIImage);
    }

    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        heartUIImage.material = new Material(heartUIImage.material);
        
        // Lock inputs immediately on start so players can't join during the animation
        if (inputManager != null)
            inputManager.DisableJoining();
        
    }

    public void EnableInput()
    {
        IsAcceptingInput = true;
        
        if (inputManager != null)
            inputManager.EnableJoining();
        
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // Disable dummy 
        if (dummyCursor != null)
        {
            dummyCursor.SetActive(false);
        }
        PlayerSelectInput newCursor = playerInput.GetComponent<PlayerSelectInput>();
        
        if (newCursor != null)
            newCursor.SetupCursor(allCharacterSlots, mainCanvas, this, playerInput.playerIndex);
    }

    public void CharacterLockedIn(int playerID, int slotIndex)
    {
        Debug.Log($"Player {playerID + 1} locked in character slot {slotIndex}!");
        // Set float based on which they locked in 
        if (slotIndex == 0)
        {
            heartUIImage.material.SetFloat(leftAmountID, 1.0f);
        }
        else if (slotIndex == 2)
        {
            heartUIImage.material.SetFloat(rightAmountID, 1.0f);
        }


    }

    public void CharacterUnlocked(int playerID, int slotIndex)
    {
        Debug.Log($"Player {playerID + 1} deselected their character in slot {slotIndex}.");
        
        // Reset the float based on which slot they unlocked
        if (slotIndex == 0)
        {
            heartUIImage.material.SetFloat(leftAmountID, 0.0f);
        }
        else if (slotIndex == 2)
        {
            heartUIImage.material.SetFloat(rightAmountID, 0.0f);
        }
    }

    public void AttemptReturnToMenu(int playerID)
    {
        Debug.Log($"Player {playerID + 1} is backing out to the Main Menu!");

        onReturnToMenuRequested?.Invoke(); 
    }
}