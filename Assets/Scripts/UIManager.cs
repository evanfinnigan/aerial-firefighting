using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour {

    public AudioLowPassFilter filter;

    public Canvas startMenu;
    public InputField nameInput;

    public Canvas controls;

    public Canvas stats;

    public Canvas hud;
    //public Text hudTimer;

    public Canvas winScreen;
    public Text treeText;
    public Text peopleText;
    public Text timeText;
    public Text totalScore;

    public GameObject resetButton;

    public MapTileGrid map;

    public Toggle showControls;

    public Text endGameText;
    public Button endGameButton;

    bool gameEnded = false;

    private void Awake()
    {
        Time.timeScale = 0; // game isn't running at the beginning

        if (!PlayerPrefs.GetString("name").Equals(""))
        {
            nameInput.text = PlayerPrefs.GetString("name");
        }
        else
        {
            nameInput.text = "Human";
        }

        if (PlayerPrefs.GetInt("ShowControls", 1) == 1)
        {
            showControls.isOn = true;
        }
        else
        {
            showControls.isOn = false;
        }

        InvokeRepeating("CheckForGameEnd", 10f, 5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void CheckForGameEnd()
    {
        if (map.GetTilesInState(MapTile.TileState.burning) == 0)
        {

            if (map.GetTilesInState(MapTile.TileState.dead) == map.GetTilesOfType(MapTile.TileType.forest) + map.GetTilesOfType(MapTile.TileType.town))
            {
                endGameText.text = "FAILURE";
            }

            endGameButton.gameObject.SetActive(true);
            endGameText.gameObject.SetActive(true);
        }
    }

    public void EndGame()
    {
        gameEnded = true;

        Time.timeScale = 0;
        hud.enabled = false;
        FadeOut();

        // 9 trees per forest tile
        int treesSaved = 9*map.GetTilesInState(MapTile.TileState.healthy, MapTile.TileType.forest);
        int treesTotal = 9*map.GetTilesOfType(MapTile.TileType.forest);

        int townsSaved = map.GetTilesInState(MapTile.TileState.healthy, MapTile.TileType.town);
        int townsTotal = map.GetTilesOfType(MapTile.TileType.town);

        int minutesPassed = (int)FindObjectOfType<SkyManager>().GetTimePassed();

        treeText.text = string.Format("Trees saved: {0}/{1}", treesSaved, treesTotal, treesSaved, treesSaved*100);
        peopleText.text = string.Format("Towns saved: {0}/{1}", townsSaved, townsTotal, townsSaved, townsSaved * 5000);
        timeText.text = string.Format("Time: {0} minutes", minutesPassed);

        int score = treesSaved * 100 + townsSaved * 5000;
        totalScore.text = string.Format("Score: {0} points", score);

        winScreen.enabled = true;

        // Save Scores
        string playerName = PlayerPrefs.GetString("name", "Human");
        string statstr = PlayerPrefs.GetString("stats", "");
        if (!statstr.Equals(""))
        {
            ScoreDataList<ScoreData> scoreData = JsonUtility.FromJson<ScoreDataList<ScoreData>>(statstr);
            List<ScoreData> items = new List<ScoreData>(scoreData.items);

            ScoreData newScore = new ScoreData();

            foreach (ScoreData item in items.ToArray())
            {
                if (item.name.Equals(playerName))
                {
                    newScore = item;
                    items.Remove(item);
                }
            }


            newScore.name = playerName;
            newScore.highScore = score > newScore.highScore ? score : newScore.highScore;
            newScore.fastestTime = (minutesPassed < newScore.fastestTime || newScore.fastestTime == 0) ? minutesPassed : newScore.fastestTime;
            newScore.totalMinutesFlown += minutesPassed;
            newScore.treesSaved += 9*treesSaved;
            newScore.peopleSaved += townsSaved;

            items.Add(newScore);
            scoreData.items = items.ToArray();

            string serialized = JsonUtility.ToJson(scoreData);

            Debug.LogFormat("saving scores: {0}", serialized);

            PlayerPrefs.SetString("stats", serialized);
            PlayerPrefs.Save();
        }
        else
        {
            List<ScoreData> items = new List<ScoreData>();

            ScoreData newScore = new ScoreData();

            newScore.name = playerName;
            newScore.highScore = score > newScore.highScore ? score : newScore.highScore;
            newScore.fastestTime = (minutesPassed < newScore.fastestTime || newScore.fastestTime == 0) ? minutesPassed : newScore.fastestTime;
            newScore.totalMinutesFlown += minutesPassed;
            newScore.treesSaved += 9 * treesSaved;
            newScore.peopleSaved += townsSaved;

            items.Add(newScore);
            ScoreDataList<ScoreData> scoreData = new ScoreDataList<ScoreData>();
            scoreData.items = items.ToArray();

            string serialized = JsonUtility.ToJson(scoreData);

            Debug.LogFormat("saving scores: {0}", serialized);

            PlayerPrefs.SetString("stats", serialized);
            PlayerPrefs.Save();
        }
        
    }

    public void SetName(string input)
    {
        PlayerPrefs.SetString("name", input);
        PlayerPrefs.Save();
    }

    public void Btn_Start()
    {
        nameInput.interactable = false;
        
        if (showControls.isOn)
        {
            ShowControls();
        }
        else
        {
            StartGame();
        }
    }

    public void ShowControls()
    {
        StartCoroutine(ShowControlsCo());
    }

    public IEnumerator ShowControlsCo()
    {
        startMenu.enabled = false;
        controls.enabled = true;

        yield return new WaitForSecondsRealtime(4f);

        StartGame();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        startMenu.enabled = false;
        controls.enabled = false;
        resetButton.SetActive(true); // can't reset before the game actually starts
        hud.enabled = true;
        FadeIn();
    }

    public void OnShowControlsChanged(bool val)
    {
        PlayerPrefs.SetInt("ShowControls", val ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Btn_Stats()
    {
        stats.enabled = !stats.enabled;
    }

    public void Btn_Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }

    public void Btn_Exit()
    {
        Quit();
    }

    public void TogglePause()
    {
        startMenu.enabled = !startMenu.enabled;
        nameInput.interactable = startMenu.enabled;
        hud.enabled = !startMenu.enabled;
        Pause();
    }

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        LowPass();
    }

    public void LowPass()
    {
        if (Time.timeScale == 0)
        {
            //filter.cutoffFrequency = 800;
            FadeOut();
        }

        else
        {
            //filter.cutoffFrequency = 22000;
            FadeIn();
        }
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCo(1f));
    }

    private IEnumerator FadeInCo(float time)
    {
        float timer = 0;
        while (timer < time)
        {
            filter.cutoffFrequency = Mathf.Lerp(800, 22000, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        filter.cutoffFrequency = 22000;
    }

    public void FadeOut()
    {
        filter.cutoffFrequency = 800;
    }


    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

}
