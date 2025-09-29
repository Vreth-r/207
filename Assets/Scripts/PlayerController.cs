using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public KeyCode chargeKey = KeyCode.Space;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode shootKey = KeyCode.F;
    public KeyCode blockKey = KeyCode.G;

    public int maxLives = 5;
    public int lives;
    public int ammo;

    private int charges = 0;
    private GameManager gameManager;

    void Start()
    {
        lives = maxLives;
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        // CHARGE handling
        if (Input.GetKeyDown(chargeKey))
        {
            BeatManager.HitResult result = BeatManager.Instance.CheckHit();
            if (result == BeatManager.HitResult.Perfect || result == BeatManager.HitResult.Good)
            {
                charges++;
                Debug.Log($"{name} hit charge ({charges}/2)");
            }
            else
            {
                // Missed charge → lose 1 life
                lives = Mathf.Max(0, lives - 1);
                Debug.Log($"{name} missed charge! Lives: {lives}");
            }
        }

        // ACTION handling (only if charged twice)
        if (charges >= 2)
        {
            if (Input.GetKeyDown(reloadKey))
            {
                AttemptAction(GameManager.PlayerAction.Reload);
            }
            else if (Input.GetKeyDown(shootKey))
            {
                AttemptAction(GameManager.PlayerAction.Shoot);
            }
            else if (Input.GetKeyDown(blockKey))
            {
                AttemptAction(GameManager.PlayerAction.Block);
            }
        }
    }

    void AttemptAction(GameManager.PlayerAction action)
    {
        BeatManager.HitResult result = BeatManager.Instance.CheckHit();
        if (result == BeatManager.HitResult.Perfect || result == BeatManager.HitResult.Good)
        {
            gameManager.QueueAction(this, action);
            charges = 0; // reset after action
        }
        else
        {
            Debug.Log($"{name} attempted {action} but missed the beat!");
        }
    }

    public void TakeDamage(int amount)
    {
        lives = Mathf.Max(0, lives - amount);
        Debug.Log($"{name} took {amount} damage! Lives: {lives}");
    }

    public bool IsAlive() => lives > 0;
}
