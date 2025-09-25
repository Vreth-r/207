using UnityEngine;
using System;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;

    [Header("Beat Settings")]
    public float bpm = 120f;
    public int beatsPerCycle = 4;

    [Header("Timing Windows (seconds)")]
    public float perfectWindow = 0.12f; // ±120ms
    public float goodWindow = 0.22f;    // ±220ms

    [Header("Visuals")]
    public GameObject beatPrefab;
    public Transform spawnPoint;
    public Transform hitZone;
    public float travelTime = 2f;

    [Header("Debug")]
    public bool debugTiming = false;

    // internal timing
    [HideInInspector] public float beatInterval;
    private float songStartTime;
    private float nextBeatTime;
    private int currentBeat = 0;

    public event Action<int> OnBeat;

    void Awake() => Instance = this;

    void Start()
    {
        beatInterval = 60f / bpm;
        songStartTime = Time.time;
        nextBeatTime = songStartTime + beatInterval;
    }

    void Update()
    {
        // catch up if frames skipped: spawn for each beat passed
        while (Time.time >= nextBeatTime)
        {
            currentBeat = (currentBeat + 1) % beatsPerCycle;
            OnBeat?.Invoke(currentBeat);
            SpawnMarker();
            nextBeatTime += beatInterval;
        }
    }

    void SpawnMarker()
    {
        if (beatPrefab == null || spawnPoint == null || hitZone == null) return;
        GameObject marker = Instantiate(beatPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);
        BeatMarker bm = marker.GetComponent<BeatMarker>();
        if (bm != null)
            bm.Initialize(hitZone.position, travelTime);
    }

    public enum HitResult { Miss, Good, Perfect }

    // Main check function: returns graded result and outputs distance to nearest beat (in seconds)
    public HitResult CheckHit(float perfectWindowOverride, float goodWindowOverride, out float distanceToBeat)
    {
        float interval = beatInterval;
        float timeSinceStart = Time.time - songStartTime;

        // remainder = time since last beat (0..interval)
        float remainder = Mathf.Repeat(timeSinceStart, interval);

        // distance to nearest beat (either forward or backward)
        distanceToBeat = Mathf.Min(remainder, interval - remainder);

        float pW = (perfectWindowOverride > 0) ? perfectWindowOverride : perfectWindow;
        float gW = (goodWindowOverride > 0) ? goodWindowOverride : goodWindow;

        if (distanceToBeat <= pW) return HitResult.Perfect;
        if (distanceToBeat <= gW) return HitResult.Good;
        return HitResult.Miss;
    }

    // convenience overload using configured windows
    public HitResult CheckHit(out float distance)
    {
        return CheckHit(perfectWindow, goodWindow, out distance);
    }

    // quick boolean test
    public bool IsOnBeat(float window)
    {
        float d;
        return CheckHit(window, window, out d) != HitResult.Miss;
    }
}
