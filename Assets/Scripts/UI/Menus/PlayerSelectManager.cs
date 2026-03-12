using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerSelectManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform mainCanvas;
    public RectTransform[] allCharacterSlots;

    [Header("Events")]
    public UnityEvent onReturnToMenuRequested;

    private PlayerInputManager inputManager;
    
    // The cursors check this to see if they are allowed to move or select
    public bool IsAcceptingInput { get; private set; } = false;

    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        
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
        PlayerSelectInput newCursor = playerInput.GetComponent<PlayerSelectInput>();
        
        if (newCursor != null)
            newCursor.SetupCursor(allCharacterSlots, mainCanvas, this, playerInput.playerIndex);
    }

    public void CharacterLockedIn(int playerID, int slotIndex)
    {
        Debug.Log($"Player {playerID + 1} locked in character slot {slotIndex}!");
    }

    public void CharacterUnlocked(int playerID)
    {
        Debug.Log($"Player {playerID + 1} deselected their character.");
    }

    public void AttemptReturnToMenu(int playerID)
    {
        Debug.Log($"Player {playerID + 1} is backing out to the Main Menu!");

        onReturnToMenuRequested?.Invoke(); 
    }
}