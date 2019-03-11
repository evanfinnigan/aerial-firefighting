using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkyManager : MonoBehaviour {

    public Material skymaterial;

    public Text time;

    public Light sceneLight;

    // game starts at noon
    float minutes = 0f;

    Color midnight = new Color(14f/255f, 18f/255f, 44f/255f, 128f/255f);
    Color daybreak = new Color(199f/255f, 112f/255f, 72f/255f, 128f/255f);
    Color midday = new Color(91f / 255f, 209f / 255f, 236f/255f, 128f/255f);
    Color sunset = new Color(173f / 255f, 57f / 255f, 57f / 255f, 128f / 255f);
    //Color evening = new Color(40f / 255f, 79f / 255f, 98f / 255f, 128f / 255f);

    float timeTotal;

    // Use this for initialization
    void Awake () {
        minutes = Random.Range(0f, 59f);

        RenderSettings.skybox = skymaterial;

        // Update sky every 30 seconds
        InvokeRepeating("UpdateSky", 0, 30);
    }

    private void Update()
    {
        minutes += Time.deltaTime / 60f;
        timeTotal += Time.deltaTime;
        if (minutes >= 60f) minutes = 0f;

        UpdateTime();
    }

    public float GetTimePassed()
    {
        return timeTotal;
    }

    void UpdateTime()
    {
        float h = Mathf.Lerp(0f, 23f, minutes / 60f);

        string am_pm = h < 12f ? "am" : "pm";
        float m = h - Mathf.Floor(h);
        m = Mathf.Lerp(0f, 59f, m / 1f);

        time.text = string.Concat(((int)h % 12 == 0 ? 12 : (int)h % 12), ":", ((int)m).ToString("D2"), " ", am_pm);
    } 

    void UpdateSky()
    {
        // night
        if (0f <= minutes && minutes < 9f)
        {
            float lerpVal = (float)minutes / 9f;
            skymaterial.SetColor("_Tint", Color.Lerp(midnight, midnight, lerpVal));
            sceneLight.intensity = 0.3f;
        }
        // early morning: midnight-->daybreak
        if (9f <= minutes && minutes < 15f)
        {
            float lerpVal = (float)(minutes - 9f) / 6f;
            skymaterial.SetColor("_Tint", Color.Lerp(midnight,daybreak,lerpVal));
            sceneLight.intensity = Mathf.Lerp(0.3f,0.55f, lerpVal);
        }
        // mid-late morning: daybreak-->midday
        else if ( 15f <= minutes && minutes < 30f)
        {
            float lerpVal = (float)(minutes - 15f) / 15f;
            skymaterial.SetColor("_Tint", Color.Lerp(daybreak, midday, lerpVal));
            sceneLight.intensity = Mathf.Lerp(0.55f, 0.7f, lerpVal);
        }
        // afternoon: midday
        else if ( 30f <= minutes && minutes < 44f)
        {
            float lerpVal = (float)(minutes - 30f) / 14f;
            skymaterial.SetColor("_Tint", Color.Lerp(midday, midday, lerpVal));
            sceneLight.intensity = Mathf.Lerp(0.7f, 0.7f, lerpVal);
        }
        // late afternoon: midday --> sunset
        else if (44f <= minutes && minutes < 52f)
        {
            float lerpVal = (float)(minutes - 44f) / 8f;
            skymaterial.SetColor("_Tint", Color.Lerp(midday, sunset, lerpVal));
            sceneLight.intensity = Mathf.Lerp(0.7f, 0.5f, lerpVal);
        }
        // evening
        else if (52f <= minutes)
        {
            float lerpVal = (float)(minutes - 52f) / 7f;
            skymaterial.SetColor("_Tint", Color.Lerp(sunset, midnight, lerpVal));
            sceneLight.intensity = Mathf.Lerp(0.5f, 0.3f, lerpVal);
        }

        sceneLight.color = skymaterial.GetColor("_Tint");
    }
}
