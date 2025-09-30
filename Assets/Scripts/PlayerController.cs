using UnityEngine;
using TMPro;

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

    private bool inputAttemptedThisBeat = false; // tracks if they pressed something
    private int lastProcessedBeat = -1;          // ensures we only miss once per beat

    [Header("UI Hooks")]
    public UIManager livesUI;
    public UIManager chargesUI;

    void Start()
    {
        lives = maxLives;
        gameManager = FindFirstObjectByType<GameManager>();

        // horrible horrible awful dont look im in a rush i lost a week due to illness
        for (int i = 0; i < 5; i++)
        {
            livesUI.AddPrefab();
        }
    }

    void Update()
    {
        int currentBeat = BeatManager.Instance.GetCurrentBeatInCycle();

        // Reset once per new beat
        if (currentBeat != lastProcessedBeat)
        {
            if (!inputAttemptedThisBeat && lastProcessedBeat > 0)
            {
                RegisterMiss(lastProcessedBeat);
            }

            inputAttemptedThisBeat = false;
            lastProcessedBeat = currentBeat;
        }

        // CHARGE beats (1 & 2)
        if ((currentBeat == 1 || currentBeat == 2) && Input.GetKeyDown(chargeKey))
        {
            BeatManager.HitResult result = BeatManager.Instance.CheckHit();
            inputAttemptedThisBeat = true;

            if ((result == BeatManager.HitResult.Perfect || result == BeatManager.HitResult.Good) && charges < 2)
            {
                GainCharge();
                Debug.Log($"{name} hit charge ({charges}/2)");
            }
            else
            {
                RegisterMiss(currentBeat);
            }
        }

        // ACTION beat (3)
        if (currentBeat == 3 && charges >= 2)
        {
            if (Input.GetKeyDown(chargeKey))
            {
                inputAttemptedThisBeat = true;
                SetChargesToNone();
                Debug.Log($"{name} tried overcharging! Charges: 0/0");
                AttemptAction(GameManager.PlayerAction.None);
            }
            else if (Input.GetKeyDown(reloadKey))
            {
                inputAttemptedThisBeat = true;
                AttemptAction(GameManager.PlayerAction.Reload);
            }
            else if (Input.GetKeyDown(shootKey))
            {
                inputAttemptedThisBeat = true;
                AttemptAction(GameManager.PlayerAction.Shoot);
            }
            else if (Input.GetKeyDown(blockKey))
            {
                inputAttemptedThisBeat = true;
                AttemptAction(GameManager.PlayerAction.Block);
            }
        }
    }

    private void RegisterMiss(int beatNum)
    {
        // will clean this later
        if (beatNum == 1 || beatNum == 2) // only charge beats cause life loss
        {
            Debug.Log($"{name} missed charge on beat {beatNum}! Lives: {lives}");
        }
        else if (beatNum == 3)
        {
            TakeDamage(1);
            Debug.Log($"{name} missed their action opportunity on beat 3!");
            SetChargesToNone();
            //AttemptAction(GameManager.PlayerAction.None); DO NOT UNCOMMENT THIS IT MAKES THE GAME CRASH I DONT KNOW WHY
        }
    }

    private void AttemptAction(GameManager.PlayerAction action)
    {
        BeatManager.HitResult result = BeatManager.Instance.CheckHit();
        if (result == BeatManager.HitResult.Perfect || result == BeatManager.HitResult.Good)
        {
            gameManager.QueueAction(this, action);
            SetChargesToNone();
        }
        else
        {
            RegisterMiss(3);
        }
    }

    public void TakeDamage(int amount)
    {
        lives = Mathf.Max(0, lives - amount);
        livesUI.RemovePrefab();
        Debug.Log($"{name} took {amount} damage! Lives: {lives}");
    }

    public void GainCharge()
    {
        charges++;
        chargesUI.AddPrefab();
    }

    public void RemoveCharge()
    {
        charges--;
        chargesUI.RemovePrefab();
    }

    public void SetChargesToNone()
    {
        charges = 0;
        chargesUI.RemoveAll();
    }

    public bool IsAlive() => lives > 0;
}
