using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    public PlayerController player1;
    public PlayerController player2;

    [Header("UI")]
    public TextMeshProUGUI resultText;

    void Start()
    {
        BeatManager.Instance.OnBeat += OnBeat;
    }

    void OnBeat(int beatIndex)
    {
        // At the end of cycle, resolve actions
        if (beatIndex == 0) // every new cycle
        {
            ResolveRound();
            player1.ResetTurn();
            player2.ResetTurn();
        }
    }

    void ResolveRound()
    {
        PlayerAction p1Action = player1.chosenAction;
        PlayerAction p2Action = player2.chosenAction;

        player1.ResolveAction(player2);
        player2.ResolveAction(player1);

        string log = $"P1: {p1Action}, P2: {p2Action}";
        resultText.text = log;
    }
}
