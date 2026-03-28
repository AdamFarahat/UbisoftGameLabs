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

    [SerializeField] private Transform playerTarget;
    public static float PlayerTargetHeight => Instance.playerTarget.position.y;
    public static float PlayerLine => Instance.playerTarget.position.z;

    [SerializeField] private Transform spawnLine;
    public static float SpawnLine => Instance.spawnLine.position.z;

    [SerializeField] private Transform enemyMoveBufferLine;
    public static float EnemyMoveBufferLine => Instance.enemyMoveBufferLine.position.z;
    
    [SerializeField] private Transform enemyShootBufferLine;
    public static float EnemyShootBufferLine => Instance.enemyShootBufferLine.position.z;

    [SerializeField] private Transform tutorialEnemyDespawnLine;
    public static float TutorialEnemyDespawnLine => Instance.tutorialEnemyDespawnLine.position.z;

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
        Assert.IsNotNull(playerTarget);
        Assert.IsNotNull(spawnLine);
        Assert.IsNotNull(enemyMoveBufferLine);
        Assert.IsNotNull(enemyShootBufferLine);
        Assert.IsNotNull(tutorialEnemyDespawnLine);

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

    public static float ScreenAngleOfVector(Vector3 vector)
    {
        Vector3 cam = FindFirstObjectByType<Camera>().transform.InverseTransformDirection(vector);
        return Mathf.Rad2Deg * Mathf.Atan2(cam.y, cam.x);
    }
}
