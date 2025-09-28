using UnityEngine;

public class BeatMarker : MonoBehaviour
{
    private Vector3 start;
    private Vector3 target;
    private float travelTime;
    private float spawnTime;

    public void Initialize(Vector3 targetPos, float travelTimeSeconds)
    {
        start = transform.position; // store where we spawned
        target = targetPos;
        travelTime = travelTimeSeconds;
        spawnTime = Time.time;
    }

    void Update()
    {
        float t = (Time.time - spawnTime) / travelTime;

        // move linearly from start → target
        transform.position = Vector3.Lerp(start, target, t);

        // destroy once it reaches or passes the end
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
