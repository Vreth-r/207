using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    public PlayerController player1;
    public PlayerController player2;

    [Header("UI")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;

    private int p1Score = 0;
    private int p2Score = 0;
    private float countdownTime = 3f;

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            yield return StartCoroutine(CountdownPhase());
            yield return StartCoroutine(ResolutionPhase());
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator CountdownPhase()
    {
        player1.ResetTurn();
        player2.ResetTurn();
        resultText.text = "";

        float timer = countdownTime;
        while (timer > 0)
        {
            countdownText.text = Mathf.Ceil(timer).ToString();
            player1.HandleInput();
            player2.HandleInput();
            timer -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownText.text = "";
    }

    IEnumerator ResolutionPhase()
    {
        PlayerAction p1Action = player1.chosenAction;
        PlayerAction p2Action = player2.chosenAction;

        string resolutionLog = $"{player1.playerName}: {p1Action} | {player2.playerName}: {p2Action}\n";

        // Apply actions
        if (p1Action == PlayerAction.Reload)
            player1.ammo = Mathf.Min(player1.ammo + 1, player1.maxAmmo);
        if (p2Action == PlayerAction.Reload)
            player2.ammo = Mathf.Min(player2.ammo + 1, player2.maxAmmo);

        bool p1Hit = false;
        bool p2Hit = false;

        if (p1Action == PlayerAction.Shoot)
        {
            player1.ammo--;
            if (p2Action != PlayerAction.Block)
                p2Hit = true;
        }

        if (p2Action == PlayerAction.Shoot)
        {
            player2.ammo--;
            if (p1Action != PlayerAction.Block)
                p1Hit = true;
        }

        if (p1Hit && !p2Hit)
        {
            p1Score++;
            resolutionLog += $"{player1.playerName} wins this round!";
        }
        else if (p2Hit && !p1Hit)
        {
            p2Score++;
            resolutionLog += $"{player2.playerName} wins this round!";
        }
        else if (p1Hit && p2Hit)
        {
            resolutionLog += "Both players got hit!";
        }
        else
        {
            resolutionLog += "No one got hit.";
        }

        scoreText.text = $"P1: {p1Score} | P2: {p2Score}";
        resultText.text = resolutionLog;

        yield return null;
    }
}
