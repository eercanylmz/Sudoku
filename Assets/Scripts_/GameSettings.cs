using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public enum EGameMode
    {
        NOT_SET,
        EASY,
        MEDIUM,
        HARD,
        VERY_HARD,
    }
    public static GameSettings Instance;

    private void Awake()
    {
        _Paused = false;
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private EGameMode _GameMode;
    private bool _continuePreviosGame = false;
    private bool _exitAfterWon = false;
    private bool _Paused = false;

    public void SetExitAfetWon(bool set)
    {
        _exitAfterWon = set;
        _continuePreviosGame = false;
    }
    public bool GetExitAfterWon()
    {
        return _exitAfterWon;
    }

    public void SetContinuePreviousGame(bool continue_game)
    {
        _continuePreviosGame = continue_game;

    }
    public bool GetContinuePreviousGame()
    {
        return _continuePreviosGame;
    }

    public void SetPaused(bool paused) { _Paused = paused; }
    public bool GetPaused() { return _Paused; }

    void Start()
    {
        _GameMode = EGameMode.NOT_SET;
    }

    public void SetGameMode(EGameMode mode)
    {
        _GameMode = mode;
    }
    public void SetGameMode(string mode)
    {
        if (mode == "Easy") SetGameMode(EGameMode.EASY);
        else if (mode == "Medium") SetGameMode(EGameMode.MEDIUM);
        else if (mode == "Hard") SetGameMode(EGameMode.HARD);
        else if (mode == "VeryHard") SetGameMode(EGameMode.VERY_HARD);
        else SetGameMode(EGameMode.NOT_SET);
    }
    public string GetGameMode()
    {
        switch (_GameMode)
        {
            case EGameMode.EASY: return "Easy";
            case EGameMode.MEDIUM: return "Medium";
            case EGameMode.HARD: return "Hard";
            case EGameMode.VERY_HARD: return "VeryHard";
        }
        return "";
    }
}