using UnityEngine;

public enum PlayerAction { None, Reload, Shoot, Block }

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode chargeKey;
    public KeyCode reloadKey;
    public KeyCode shootKey;
    public KeyCode blockKey;

    [Header("State")]
    public int ammo = 0;
    public int maxAmmo = 3;
    public int chargeCount = 0;
    public PlayerAction chosenAction = PlayerAction.None;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // ensure BeatManager exists
        if (BeatManager.Instance == null) return;

        // When player presses charge
        if (Input.GetKeyDown(chargeKey))
        {
            float dist;
            var result = BeatManager.Instance.CheckHit(out dist); // uses configured windows from BeatManager

            if (result != BeatManager.HitResult.Miss)
            {
                chargeCount++;
                if (BeatManager.Instance.debugTiming)
                    Debug.Log($"{name} charge hit {chargeCount}/2 - {result} (dist {dist:F3}s)");
            }
            else
            {
                chargeCount = 0; // miss resets charge
                if (BeatManager.Instance.debugTiming)
                {
                    Debug.Log($"{name} charge MISS (dist {dist:F3}s). Reset charge.");
                    Debug.Log($"beatInterval: {BeatManager.Instance.beatInterval:F3}s, time: {Time.time:F3}");
                }
            }
        }

        // Can only perform an action if fully charged
        if (chargeCount >= 2 && chosenAction == PlayerAction.None)
        {
            if (Input.GetKeyDown(reloadKey))
            {
                float dist; var res = BeatManager.Instance.CheckHit(out dist);
                if (res != BeatManager.HitResult.Miss)
                {
                    chosenAction = PlayerAction.Reload;
                    if (BeatManager.Instance.debugTiming) Debug.Log($"{name} chose Reload ({res}, d={dist:F3}s)");
                }
                else if (BeatManager.Instance.debugTiming) Debug.Log($"{name} Reload MISS (d={dist:F3}s)");
            }
            else if (Input.GetKeyDown(shootKey) && ammo > 0)
            {
                float dist; var res = BeatManager.Instance.CheckHit(out dist);
                if (res != BeatManager.HitResult.Miss)
                {
                    chosenAction = PlayerAction.Shoot;
                    if (BeatManager.Instance.debugTiming) Debug.Log($"{name} chose Shoot ({res}, d={dist:F3}s)");
                }
                else if (BeatManager.Instance.debugTiming) Debug.Log($"{name} Shoot MISS (d={dist:F3}s)");
            }
            else if (Input.GetKeyDown(blockKey))
            {
                float dist; var res = BeatManager.Instance.CheckHit(out dist);
                if (res != BeatManager.HitResult.Miss)
                {
                    chosenAction = PlayerAction.Block;
                    if (BeatManager.Instance.debugTiming) Debug.Log($"{name} chose Block ({res}, d={dist:F3}s)");
                }
                else if (BeatManager.Instance.debugTiming) Debug.Log($"{name} Block MISS (d={dist:F3}s)");
            }
        }
    }



    public void ResolveAction(PlayerController opponent)
    {
        if (chosenAction == PlayerAction.Reload)
        {
            ammo = Mathf.Min(ammo + 1, maxAmmo);
        }
        else if (chosenAction == PlayerAction.Shoot)
        {
            ammo--;
            if (opponent.chosenAction != PlayerAction.Block)
            {
                Debug.Log($"{gameObject.name} hit {opponent.gameObject.name}!");
                opponent.Die();
            }
        }
        else if (chosenAction == PlayerAction.Block)
        {
            Debug.Log($"{gameObject.name} blocked!");
        }
    }

    public void ResetTurn()
    {
        chargeCount = 0;
        chosenAction = PlayerAction.None;
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} is out!");
    }
}
