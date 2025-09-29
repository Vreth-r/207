using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;

    [Header("Beat Settings")]
    public float bpm = 120f;

    [Header("Timing Windows (seconds)")]
    public float perfectWindow = 0.12f; // ±120ms
    public float goodWindow = 0.22f;    // ±220ms

    [Header("Visuals")]
    public GameObject beatPrefab;
    public Transform spawnPoint;
    public Transform hitZone;

    private float beatInterval;
    private float nextBeatTime;
    private BeatMarker activeMarker;

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

        GameObject markerObj = Instantiate(beatPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);
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
}
