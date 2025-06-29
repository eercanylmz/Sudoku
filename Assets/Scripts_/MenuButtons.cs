using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void LoadEasyScene(string name)
    {
        GameSettings.Instance.SetGameMode(GameSettings.EGameMode.EASY);
        SceneManager.LoadScene(name);
    }

    public void LoadMedýumScene(string name)
    {
        GameSettings.Instance.SetGameMode(GameSettings.EGameMode.MEDIUM);
        SceneManager.LoadScene(name);
    }

    public void LoadHardScene(string name)
    {
        GameSettings.Instance.SetGameMode(GameSettings.EGameMode.HARD);
        SceneManager.LoadScene(name);
    }
    public void LoadVeryHardScene(string name)
    {
        GameSettings.Instance.SetGameMode(GameSettings.EGameMode.VERY_HARD);
        SceneManager.LoadScene(name);
    }
    public void AcivateObject(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void DeAcivateObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void SetPause(bool pause)
    {
        GameSettings.Instance.SetPaused(pause);
    }

    public void ContinuePreviousGame(bool continue_game)
    {
        GameSettings.Instance.SetContinuePreviousGame(continue_game);
    }
    public void ExitAfterWon()
    {
        GameSettings.Instance.SetExitAfetWon(true);
    }
}