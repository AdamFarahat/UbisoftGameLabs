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

    // Called by the Cursor when a player presses 'B' while locked in
    public void CharacterUnlocked(int playerID)
    {
        Debug.Log($"Player {playerID + 1} deselected their character.");
        //TODO check if both are locked in
    }

    // Called by the Cursor when a player presses 'B' while ALREADY unlocked
    public void AttemptReturnToMenu(int playerID)
    {
        Debug.Log($"Player {playerID + 1} is backing out to the Main Menu!");
        // Todo inform menus manager to animate out player select (and animate in menu)
    }
}