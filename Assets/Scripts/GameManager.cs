using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player1;
    public PlayerController player2;

    public enum PlayerAction { None, Reload, Shoot, Block }

    private PlayerAction p1Queued = PlayerAction.None;
    private PlayerAction p2Queued = PlayerAction.None;
    private PlayerController p1Actor;
    private PlayerController p2Actor;

    void Update()
    {
        // Check win/loss
        if (!player1.IsAlive())
        {
            Debug.Log("Player 2 wins!");
            enabled = false;
        }
        else if (!player2.IsAlive())
        {
            Debug.Log("Player 1 wins!");
            enabled = false;
        }
    }

    public void QueueAction(PlayerController actor, PlayerAction action)
    {
        if (actor == player1)
        {
            p1Queued = action;
            p1Actor = actor;
        }
        else if (actor == player2)
        {
            p2Queued = action;
            p2Actor = actor;
        }

        // If both queued, resolve immediately
        if (p1Queued != PlayerAction.None && p2Queued != PlayerAction.None)
        {
            ResolveActions();
            p1Queued = PlayerAction.None;
            p2Queued = PlayerAction.None;
        }
    }

    void ResolveActions()
    {
        Debug.Log($"Resolving: P1 {p1Queued} vs P2 {p2Queued}");

        // Player 1’s action
        ResolveSingleAction(p1Actor, p1Queued, p2Actor, p2Queued);
        // Player 2’s action
        ResolveSingleAction(p2Actor, p2Queued, p1Actor, p1Queued);
    }

    void ResolveSingleAction(PlayerController actor, PlayerAction action, PlayerController opponent, PlayerAction opponentAction)
    {
        switch (action)
        {
            case PlayerAction.Reload:
                actor.ammo++;
                Debug.Log($"{actor.name} reloaded → ammo {actor.ammo}");
                break;

            case PlayerAction.Shoot:
                if (actor.ammo > 0)
                {
                    actor.ammo--;
                    if (opponentAction == PlayerAction.Reload)
                    {
                        opponent.TakeDamage(1);
                        Debug.Log($"{actor.name} shot {opponent.name} while reloading!");
                    }
                    else if (opponentAction == PlayerAction.Block)
                    {
                        Debug.Log($"{opponent.name} blocked {actor.name}'s shot!");
                    }
                }
                else
                {
                    Debug.Log($"{actor.name} tried to shoot but had no ammo!");
                }
                break;

            case PlayerAction.Block:
                Debug.Log($"{actor.name} blocked!");
                break;
        }
    }
}
