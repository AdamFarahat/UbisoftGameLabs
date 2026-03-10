using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform mainCanvas;
    public RectTransform[] allCharacterSlots;

    private PlayerInputManager inputManager;
    
    // The cursors check this to see if they are allowed to move or select
    public bool IsAcceptingInput { get; private set; } = false;

    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        
        // Lock inputs immediately on start so players can't join during the animation
        if (inputManager != null)
        {
            inputManager.DisableJoining();
        }
    }

    // Called by PlayerSelectAnimations when the intro loop finishes
    public void EnableInput()
    {
        IsAcceptingInput = true;
        
        if (inputManager != null)
        {
            inputManager.EnableJoining();
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerSelectCursor newCursor = playerInput.GetComponent<PlayerSelectCursor>();
        
        if (newCursor != null)
        {
            newCursor.SetupCursor(allCharacterSlots, mainCanvas, this, playerInput.playerIndex);
        }
    }

    public void CharacterLockedIn(int playerID, int slotIndex)
    {
        Debug.Log($"Player {playerID + 1} locked in character slot {slotIndex}!");
    }
}