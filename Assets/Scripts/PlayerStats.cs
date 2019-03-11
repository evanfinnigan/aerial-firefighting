using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDataComparer : IComparer<ScoreData>
{
    int IComparer<ScoreData>.Compare(ScoreData x, ScoreData y)
    {
        return y.highScore - x.highScore;
    }
}

public class PlayerStats : MonoBehaviour {

    public Canvas leaderboard;

    public Transform textHolder;
    public GameObject textPrefab;

    private void Awake()
    {
        // populate list with saved data
        string s = PlayerPrefs.GetString("stats");
        if (!s.Equals(""))
        {
            ScoreDataList<ScoreData> leaderboards = JsonUtility.FromJson<ScoreDataList<ScoreData>>(s);
            List<ScoreData> scores = new List<ScoreData>(leaderboards.items);
            scores.Sort(new ScoreDataComparer());
            foreach (ScoreData i in scores)
            {
                if (!i.name.Equals(""))
                {
                    GameObject g = Instantiate(textPrefab, textHolder);
                    Text t = g.GetComponent<Text>();
                    t.text = string.Format("{0}\nHigh Score: {1}\nTrees Saved: {2}\nTowns Saved: {3}\nFastest Time: {4} minutes\nTime Flown: {5} minutes", i.name, i.highScore, i.treesSaved, i.peopleSaved, i.fastestTime, i.totalMinutesFlown);
                }
            }
        }
        else
        {
            GameObject g = Instantiate(textPrefab, textHolder);
            Text t = g.GetComponent<Text>();
            t.text = "No Scores Yet!";
        }
    }

    public void Btn_Back()
    {
        leaderboard.enabled = false;
    }

}
