using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectInput : MonoBehaviour
{
    private RectTransform cursorIcon; 
    private RectTransform[] characterSlots; 
    private PlayerSelectManager manager;
    private int currentIndex = 0;
    private int myPlayerID;
    private bool isLockedIn = false;

    public int CurrentIndex => currentIndex;

    // 
    public void SetupCursor(RectTransform[] slots, Transform canvasTransform, PlayerSelectManager myManager, int playerID, GameObject dummyCursor)
    {
        // Attach this transform to the canvas 
        transform.SetParent(canvasTransform, false);

        characterSlots = slots;
        manager = myManager;
        myPlayerID = playerID;
        currentIndex = 1; // cursors start in the middle 
        isLockedIn = false; 

        // Assign the new cursor to the dummy cursor on screen
        if (dummyCursor != null)
        {
            // Make dummy a child of this invisible controller and center it
            dummyCursor.transform.SetParent(this.transform, false);
            dummyCursor.transform.localPosition = Vector3.zero;
            
            // Set the dummy cursor transform to this player's cursor transform 
            cursorIcon = dummyCursor.GetComponent<RectTransform>();
        }

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

            // Stop two players from selecting the same character
            if (manager.IsSlotTaken(currentIndex))
            {
                Debug.Log("Slot is already taken by the other player!");
                return;
            }

            isLockedIn = true; 

            manager.CharacterLockedIn(myPlayerID, currentIndex);

            //Check which slot the other player is in. Then set the device for the character based on that
            if (currentIndex == 0)
            {
                OnSwordPlayerSelect(context);
            }
            else if (currentIndex == 2)
            {
                OnGunPlayerSelect(context);
            }
            
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

        UpdateCursorPosition();
    }

    // Move the transform component of this object to the new slot
    private void UpdateCursorPosition()
    {
        // Attach this transform to the new slot and center it
        transform.SetParent(characterSlots[currentIndex].transform, false);
        transform.localPosition = Vector3.zero; 

        // Notify manager and shift icons 
        manager.UpdateCursorSlot(myPlayerID, currentIndex);
        manager.RefreshCursorOffsets();
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

                //Check which slot the player is in. Then unset the device for that character based on that
                if (currentIndex == 0)
                {
                    OnSwordPlayerDeselect(context);
                }
                else if (currentIndex == 2)
                {
                    OnGunPlayerDeselect(context);
                } 
            }
            else // return to menu 
            {
                manager.AttemptReturnToMenu(myPlayerID);
            }
        }
    }

    public void OnGunPlayerSelect(InputAction.CallbackContext ctx)
    {
        PlayerSelect.gunPlayerDevice = ctx.control.device;
    }

    public void OnSwordPlayerSelect(InputAction.CallbackContext ctx)
    {
        PlayerSelect.swordPlayerDevice = ctx.control.device;
    }

    public void OnGunPlayerDeselect(InputAction.CallbackContext ctx)
    {
        if (PlayerSelect.gunPlayerDevice == ctx.control.device)
        {
            PlayerSelect.gunPlayerDevice = null;
        }
    }

    public void OnSwordPlayerDeselect(InputAction.CallbackContext ctx)
    {
        if (PlayerSelect.swordPlayerDevice == ctx.control.device)
        {
            PlayerSelect.swordPlayerDevice = null;
        }
    }
}