using System;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public  int hour_ = 0;
    public int minute_ = 0;
    public int second_ = 0;
    public Text textClock;
    private float delta_time;
    private bool stop_clock_ = false;

    public static Clock instance;

    private void Awake()
    {
        if (instance)
            Destroy(instance);

        instance = this;
        textClock = GetComponent<Text>();
        if (GameSettings.Instance.GetContinuePreviousGame())
            delta_time = Config.ReadGameTime();
        else
            delta_time = 0;
    }

    void Start()
    {
        stop_clock_ = false;
    }

    void Update()
    {
        if (GameSettings .Instance .GetPaused() ==false && stop_clock_ == false)
        {
            delta_time += Time.deltaTime;
            TimeSpan span = TimeSpan.FromSeconds(delta_time);

            string hour = LeadingZero(span.Hours);
            string minute = LeadingZero(span.Minutes);
            string seconds = LeadingZero(span.Seconds);

            textClock.text = hour+":"+minute +":"+seconds;


        }
    }
    string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    public void OnGameOver()
    {
        stop_clock_ = true;
    }
    private void OnEnable()
    {
        GameEvents.OnGameOver += OnGameOver;
    }
    private void OnDisable()
    {
        GameEvents.OnGameOver -= OnGameOver;
    }

    public static string GetCurrentTime()
    {
        return instance.delta_time .ToString ();
    }
    public Text GetCurrentTimeText()
    {
        return textClock;   
    }
}
