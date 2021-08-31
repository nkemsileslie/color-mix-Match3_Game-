using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "level")]
public class level : ScriptableObject
{
    [Header("Board Dimnsion")]
   public int width;
   public int height;

   [Header("Starting Tiles")]
   public TileType[] boardLayout;

   [Header("Available Dots")]
   public GameObject[] dots;
   [Header("Score Goals")]
   public int[] scoreGoals;
   [Header("End Game Requirements")]
   public EndGameRequirements endGameRequirements;
   public BlankGoal[] levelGoals;
}
