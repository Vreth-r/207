using UnityEngine;
using System;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;

    [Header("Beat Settings")]
    public float bpm = 120f;   // beats per minute

    [Header("Timing Windows (seconds)")]
    public float perfectWindow = 0.12f; 
    public float goodWindow = 0.22f;    

    [Header("Visuals")]
    public GameObject beatPrefab;
    public Transform spawnPoint;
    public Transform hitZone;

    [Header("Debug")]
    public bool debugTiming = false;

    // internal timing
    [HideInInspector] public float beatInterval; // seconds per beat
    private float nextBeatTime;
    private float currentBeatTime; 
    private GameObject activeBeatMarker;
    private int beatIndex = 0;

    // events
    public event Action<int> OnBeat; // pass beat index if you want cycles

    void Awake() => Instance = this;

    void Start()
    {
        beatInterval = 60f / bpm; // time between beats in seconds
        nextBeatTime = Time.time; // first beat spawns immediately
    }

    void Update()
    {
        if (Time.time >= nextBeatTime)
        {
            SpawnBeat();
            currentBeatTime = nextBeatTime;
            nextBeatTime += beatInterval;
        }
    }

    void SpawnBeat()
    {
        // destroy old marker if still alive
        if (activeBeatMarker != null)
            Destroy(activeBeatMarker);

        // spawn new marker
        if (beatPrefab != null && spawnPoint != null && hitZone != null)
        {
            activeBeatMarker = Instantiate(beatPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);

            BeatMarker bm = activeBeatMarker.GetComponent<BeatMarker>();
            if (bm != null)
            {
                // automatically set travel time so marker lands on hit zone exactly on beat
                bm.Initialize(hitZone.position, beatInterval);
            }
        }

        beatIndex++;
        OnBeat?.Invoke(beatIndex);
    }

    public enum HitResult { Miss, Good, Perfect }

    /// <summary>
    /// Check a player input against the current active beat
    /// </summary>
    public HitResult CheckHit(out float distanceToBeat)
    {
        distanceToBeat = Mathf.Infinity;
        if (activeBeatMarker == null) return HitResult.Miss;

        // measure input vs scheduled beat time
        distanceToBeat = Mathf.Abs(Time.time - currentBeatTime);

        if (distanceToBeat <= perfectWindow) return HitResult.Perfect;
        if (distanceToBeat <= goodWindow) return HitResult.Good;
        return HitResult.Miss;
    }
}
