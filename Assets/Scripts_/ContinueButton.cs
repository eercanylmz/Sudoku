using System;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public Text timeText;
    public Text levelText;

    string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    private void Start()
    {
        if (Config.GameDataFileExist() == false)
        {
            gameObject.GetComponent<Button>().interactable = false;
            timeText.text = " ";
            levelText.text = " ";

        }
        else
        {
            float delta_time = Config.ReadGameTime();
            delta_time += Time.deltaTime;
            TimeSpan span = TimeSpan.FromSeconds(delta_time);


            string hour = LeadingZero(span.Hours);
            string minute = LeadingZero(span.Minutes);
            string second = LeadingZero(span.Seconds);

            timeText.text = hour + ":" + minute + ":" + second;
            levelText.text = Config.ReadBoardLevel();
        }
    }
    public void SetGameData()
    {
        GameSettings.Instance.SetGameMode(Config.ReadBoardLevel());

    }
}