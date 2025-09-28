using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    public PlayerController player1;
    public PlayerController player2;

    [Header("Beat Cycle Settings")]
    public int beatsPerRound = 3; // how many beats make one "turn"

    private int currentBeat = 0;

    /*
    void Start()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat += OnBeat;
        }
    }

    void OnDestroy()
    {
        if (BeatManager.Instance != null)
        {
            BeatManager.Instance.OnBeat -= OnBeat;
        }
    }

    void OnBeat(int beatIndex)
    {
        currentBeat = beatIndex;

        // End of cycle: resolve both players simultaneously
        if (currentBeat == beatsPerRound - 1)
        {
            ResolveActions();
            CheckGameOver();
        }
    }*/

    void ResolveActions()
    {
        if (player1 == null || player2 == null) return;

        // Both players resolve their chosen action for this cycle
        player1.ResolveAction(player2);
        player2.ResolveAction(player1);

        Debug.Log($"--- End of Round --- P1 Lives: {player1.lives} | P1 Ammo: {player1.ammo} || P2 Lives: {player2.lives} | P2 Ammo: {player2.ammo}");
    }

    void CheckGameOver()
    {
        if (player1.lives <= 0 && player2.lives <= 0)
        {
            Debug.Log("Draw! Both players are out of lives.");
            // TODO: Hook in UI/game restart
        }
        else if (player1.lives <= 0)
        {
            Debug.Log("Player 2 Wins!");
            // TODO: Hook in UI/game restart
        }
        else if (player2.lives <= 0)
        {
            Debug.Log("Player 1 Wins!");
            // TODO: Hook in UI/game restart
        }
    }
}
