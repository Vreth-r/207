using UnityEngine;

public enum PlayerAction { None, Reload, Shoot, Block }

public class PlayerController : MonoBehaviour
{
    public string playerName = "Player";
    public int ammo = 0;
    public int maxAmmo = 3;
    public PlayerAction chosenAction = PlayerAction.None;

    [Header("Controls")]
    public KeyCode reloadKey;
    public KeyCode shootKey;
    public KeyCode blockKey;

    public void ResetTurn()
    {
        chosenAction = PlayerAction.None;
    }

    public void HandleInput()
    {
        if (chosenAction != PlayerAction.None) return;

        if (Input.GetKeyDown(reloadKey))
            chosenAction = PlayerAction.Reload;
        else if (Input.GetKeyDown(shootKey) && ammo > 0)
            chosenAction = PlayerAction.Shoot;
        else if (Input.GetKeyDown(blockKey))
            chosenAction = PlayerAction.Block;
    }
}
