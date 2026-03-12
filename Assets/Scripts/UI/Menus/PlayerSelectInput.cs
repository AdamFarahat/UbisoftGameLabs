using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectInput : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The moving icon/cursor inside this prefab")]
    [SerializeField] private RectTransform cursorIcon; 
    private RectTransform[] characterSlots; 
    private PlayerSelectManager manager;
    private int currentIndex = 0;
    private int myPlayerID;
    private bool isLockedIn = false;

    public void SetupCursor(RectTransform[] slots, Transform canvasTransform, PlayerSelectManager myManager, int playerID)
    {
        transform.SetParent(canvasTransform, false);

        characterSlots = slots;
        manager = myManager;
        myPlayerID = playerID;
        
        // Start in the center slot 
        currentIndex = 1; 
        isLockedIn = false; // Ensure they start unlocked
        
        // Snap to the starting position
        if (characterSlots != null && characterSlots.Length > 0 && cursorIcon != null)
        {
            UpdateCursorPosition();
        }
    }

    // Hooked up to the 'Move' action in the Player Input component
    public void OnMove(InputAction.CallbackContext context)
    {
        // Block movement if the UI is still animating OR if the player already locked in
        if (!manager.IsAcceptingInput || isLockedIn) return; 

        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if (input.x > 0.5f) MoveCursor(1);        // Move Right
            else if (input.x < -0.5f) MoveCursor(-1); // Move Left
        }
    }

    // Hooked up to the 'Select' action in the Player Input component
    public void OnSelect(InputAction.CallbackContext context)
    {
        // Block selection if the UI is still animating
        if (!manager.IsAcceptingInput) return;

        // Do not allow selection on middle slot
        if (currentIndex == 1) return;

        // Only lock in if they aren't already locked
        if (context.performed && !isLockedIn)
        {
            isLockedIn = true; 

            manager.CharacterLockedIn(myPlayerID, currentIndex);
            
            //TODO visual update
            // TEMP: slightly shrink the cursor to show it is locked
            cursorIcon.localScale = new Vector3(0.8f, 0.8f, 1f); 
        }
    }

    // Move cursor between slots 
    private void MoveCursor(int direction)
    {
        if (characterSlots == null || characterSlots.Length == 0) return;

        currentIndex = Mathf.Clamp(currentIndex + direction, 0, characterSlots.Length - 1);

        /*// Wrap around logic
        if (currentIndex >= characterSlots.Length) currentIndex = characterSlots.Length - 1;
        else if (currentIndex < 0) currentIndex = 0;*/

        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        cursorIcon.position = characterSlots[currentIndex].position;
        transform.SetParent(characterSlots[currentIndex].transform, true);
    }


    // Hooked up to the 'Cancel' action in the Player Input component
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!manager.IsAcceptingInput) return;

        if (context.performed)
        {
            if (isLockedIn) // deselect
            {
                isLockedIn = false; 
                
                manager.CharacterUnlocked(myPlayerID, currentIndex);
                
                // TODO visual update 
                // TEMP: Visually return the cursor to normal size
                cursorIcon.localScale = Vector3.one; 
            }
            else // return to menu 
            {
                manager.AttemptReturnToMenu(myPlayerID);
            }
        }
    }

    
}