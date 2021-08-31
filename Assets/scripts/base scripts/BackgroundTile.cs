using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{   
    private SpriteRenderer sprite;
    public int hitPoint;
    private GoalManager goalManager;
    private void Start(){
        goalManager = FindObjectOfType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update(){
        if(hitPoint <= 0){
            if(goalManager != null){
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage( int damage){
        hitPoint -= damage;
        MakeLighter();
    }

    void MakeLighter(){
        //Make the current color of breakable tile
        Color color = sprite.color;
        //Get the current colors alph value and cut it half that is to make the color lighter
        float newalpha = color.a * .5f;
        sprite.color = new Color(color.r , color.g , color.b, newalpha);

    }
  
   
}
