using UnityEngine;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;

    [Header("Beat Settings")]
    public float bpm = 120f;

    [Header("Timing Windows (seconds)")]
    public float perfectWindow = 0.12f;
    public float goodWindow = 0.3f;

    [Header("Visuals")]
    public GameObject beatPrefab;
    public Transform spawnPoint;
    public Transform hitZone;

    private float beatInterval;
    private float nextBeatTime;
    private BeatMarker activeMarker;
    private int beatCount = 0; // cycle counter

    void Awake() => Instance = this;

    void Start()
    {
        beatInterval = 60f / bpm;
        nextBeatTime = Time.time + beatInterval;
    }

    void Update()
    {
        if (Time.time >= nextBeatTime)
        {
            SpawnMarker();
            nextBeatTime += beatInterval;
        }
    }

    void SpawnMarker()
    {
        if (beatPrefab == null || spawnPoint == null || hitZone == null) return;

        beatCount = (beatCount % 3) + 1; // cycle 1 → 2 → 3 → repeat

        GameObject markerObj = Instantiate(beatPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);
        if (beatCount == 3) // if its an action beat
        {
            markerObj.GetComponent<Image>().color = Color.blue;
        }
        activeMarker = markerObj.GetComponent<BeatMarker>();
        activeMarker.Initialize(hitZone.position, beatInterval);
    }

    public enum HitResult { Miss, Good, Perfect }

    public HitResult CheckHit()
    {
        if (activeMarker == null) return HitResult.Miss;

        float distanceToBeat = Mathf.Abs(Time.time - activeMarker.arrivalTime);

        if (distanceToBeat <= perfectWindow) return HitResult.Perfect;
        if (distanceToBeat <= goodWindow) return HitResult.Good;
        return HitResult.Miss;
    }

    public int GetCurrentBeatInCycle() => beatCount; // 1, 2, or 3
}
