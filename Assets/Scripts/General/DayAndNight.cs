using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class DayAndNight : MonoBehaviour
{
    public TextMeshProUGUI timeDisplay;
    public Volume ppv;

    public float tick = 60f; // controls how fast in-game time moves
    public float seconds;
    public int mins;
    public int hours;

    public bool activateLights;
    public GameObject[] lights;
    public SpriteRenderer[] stars;

    [Header("Transition Settings")]
    public float transitionSpeed = 0.05f; // increase if it’s too slow
    public float dayStart = 6f;
    public float nightStart = 18f;

    void Start()
    {
        if (ppv == null)
            ppv = GetComponent<Volume>();
    }

    void FixedUpdate()
    {
        CalcTime();
        DisplayTime();
        UpdateLighting();
    }

    void CalcTime()
    {
        seconds += Time.fixedDeltaTime * tick;

        if (seconds >= 60)
        {
            seconds = 0;
            mins++;
        }

        if (mins >= 60)
        {
            mins = 0;
            hours++;
        }

        if (hours >= 24)
        {
            hours = 0;
        }
    }

    void UpdateLighting()
    {
        
        if (hours >= nightStart || hours < dayStart)
        {
            // Gradually increase weight up to 1
            ppv.weight = Mathf.MoveTowards(ppv.weight, 1f, Time.deltaTime * transitionSpeed);

            foreach (var s in stars)
            {
                float newAlpha = Mathf.MoveTowards(s.color.a, 1f, Time.deltaTime * transitionSpeed);
                s.color = new Color(s.color.r, s.color.g, s.color.b, newAlpha);
            }

            if (!activateLights && ppv.weight > 0.5f)
            {
                foreach (var l in lights)
                    l.SetActive(true);
                activateLights = true;
            }
        }
        
        else
        {
            // Gradually decrease weight down to 0
            ppv.weight = Mathf.MoveTowards(ppv.weight, 0f, Time.deltaTime * transitionSpeed);

            foreach (var s in stars)
            {
                float newAlpha = Mathf.MoveTowards(s.color.a, 0f, Time.deltaTime * transitionSpeed);
                s.color = new Color(s.color.r, s.color.g, s.color.b, newAlpha);
            }

            if (activateLights && ppv.weight < 0.3f)
            {
                foreach (var l in lights)
                    l.SetActive(false);
                activateLights = false;
            }
        }
    }

    void DisplayTime()
    {
        timeDisplay.text = string.Format("{0:00}:{1:00}", hours, mins);
    }
}
