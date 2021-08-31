using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class confirmPanel : MonoBehaviour
{
    [Header("Level Information")]
    public string levelToLoad;
    public int level;
   // private GameData gameData;
    private int starsActive;

    [Header("UI stuff :) ")]
    public Image[] stars;
    public Text highScoreTexts;
    public Text starText;
    public int highScore;
    

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActivateStars();
        SetText();
    }

    void LoadData(){
        if(gameData!= null){
        starsActive = gameData.saveData.stars[level -1];
        highScore = gameData.saveData.highScore[level -1];
        }
    }
    void SetText(){
        highScoreTexts.text = "" + highScore;
        starText.text = "" + starText + "/3";
    }

   void ActivateStars(){
        //COME BACK TO THIS WHEN THE BINARY FILE IS DONE !! :) 
        for (int i = 0; i < starsActive; i ++)
        {
            stars[i].enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", level -1);
        SceneManager.LoadScene(levelToLoad);
    }

}
