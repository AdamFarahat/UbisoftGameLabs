using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerSelectManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform mainCanvas;
    [SerializeField] private RectTransform[] allCharacterSlots;
    [SerializeField] private GameObject[] dummyCursors;
    [SerializeField] private float cursorOverlapOffset = 20f;
    private readonly List<PlayerSelectInput> activePlayers = new();
    private readonly int[] cursorSlots = new int[2] { 1, 1 }; // default middle 
    private bool[] playerReadyStates = new bool[2];

    [SerializeField] private Image heartUIImage;
    private readonly int leftAmountID = Shader.PropertyToID("_LeftAmount");
    private readonly int rightAmountID = Shader.PropertyToID("_RightAmount");

    [Header("Events")]
    public UnityEvent onReturnToMenuRequested;
    public UnityEvent onAllPlayersReady;

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
        heartUIImage.material.SetFloat("_ShakeStrength", 0f);
        
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

    // When a player joins, assign a new cursor to it from the scene
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int pIndex = playerInput.playerIndex;
        PlayerSelectInput newPlayerInput = playerInput.GetComponent<PlayerSelectInput>();
        
        if (newPlayerInput != null)
        {
            // Grab the specific scene cursor for this player ID
            GameObject assignedCursor = null;
            if (dummyCursors != null && pIndex >= 0 && pIndex < dummyCursors.Length)
            {
                assignedCursor = dummyCursors[pIndex];
            }

            // Setup logic for new cursor 
            newPlayerInput.SetupCursor(allCharacterSlots, mainCanvas, this, pIndex, assignedCursor);

            activePlayers.Add(newPlayerInput);
            RefreshCursorOffsets();
        }
    }

    public void CharacterLockedIn(int playerID, int slotIndex)
    {
        Debug.Log($"Player {playerID + 1} locked in character slot {slotIndex}!");
        
        // todo i will put this material crap elsewhere 
        // Smooth increase over a quarter second
        if (slotIndex == 0) heartUIImage.material.DOFloat(1f, leftAmountID, 0.25f).SetEase(Ease.OutQuad);
        else if (slotIndex == 2) heartUIImage.material.DOFloat(1f, rightAmountID, 0.25f).SetEase(Ease.OutQuad);

        // Start shake
        heartUIImage.material.SetFloat("_ShakeStrength", 1f);

        // Smooth shake to 0 over a quarter second
        heartUIImage.material.DOFloat(0f, "_ShakeStrength", 0.25f).SetEase(Ease.OutQuad);

        // ready player # 
        playerReadyStates[playerID] = true;
        CheckIfAllPlayersReady();
    }

    // Checks if another player has already locked into this specific slot
    public bool IsSlotTaken(int slotIndex)
    {
        // Return true if either player is locked in at this slot
        if (playerReadyStates[0] && cursorSlots[0] == slotIndex) return true;
        if (playerReadyStates[1] && cursorSlots[1] == slotIndex) return true;

        return false;
    }

    public void CharacterUnlocked(int playerID, int slotIndex)
    {
        Debug.Log($"Player {playerID + 1} deselected their character in slot {slotIndex}.");
        
        // todo i will put this material crap elsewhere 
        if (slotIndex == 0) heartUIImage.material.DOFloat(0f, leftAmountID, 0.25f).SetEase(Ease.OutQuad);
        else if (slotIndex == 2) heartUIImage.material.DOFloat(0f, rightAmountID, 0.25f).SetEase(Ease.OutQuad);


        playerReadyStates[playerID] = false;
    }

    private void CheckIfAllPlayersReady()
    {
        if (activePlayers.Count < 2) return;

        // Check if both ready
        if (playerReadyStates[0] && playerReadyStates[1])
        {            
            // Lock inputs during animations 
            IsAcceptingInput = false; 

            onAllPlayersReady?.Invoke();
        }
    }

    public void AttemptReturnToMenu(int playerID)
    {
        Debug.Log($"Player {playerID + 1} is backing out to the Main Menu!");

        PlayerSelect.OnGameEnd(); // reset player select state

        onReturnToMenuRequested?.Invoke(); 
    }

    // Shift a cursor if it is sharing a slot with another cursor 
    public void RefreshCursorOffsets()
    {
        if (dummyCursors == null || dummyCursors.Length < 2) return;

        // Assign cursors 
        RectTransform cursor1 = dummyCursors[0] != null ? dummyCursors[0].GetComponent<RectTransform>() : null;
        RectTransform cursor2 = dummyCursors[1] != null ? dummyCursors[1].GetComponent<RectTransform>() : null;

        if (cursor1 == null || cursor2 == null) return;

        // If both cursors are in the same slot, shift them a bit
        if (cursorSlots[0] == cursorSlots[1])
        {
            // Nudge cursor 1 left, and cursor 2 right
            cursor1.anchoredPosition = new Vector2(-cursorOverlapOffset, cursor1.anchoredPosition.y);
            cursor2.anchoredPosition = new Vector2(cursorOverlapOffset, cursor2.anchoredPosition.y);
        }
        else
        {
            // If not sharing slot, center cursor 
            cursor1.anchoredPosition = new Vector2(0f, cursor1.anchoredPosition.y);
            cursor2.anchoredPosition = new Vector2(0f, cursor2.anchoredPosition.y);
        }
    }

    // Update the manager's record of where a player's cursor is
    public void UpdateCursorSlot(int playerID, int slotIndex)
    {
        if (playerID >= 0 && playerID < cursorSlots.Length)
        {
            cursorSlots[playerID] = slotIndex;
        }
    }    
}