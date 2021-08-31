using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    //----------------------------------------------------------
    private EndGameManager endGameManager;
    public bool isMatched = false;
    private HintManager hintManager;
    private FindMatches findMatches;
    private Board board;
    public GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    //-------------------------------------------------------------
    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;
    // Start is called before the first frame update
    [Header("powerup stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool IsAdjacentBomb;
    public GameObject adjacentMarker;
    public GameObject RowArrow;
    public GameObject ColumnArrow;
    public GameObject colorBomb;
    void Start()
    {   

        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        IsAdjacentBomb = false;
        
        endGameManager = FindObjectOfType<EndGameManager>();
        hintManager = FindObjectOfType<HintManager>();
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //targetX = (int)transform .position.x;
        //targetY = (int)transform .position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
        
    }
    //This is for testing and debug only.
    private void OnMouseOver(){
        if(Input.GetMouseButtonDown(1)){
            IsAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        /*
        if(isMatched){
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .4f);
        }
        */
      targetX = column;
      targetY = row;  
        if(Mathf.Abs(targetX - transform.position.x) > .1){
          // MOve towards the targets
          tempPosition = new Vector2(targetX, transform.position.y);
          transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
          if(board.allDots[column, row] != this.gameObject){
            board.allDots[column, row] = this.gameObject;
          }
            findMatches.FindAllMatches();


        }else{
        //Directlty set the position
        tempPosition = new Vector2(targetX, transform.position.y);
        transform.position = tempPosition;
        board.allDots[column, row] = this.gameObject;
        }
        if(Mathf.Abs(targetY - transform.position.y) > .1){
          // MOve towards the targets
          tempPosition = new Vector2(transform.position.x, targetY);
          transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
           if(board.allDots[column, row] != this.gameObject){
              board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();

        }else{
        //Directlty set the position
        tempPosition = new Vector2(transform.position.x, targetY);
        transform.position = tempPosition;
        
        }
    }

    public IEnumerator CheckMoveCo(){
        if(isColorBomb){
            // This piece is a color bomb and the other piece is the color to destroy
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }else if(otherDot.GetComponent<Dot>().isColorBomb){
            //the othe piee is the color bomb tis piece has the color to destroy
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if(otherDot != null){
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched){
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }else{
                if(endGameManager != null)
                {
                    if(endGameManager.requirements.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }
            board.DestroyMatches();
            }
           // otherDot = null;
        }
    }

    private void OnMouseDown(){
        // Destroy the hint
        if(hintManager != null){
            hintManager.DestroyHint();
        }
        if(board.currentState == GameState.move){
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(firstTouchPosition);
        }
    }
    private void OnMouseUp()
    {
        if(board.currentState == GameState.move){
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();   
        }
    }

    void CalculateAngle(){
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y)> swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x)> swipeResist)
        {
            board.currentState = GameState.wait;
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 100 / Mathf.PI;
        
       // Debug.Log(swipeAngle);
       MovePieces();
      
       board.currentDot = this;

        }else{
            board.currentState = GameState.move;
        }
    }

    void MovePiecesActual(Vector2 direction){
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if(otherDot != null){
        otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
        otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
        column += (int)direction.x;
        row += (int)direction.y;
        StartCoroutine(CheckMoveCo());
        }else{
            board.currentState = GameState.move;
        }

    }

    void MovePieces(){
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width -1){
            //right swipe
            /* 
            otherDot = board.allDots[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
            StartCoroutine(CheckMoveCo());
            */
            MovePiecesActual(Vector2.right);

        }else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height -1){
            //Up swipe
            /*
            otherDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
            StartCoroutine(CheckMoveCo());
            */
            MovePiecesActual(Vector2.up);

        }else if((swipeAngle > 135 || swipeAngle <= -135) && column > 0){
            //left swipe
            /*
            otherDot = board.allDots[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
            StartCoroutine(CheckMoveCo());
            */
             MovePiecesActual(Vector2.left);

        }else if(swipeAngle < -45 && swipeAngle >= -135 && row > 0){
            //down swipe
            /*
            otherDot = board.allDots[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
            StartCoroutine(CheckMoveCo());
            */
             MovePiecesActual(Vector2.down);
        
        }else{
        board.currentState = GameState.move;
        }
        
    }
    void FindMatches(){
        if(column > 0 && column < board.width - 1){
            GameObject leftDot1 = board.allDots[column -1, row];
            GameObject rightDot1 = board.allDots[column +1, row];
            if(leftDot1 != null && rightDot1 != null){
                if( leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag){
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }

         if(row > 0 && row < board.height - 1){
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row -1];
            if(upDot1 != null && downDot1 != null){
                if( upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag){
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(RowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = transform;
    }

    public void MakeColumnBomb(){
        isColumnBomb = true;
        GameObject arrow = Instantiate(ColumnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = transform;
    }
     public void MakeColorBomb(){
        isColorBomb = true;
        GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
        color.transform.parent = this.transform;
        this.gameObject.tag = "Color";
    }

    public void MakeAdjacentBomb(){
        IsAdjacentBomb = true;
        GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
        marker.transform.parent = transform;
    }
}
