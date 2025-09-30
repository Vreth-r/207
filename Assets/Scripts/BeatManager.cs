using UnityEngine;
using UnityEngine.UI;
using System;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;
    public bool running = true;

    [Header("Beat Settings")]
    public float bpm = 120f;

    [Header("Timing Windows (seconds)")]
    public float perfectWindow = 0.0f;
    public float goodWindow = 0.05f;

    [Header("Visuals")]
    public GameObject beatPrefab;
    public Transform spawnPoint;
    public Transform hitZone;

    private float beatInterval;
    private float nextBeatTime;
    private BeatMarker activeMarker;
    private int beatCount = 0; // cycle counter

    public event Action<int> OnBeatEnd;

    void Awake() => Instance = this;

    void Start()
    {
        beatInterval = 60f / bpm;
        nextBeatTime = Time.time + beatInterval;
    }

    public void IncreaseBPM(float delta)
    {
        bpm += delta;
        beatInterval = 60f / bpm;
    }

    void Update()
    {
        if (!running) return;

        if (Time.time >= nextBeatTime)
        {
            if (OnBeatEnd != null)
            {
                OnBeatEnd.Invoke(beatCount);
            }
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
            markerObj.GetComponent<Image>().color = Color.red;
        }
        activeMarker = markerObj.GetComponent<BeatMarker>();
        activeMarker.Initialize(hitZone.position, beatInterval);
    }

    public enum HitResult { Miss, Good, Perfect }

    public HitResult CheckHit()
    {
        if (activeMarker == null) return HitResult.Miss;

        float distanceToBeat = Mathf.Abs(Time.time - activeMarker.arrivalTime);
        if (distanceToBeat <= perfectWindow)
        {
            return HitResult.Perfect;
        }
        if (distanceToBeat <= goodWindow)
        {
            return HitResult.Good;
        }
        return HitResult.Miss;
    }

    public int GetCurrentBeatInCycle() => beatCount; // 1, 2, or 3
}
