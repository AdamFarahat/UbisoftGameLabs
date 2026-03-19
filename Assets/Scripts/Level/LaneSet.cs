using System;
using UnityEngine;
using UnityEngine.Assertions;

public class LaneSet : MonoBehaviour
{
    private static LaneSet instance;
    public static LaneSet Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<LaneSet>();
            return instance;
        }
    }

    public static int LaneCount => 5;

    [Header("Layout")]
    [SerializeField] private float laneSeparation = 12f;

    [SerializeField] private Transform heartLine;
    public static float HeartLine => Instance.heartLine.position.z;

    [SerializeField] private Transform visibleEndLine;
    public static float VisibleEndLine => Instance.visibleEndLine.position.z;

    [SerializeField] private Transform playerTargetHeight;
    public static float PlayerTargetHeight => Instance.playerTargetHeight.position.y;

    // TODO add PlayerLine and SpawnDistance

    [Header("Highlights")]
    [SerializeField] private SpriteRenderer[] highlights;
    [SerializeField] private Sprite gunHighlight;
    [SerializeField] private Sprite swordHighlight;
    [SerializeField] private Sprite sharedHighlight;

    [SerializeField] private float highlightIndexThreshold = 0.1f;
    private int gunLaneIndex = -1;
    private int swordLaneIndex = -1;

    private void OnValidate()
    {
        instance = this;
    }

    private void Awake()
    {
        instance = this;

        Assert.IsNotNull(heartLine);
        Assert.IsNotNull(visibleEndLine);
        Assert.IsNotNull(playerTargetHeight);

        Assert.IsTrue(highlights.Length == LaneCount);
        Assert.IsNotNull(gunHighlight);
        Assert.IsNotNull(swordHighlight);
        Assert.IsNotNull(sharedHighlight);
    }

    private void Start()
    {
        if (GunPlayerController.Instance != null)
        {
            gunLaneIndex = Mathf.FloorToInt(GunPlayerController.LaneIndex);
            AddGunHighlight();
        }

        if (SwordPlayerController.Instance != null)
        {
            swordLaneIndex = Mathf.FloorToInt(SwordPlayerController.LaneIndex);
            AddSwordHighlight();
        }
    }

    private void Update()
    {
        if (GunPlayerController.Instance != null)
        {
            int newGunIndex = Mathf.FloorToInt(GunPlayerController.LaneIndex);
            if (newGunIndex != gunLaneIndex && Mathf.Abs(GunPlayerController.LaneIndex - newGunIndex) < highlightIndexThreshold)
            {
                RemoveGunHighlight();
                gunLaneIndex = newGunIndex;
                AddGunHighlight();
            }
        }

        if (SwordPlayerController.Instance != null)
        {
            int newSwordIndex = Mathf.FloorToInt(SwordPlayerController.LaneIndex);
            if (newSwordIndex != swordLaneIndex && Mathf.Abs(SwordPlayerController.LaneIndex - newSwordIndex) < highlightIndexThreshold)
            {
                RemoveSwordHighlight();
                swordLaneIndex = newSwordIndex;
                AddSwordHighlight();
            }
        }
    }

    private void AddGunHighlight()
    {
        SpriteRenderer highlight = highlights[gunLaneIndex];

        if (highlight.sprite == null)
            highlight.sprite = gunHighlight;
        else if (highlight.sprite == swordHighlight)
            highlight.sprite = sharedHighlight;
    }

    private void AddSwordHighlight()
    {
        SpriteRenderer highlight = highlights[swordLaneIndex];

        if (highlight.sprite == null)
            highlight.sprite = swordHighlight;
        else if (highlight.sprite == gunHighlight)
            highlight.sprite = sharedHighlight;
    }

    private void RemoveGunHighlight()
    {
        SpriteRenderer highlight = highlights[gunLaneIndex];

        if (highlight.sprite == gunHighlight)
            highlight.sprite = null;
        else if (highlight.sprite == sharedHighlight)
            highlight.sprite = swordHighlight;
    }

    private void RemoveSwordHighlight()
    {
        SpriteRenderer highlight = highlights[swordLaneIndex];

        if (highlight.sprite == swordHighlight)
            highlight.sprite = null;
        else if (highlight.sprite == sharedHighlight)
            highlight.sprite = gunHighlight;
    }

    public static float PlayerLine
    {
        get
        {
            float line = 0f;
            int numPlayers = 0;
            if (GunPlayerController.Instance != null)
            {
                line += GunPlayerController.Instance.GetLaneDistance();
                numPlayers++;
            }
            if (SwordPlayerController.Instance != null)
            {
                line += SwordPlayerController.Instance.GetLaneDistance();
                numPlayers++;
            }
            return numPlayers > 0 ? line / numPlayers : line;
        }
    }

    private Vector3 LaneStart(int laneIndex)
    {
        int offsetIndex = laneIndex - Mathf.FloorToInt(0.5f * LaneCount);
        return GetLaneDirection() * (laneSeparation * offsetIndex * Vector3.right);
    }

    public Vector3 GetLanePosition(int laneIndex, float laneDistance)
    {
        return LaneStart(laneIndex) + laneDistance * transform.forward;
    }

    public Vector3 GetLanePosition(float laneIndex, float laneDistance)
    {
        int prevIndex = Math.Clamp(Mathf.FloorToInt(laneIndex), 0, LaneCount - 1);
        int nextIndex = Math.Clamp(Mathf.CeilToInt(laneIndex), 0, LaneCount - 1);

        return Vector3.Lerp(LaneStart(prevIndex), LaneStart(nextIndex), laneIndex - Mathf.Floor(laneIndex)) + laneDistance * transform.forward;
    }

    public Quaternion GetLaneDirection()
    {
        return Quaternion.LookRotation(transform.forward);
    }
}
