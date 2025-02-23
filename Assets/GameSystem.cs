using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameSystem : NetworkBehaviour
{
    public List<string> missions = new List<string> { "Hack Computer", "Steal Documents", "Disable Security" };
    public float gameTime = 300f; // 5 minutes

    private float timer;
    private bool gameRunning = false;
    private List<string> completedMissions = new List<string>();

    // ?? Assign in Inspector
    public TMP_Text missionText1;
    public TMP_Text missionText2;
    public TMP_Text missionText3;
    public TMP_Text timerText;

    private void Start()
    {
        if (!IsServer) return; // Only server manages game logic
        StartGame();
        UpdateMissionUI();
    }

    private void StartGame()
    {
        timer = gameTime;
        gameRunning = true;
    }

    private void Update()
    {
        if (!IsServer || !gameRunning) return;

        timer -= Time.deltaTime;
        UpdateTimerUI();

        if (timer <= 0)
        {
            SpyLoses();
        }
    }

    public void CompleteMission(string mission)
    {
        if (!IsServer) return;

        if (missions.Contains(mission) && !completedMissions.Contains(mission))
        {
            completedMissions.Add(mission);
            UpdateMissionUI();
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (completedMissions.Count == missions.Count)
        {
            SpyWins();
        }
    }

    private void SpyWins()
    {
        gameRunning = false;
        Debug.Log("Spy Wins! ??");
        // Trigger end game UI or logic
    }

    private void SpyLoses()
    {
        gameRunning = false;
        Debug.Log("Spy Loses! ?");
        // Trigger end game UI or logic
    }

    private void UpdateMissionUI()
    {
        TMP_Text[] missionTexts = { missionText1, missionText2, missionText3 };

        for (int i = 0; i < missions.Count; i++)
        {
            if (missionTexts[i] != null)
            {
                if (completedMissions.Contains(missions[i]))
                    missionTexts[i].text = $"<s>{missions[i]}</s>"; // ? Strike-through completed mission
                else
                    missionTexts[i].text = missions[i];
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
