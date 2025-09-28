using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerAction { None, Reload, Shoot, Block }

    [Header("Keys")]
    public KeyCode chargeKey;
    public KeyCode reloadKey;
    public KeyCode shootKey;
    public KeyCode blockKey;

    [Header("Stats")]
    public int maxLives = 5;
    public int lives;
    public int maxAmmo = 6;
    public int ammo;

    [Header("State")]
    public int chargeCount = 0;
    public PlayerAction chosenAction = PlayerAction.None;

    void Start()
    {
        lives = maxLives;
        ammo = 0;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (BeatManager.Instance == null) return;

        // --- CHARGE ---
        if (Input.GetKeyDown(chargeKey))
        {
            float dist;
            var result = BeatManager.Instance.CheckHit(out dist);

            if (result != BeatManager.HitResult.Miss)
            {
                chargeCount++;
                Debug.Log($"{name} charge hit {chargeCount}/2 - {result}");
            }
            else
            {
                chargeCount = 0;
                LoseLife(); // ⬅️ new: miss charge = lose 1 life
                Debug.Log($"{name} charge MISS! Lost 1 life (now {lives})");
            }
        }

        // --- ACTIONS (only after 2 charges) ---
        if (chargeCount >= 2 && chosenAction == PlayerAction.None)
        {
            if (Input.GetKeyDown(reloadKey))
            {
                float dist; var res = BeatManager.Instance.CheckHit(out dist);
                if (res != BeatManager.HitResult.Miss)
                {
                    chosenAction = PlayerAction.Reload;
                    Debug.Log($"{name} chose Reload");
                }
            }
            else if (Input.GetKeyDown(shootKey) && ammo > 0)
            {
                float dist; var res = BeatManager.Instance.CheckHit(out dist);
                if (res != BeatManager.HitResult.Miss)
                {
                    chosenAction = PlayerAction.Shoot;
                    Debug.Log($"{name} chose Shoot");
                }
            }
            else if (Input.GetKeyDown(blockKey))
            {
                float dist; var res = BeatManager.Instance.CheckHit(out dist);
                if (res != BeatManager.HitResult.Miss)
                {
                    chosenAction = PlayerAction.Block;
                    Debug.Log($"{name} chose Block");
                }
            }
        }
    }

    // --- Resolve chosen action at end of beat ---
    public void ResolveAction(PlayerController opponent)
    {
        switch (chosenAction)
        {
            case PlayerAction.Reload:
                ammo = Mathf.Min(ammo + 1, maxAmmo);
                Debug.Log($"{name} reloaded. Ammo: {ammo}");
                break;

            case PlayerAction.Shoot:
                if (ammo > 0)
                {
                    ammo--;
                    Debug.Log($"{name} shot! Ammo left: {ammo}");

                    // Resolve hit logic based on opponent's action
                    if (opponent.chosenAction == PlayerAction.Reload)
                    {
                        opponent.LoseLife(); // opponent loses 1 life
                        Debug.Log($"{opponent.name} was shot while reloading! Lost 1 life.");
                    }
                    else if (opponent.chosenAction == PlayerAction.Block)
                    {
                        Debug.Log($"{opponent.name} blocked the shot! No damage.");
                    }
                }
                break;

            case PlayerAction.Block:
                Debug.Log($"{name} blocked this turn.");
                break;
        }

        // Reset for next round
        chosenAction = PlayerAction.None;
        chargeCount = 0;
    }

    public void LoseLife()
    {
        lives--;
        if (lives <= 0)
        {
            lives = 0;
            Debug.Log($"{name} is out of lives! GAME OVER for this player.");
            // TODO: hook into game manager for win/lose
        }
    }
}
