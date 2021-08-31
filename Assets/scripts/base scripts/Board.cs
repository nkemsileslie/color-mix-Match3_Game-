using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
    wait,
    move,
    win,
    Lose,
    pause
}

public enum TileKind{
    Breakable ,
    Blank,
    Normal
}

[System.Serializable]

public class TileType{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    [Header("Scriptable Object Stuff")]
    public World world;
    public int level;
    public GameState currentState = GameState.move;
    [Header("Board Dimensions")]
    public int width;
    public int height;
    public int offSet;
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject[] dots;
    public GameObject DestroyEffect;
    private bool[, ] BlankSpaces;
    private BackgroundTile [, ] BreakableTile;
    public GameObject[, ] allDots;
    public Dot currentDot;
    [Header("Layout :) ")]
    public TileType[] boardlayout;
    private FindMatches findMatches;
    public int basePieceValue = 20;
    private int StreakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;


    private void Awake(){
        if(PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if(world != null){
            if(level < world.levels.Length){
            if(world.levels[level] != null){
                width = world.levels[level].width;
                height = world.levels[level].height;
                dots = world.levels[level].dots;
                scoreGoals = world.levels[level].scoreGoals;
                boardlayout = world.levels[level].boardLayout;
            }
          }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        BreakableTile = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        BlankSpaces = new bool [width, height]; 
        allDots = new GameObject[width, height];
        SetUp(); 
        currentState = GameState.move;
    }

    public void GenerateBlankspaces(){
        for(int i = 0; i < boardlayout.Length; i ++){
            if(boardlayout[i].tileKind == TileKind.Blank){
                BlankSpaces[boardlayout[i].x, boardlayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTile(){
        //Look at all the tiles in the layout 
        for(int i = 0; i < boardlayout.Length; i ++){
            // if a tile is a "jelly" tile
             if(boardlayout[i].tileKind == TileKind.Breakable){
                // Create a "jelly" tile at that position
                Vector2 tempPosition = new Vector2 (boardlayout[i].x, boardlayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                BreakableTile[boardlayout[i].x, boardlayout[i].y] = tile.GetComponent<BackgroundTile>();
             }
        }
    }

   private void SetUp(){
       GenerateBlankspaces();
       GenerateBreakableTile();
       for(int i = 0; i < width; i ++){
           for(int j = 0; j < height; j ++){
               if(!BlankSpaces[i, j]){
               Vector2 tempPosition = new Vector2(i, j + offSet);
               Vector2 tilePosition = new Vector2(i, j);
              GameObject backgroundTile =  Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
              backgroundTile.transform.parent = this.transform;
              backgroundTile.name = "(" + i + " , " + j + " )";
              int dotToUse = Random.Range(0, dots.Length);
             
            // Using  a while loop to check if their are any matches at that place then chose a diffrernt dot
              int maxIteration = 0;
              while(MatchesAt(i, j, dots[dotToUse]) && maxIteration < 100){
                dotToUse = Random.Range(0, dots.Length);
                maxIteration++;
              }
              //End of Loop
              maxIteration = 0;
             // Debug.Log(maxIteration);

              GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
              dot.GetComponent<Dot>().row = j;
              dot.GetComponent<Dot>().column = i;

              dot.transform.parent = this.transform;
              dot.name = "(" + i + " , " + j + " )";
              allDots[i, j] = dot;
            }
          }
       }
   }

    private bool MatchesAt(int column, int row, GameObject piece){
        if(column > 1 && row > 1){
            if(allDots[column -1, row] != null && allDots[column -2, row] != null)
            {
            if(allDots[column -1, row].tag == piece.tag && allDots[column -2, row].tag == piece.tag){
                return true;
                }
            }

            if(allDots[column, row - 1] != null && allDots[column, row -2] != null){
            if(allDots[column, row -1].tag == piece.tag && allDots[column, row -2].tag == piece.tag){
                return true;
                }
            }

        }else if(column <= 1 || row <= 1){
            if(row > 1){
                if(allDots[column, row - 1] != null && allDots[column, row -2] != null){
                if(allDots[column, row -1].tag == piece.tag && allDots[column, row -2].tag == piece.tag){
                    return true;
                    }
                }
            }

            if(column > 1){
                if(allDots[column -1, row] != null && allDots[column -2, row] != null){
                if(allDots[column -1, row].tag == piece.tag && allDots[column -2, row].tag == piece.tag){
                    return true;
                    }
                }
            }
        }

        return false;
    }

    private bool ColumnOrRow(){
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if(firstPiece != null)
        {
         foreach(GameObject currentPiece in findMatches.currentMatches)
         {
            Dot dot = currentPiece.GetComponent<Dot>();
            if(dot.row == firstPiece.row){
                numberHorizontal++;

            }
            if(dot.column == firstPiece.column){
                numberVertical++;
            }
         }
        }
        return(numberVertical == 5 || numberHorizontal ==5);
    }

    private void CheckToMakeBombs(){
        if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7 ){
             findMatches.CheckBombs();
        }
        if(findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8 ){
             if(ColumnOrRow()){
                //make a color bomb
                // check if it is (current dot matached)
                if(currentDot != null){
                    if(currentDot.isMatched){
                        if(!currentDot.isColorBomb){
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }else{
                        if(currentDot.otherDot != null){
                            Dot otherdot = currentDot.otherDot.GetComponent<Dot>();
                            if(otherdot.isMatched){
                                if(!otherdot.isColorBomb){
                                    otherdot.isMatched = false;
                                    otherdot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
         else
            {
            //make a adjacent bomb
                 if(currentDot != null){
                    if(currentDot.isMatched){
                        if(!currentDot.IsAdjacentBomb){
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }else{
                        if(currentDot.otherDot != null){
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if(otherDot.isMatched){
                                if(!otherDot.IsAdjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
                
            }
        }
    }

    private void DestroyMatchesAt(int column, int row){
        if(allDots[column, row].GetComponent<Dot>().isMatched){
            //How many elements are in the matched pieces list from findmatches
            
            if(findMatches.currentMatches.Count >= 4)
            {
               CheckToMakeBombs();
            }
            //Does a tile need to breake
            if(BreakableTile[column, row] != null){
                // if it does give one damage
                BreakableTile[column, row].TakeDamage(1);
                if(BreakableTile[column, row].hitPoint <= 0){
                    BreakableTile[column, row] = null;
                }
            }

            if(goalManager != null){
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }
           // Does the sound manager exist
           if(soundManager != null){
               soundManager.PlayRandomDestroyNoise();
           }
            GameObject particle = Instantiate(DestroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * StreakValue);
            allDots[column, row] = null;
        }
    }  

    public void DestroyMatches(){
        for(int i = 0; i < width; i ++){
            for(int j = 0; j < height; j ++)
            {
                if(allDots[i, j] != null){
                    DestroyMatchesAt(i, j);
                }
            }
        }
        //findMatches.currentMatches.Clear;
        StartCoroutine(DecreaseRowCo2());
    }

    private IEnumerator DecreaseRowCo2(){
        for(int i = 0; i < width; i ++){
            for(int j =0; j < height; j ++){
                // if the current spot isnt blank nd empty then.............('_')
                if(!BlankSpaces[i, j] && allDots[i, j] == null){
                    //Loop from the space above to the top of the colunm
                    for(int k = j + 1; k < height; k ++){
                        //if the dot is found
                        if(allDots[i, k] != null){
                            // move the dot to thiss empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //set that spot to be null
                            allDots[i, k] = null;
                            //then break out of the loop
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());    
        }

    private IEnumerator DecreaseRowCo(){
        int nullCount = 0;
        for(int i = 0; i < width; i ++){
            for(int j = 0; j < height; j ++){
                if(allDots[i, j] == null){
                    nullCount ++;
                }else if(nullCount > 0){
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard(){
        for(int i = 0; i < width; i ++){
            for(int j = 0; j < height; j++){
                if(allDots[i, j] == null && !BlankSpaces[i, j]){
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIteration = 0;

                    while(MatchesAt(i, j, dots[dotToUse]) && maxIteration < 100)
                    {
                        maxIteration++;
                        dotToUse = Random.Range(0, dots.Length);
                    }

                    maxIteration = 0;

                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    // ^ this checks all of the pieces every column nd row and also checks if its null
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;

                }
            }
        }
    }

    private bool MatchesOnBoard(){
        for(int i =0; i < width; i ++){
            for(int j = 0; j < height; j ++){
                if(allDots[i, j] != null){
                    if(allDots[i, j].GetComponent<Dot>().isMatched){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo(){
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        
         while(MatchesOnBoard()){
             StreakValue ++;
             DestroyMatches();
            yield return new WaitForSeconds(2 * refillDelay);
            
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(refillDelay);
        
        if(IsDeadLocked()){
            ShuffleBoard();
            //Debug.Log("Deadlocked! ! !");   
        }
        currentState = GameState.move;
        StreakValue = 1;

    }

    private void SwitchPieces(int column, int row, Vector2 direction){
        //Take the second Piece and save ot in a folder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //I am switching the first to be second position
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row]; 
        //set the first dot to be the second dot
        allDots[column, row] = holder;
    }

    private bool CheckforMatches(){
        for(int i = 0; i < width; i ++){
            for(int j = 0; j < height; j ++){
                if(allDots[i, j] != null){
                    //Make sure that one and two to the right are in the board
                    if(i < width - 2){
                    // Check if the dots to the right and two to your right exist
                    if(allDots[i + 1, j] != null && allDots[i + 2, j] != null){
                        if(allDots[i + 1, j].tag == allDots[i, j].tag 
                        && allDots[i + 2, j].tag == allDots[i, j].tag){
                            return true;
                        }
                    }
                }

                if(j < height -2){
                    //Check if the dot above exist
                    if(allDots[i, j + 1] != null && allDots[i, j + 2] != null){
                        if(allDots[i, j + 1].tag == allDots[i, j].tag 
                        && allDots[i, j + 2].tag == allDots[i, j].tag){
                            return true;
                        }
                    }
                }
             }
        }
    }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction){
        SwitchPieces(column, row, direction);
        if(CheckforMatches()){
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    public bool IsDeadLocked(){
        for(int i = 0; i < width; i ++){
            for(int j = 0; j < height; j ++){
                if(allDots[i, j]!= null){
                    if(i < width -1){
                       if(SwitchAndCheck(i, j, Vector2.right)){
                           return false;
                       }
                    }
                    if(j < height - 1){
                        if(SwitchAndCheck(i, j, Vector2.up)){
                            return false;
                        }
                    }
                }
            }
        }        
        return true;
    }

    private void ShuffleBoard(){
       //Create alist off game objects
       List<GameObject> newBoard = new List<GameObject>();
       //Add every piece to this list
       for(int i =0; i < width; i ++){
           for(int j = 0; j < height; j ++){
               newBoard.Add(allDots[i, j]);
           }
        }
        //for every spot on the board. . .
        for(int i = 0; i < width; i ++){
            for(int j = 0; j < height; j ++){
                // if this spot shouldnt be blank
                if(!BlankSpaces[i, j]){
                    // Pick a random integer
                    int pieceToUse = Random.Range(0, newBoard.Count);
                   
                    //assign the colunm and row to the piece

                     int maxIteration = 0;
              while(MatchesAt(i, j, newBoard[pieceToUse]) && maxIteration < 100){
                pieceToUse = Random.Range(0, newBoard.Count);
                maxIteration++;
              }
              //End of Loop
              maxIteration = 0;
              Debug.Log(maxIteration);

               // make a container for the piece
                   
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    
                    piece.column = j;
                    piece.row = i;
                    //fill in the dots arry with this new piece
                    allDots[i, j] = newBoard[pieceToUse];
                    //Remove it from the list
                    newBoard.Remove(newBoard[pieceToUse]);
                   
                }
            }
        }
        //check if the board is still deadlocked
        if(IsDeadLocked()){
            ShuffleBoard();
        }
    }
    
    
}