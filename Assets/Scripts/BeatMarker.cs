using UnityEngine;

public class BeatMarker : MonoBehaviour
{
    private Vector3 start;
    private Vector3 target;
    private float travelTime;
    private float spawnTime;

    public void Initialize(Vector3 targetPos, float timeToTravel)
    {
        start = transform.position;
        target = targetPos;
        travelTime = timeToTravel;
        spawnTime = Time.time;
    }

    void Update()
    {
        float elapsed = Time.time - spawnTime;
        float t = elapsed / travelTime;

        // Move at constant speed
        transform.position = Vector3.Lerp(start, target, t);

        // Destroy once it reaches (or passes) the hit zone
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
