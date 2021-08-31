using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    private Board board;
    private EndGameManager endGame;
    // Start is called before the first frame update
    void Start()
    {
        endGame = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<Board>();
        GetGoals();
        SetupGoals();
       
    }

    void GetGoals(){
        if(board != null)
        {
            if(board.world != null)
            {
                 if(board.level < board.world.levels.Length)
                 {
                if(board.world.levels[board.level] != null)
                {
                    levelGoals = board.world.levels[board.level].levelGoals;
                    for(int i = 0; i < levelGoals.Length; i ++)
                    {
                        levelGoals[i].numberCollected = 0;
                    }
                }
              }
            }
        }
    }
    void SetupGoals(){
        for(int i = 0; i < levelGoals.Length; i ++){
            // Create a new goal  panel at the goal intro parent posistion
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);
            //set the image and text ofthe goal
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            // create a new goal panel at the goalGameParent position
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            //-------------------------------------------------------------------------------------------------------------
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals(){
        int goalsCompleted = 0;
        for(int i = 0; i < levelGoals.Length; i ++){
            currentGoals[i].thisText.text = " " + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if(levelGoals[i].numberCollected >= levelGoals[i].numberNeeded){
                goalsCompleted++;
                currentGoals[i].thisText.text = " " + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }
        if(goalsCompleted >= levelGoals.Length){
            if(endGame != null){
                endGame.WinGame();
            }
            Debug.Log("you win !!!");
        }
    }

    public void CompareGoal(string goalToCompare){
        for(int i = 0; i < levelGoals.Length; i++){
            if(goalToCompare == levelGoals[i].matchValue){
                levelGoals[i].numberCollected++;
            }
        }
    }
}
