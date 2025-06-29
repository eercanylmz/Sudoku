using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public Text textColock;
    void Start()
    {
        textColock.text = Clock.instance.GetCurrentTimeText().text;
    } 
}
