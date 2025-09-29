using UnityEngine;

public class BeatMarker : MonoBehaviour
{
    private Vector3 start;
    private Vector3 target;
    private float travelTime;
    private float spawnTime;

    [HideInInspector] public float arrivalTime; // when it hits the zone

    public void Initialize(Vector3 targetPos, float travelTimeSeconds)
    {
        start = transform.position;
        target = targetPos;
        travelTime = travelTimeSeconds;
        spawnTime = Time.time;

        arrivalTime = spawnTime + travelTime; // store expected arrival time
    }

    void Update()
    {
        float t = (Time.time - spawnTime) / travelTime;
        transform.position = Vector3.Lerp(start, target, t);

        if (t >= 1f)
            Destroy(gameObject);
    }
}
