using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player1;
    public PlayerController player2;

    public enum PlayerAction { None, Reload, Shoot, Block }

    private PlayerAction p1Queued = PlayerAction.None;
    private PlayerAction p2Queued = PlayerAction.None;
    private PlayerController p1Actor;
    private PlayerController p2Actor;

    [Header("Animations")]
    public Animator redAttack;
    public Animator blueAttack;
    public Animator redBullet;
    public Animator blueBullet;
    public Animator redShield;
    public Animator blueShield;
    public Animator blue;
    public Animator red;

    [Header("UI")]
    public TextMeshProUGUI gameEndText;
    public Button restartButton;

    private void OnEnable()
    {
        BeatManager.Instance.OnBeatEnd += HandleBeatEnd;
    }

    private void OnDisable()
    {
        BeatManager.Instance.OnBeatEnd -= HandleBeatEnd;
    }

    private void HandleBeatEnd(int beatNum)
    {
        if (beatNum == 3)
        {
            ResolveActions();
            p1Queued = PlayerAction.None;
            p2Queued = PlayerAction.None;
        }
    }

    public void RestartButtonFunc()
    {
        SceneManager.LoadScene("Menu");
    }
    void Update()
    {
        if (!player1.IsAlive() || !player2.IsAlive())
        {
            BeatManager.Instance.running = false;
            if (!player1.IsAlive() && !player2.IsAlive())
            {
                gameEndText.text = "Tie!";
            }
            else if (player1.IsAlive())
            {
                gameEndText.text = "Player 1 Wins!";
            }
            else
            {
                gameEndText.text = "Player 2 Wins!";
            }
            gameEndText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
        }
    }

    public void QueueAction(PlayerController actor, PlayerAction action)
    {
        if (actor == player1)
        {
            p1Queued = action;
        }
        else if (actor == player2)
        {
            p2Queued = action;
        }
    }

    void ResolveActions()
    {
        Debug.Log($"Resolving: P1 {p1Queued} vs P2 {p2Queued}");

        ResolveSingleAction(player1, p1Queued, player2, p2Queued);
        ResolveSingleAction(player2, p2Queued, player1, p1Queued);
    }

    void ResolveSingleAction(PlayerController actor, PlayerAction action, PlayerController opponent, PlayerAction opponentAction)
    {
        switch (action)
        {
            case PlayerAction.Reload:
                actor.ammo++;
                if (actor == player1)
                {
                    blueBullet.Play("BulletLoad");
                }
                else
                {
                    redBullet.Play("BulletLoad");
                }
                //Debug.Log($"{actor.name} reloaded → ammo {actor.ammo}");
                break;

            case PlayerAction.Shoot:
                if (actor.ammo > 0)
                {
                    actor.ammo--;
                    if (actor == player1)
                    {
                        blueAttack.Play("Lightning");
                    }
                    else
                    {
                        redAttack.Play("Fireball");
                    }
                    if (opponentAction == PlayerAction.Reload || opponentAction == PlayerAction.None || opponentAction == PlayerAction.Shoot)
                    {
                        opponent.TakeDamage(1);
                        if (opponent == player1)
                        {
                            blue.Play("Hurt");
                        }
                        else
                        {
                            red.Play("Hurt");
                        }
                        //Debug.Log($"{actor.name} shot {opponent.name} while vulnerable!");
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
                if (actor == player1)
                {
                    blueShield.Play("ShieldUp");
                }
                else
                {
                    redShield.Play("ShieldUp");
                }
                //Debug.Log($"{actor.name} blocked!");
                break;

            case PlayerAction.None:
                Debug.Log($"{actor.name} did nothing!");
                break;
        }
    }
}
