using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Active Stuff :) ")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button myButton;
    private int starsActive;
    //public Text levelText;

[Header("Level UI")]
    public Image[] stars;
    public Text levelText;
    public int level;
    public GameObject confirmPanel;
    
    //private GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        //gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        LoadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    void LoadData()
    {
       /* // is GameData presen?
        if(gameData != null)
        {
            //Decide if the level is active
            if(gameData.saveData.isActive[level -1])
            {
                isActive = true;
            }else
            {
                isActive = false;
            }
            // Decide how many stars to activate
            starsActive = gameData.saveData.stars[level - 1];
        }*/
    }

    void ActivateStars(){
        
        for (int i = 0; i < starsActive; i ++)
        {
            stars[i].enabled = false;
        }
    }

    void DecideSprite(){
        if(isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        }else{
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    void ShowLevel()
    {
        levelText.text = ""+ level;
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfirmPanel(int level)
    {
        confirmPanel.GetComponent<confirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
}