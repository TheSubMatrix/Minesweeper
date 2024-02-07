using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isBomb;

    bool gameActive = false;

    public Vector2Int positionOnBoard;

    public Board board;

    [SerializeField] 
    Sprite[] tileSprites = new Sprite[13];

    //I know it's dumb but it kinda saves time in this case
    public enum tileState
    {
        Empty = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Covered = 9,
        Bomb = 10,
        Flag = 11,
        Red = 12,
    }
    [SerializeField] private tileState _currentState;
    private tileState CurrentState
    {
        get
        { 
            return _currentState; 
        }
        set
        {
            if(GetComponent<SpriteRenderer>() != null && tileSprites[(int)value] != null)
            {
                GetComponent<SpriteRenderer>().sprite = tileSprites[(int)value];
            }
            else
            {
                Debug.LogWarning("No sprite found for state");
            }
            _currentState = value;
        }
    }
    public void Start()
    {
        _currentState = tileState.Covered;
        if(board != null)
        {
            board.OnGameLost += OnGameLost;
            gameActive = true;
        }
    }
    public void UncoverTile()
    {
        if(board != null && gameActive)
        {
            if (!isBomb)
            {
                int foundNeighboringBombs = 0;
                List<Tile> neighboringTiles = board.GetNeighbors(positionOnBoard);
                foreach (Tile tile in neighboringTiles)
                {
                    if (tile.isBomb)
                    {
                        foundNeighboringBombs++;
                    }
                }
                CurrentState = (tileState)foundNeighboringBombs;
                board.tilesNeededToClear--;
                board.OnBoardSquaresUpdated?.Invoke(board.tilesNeededToClear);
                board.CheckWin();
                if(CurrentState == tileState.Empty)
                {
                    foreach(Tile tile in neighboringTiles)
                    {
                        if(tile.CurrentState == tileState.Covered)
                        {
                            tile.UncoverTile();
                        }
                    }
                }
            }
            else
            {
                CurrentState = tileState.Red;
                board.OnGameLost?.Invoke();
            }
        }
    }
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && CurrentState == tileState.Covered)
        {
            UncoverTile();
        }
        else if (Input.GetMouseButtonDown(1) && gameActive)
        {
            if (CurrentState == tileState.Covered)
            {
                CurrentState = tileState.Flag;
            }
            else if (CurrentState == tileState.Flag)
            {
                CurrentState = tileState.Covered;
            }
        }
    }
    void OnGameLost()
    {
        if(isBomb && (CurrentState == tileState.Covered || CurrentState == tileState.Flag))
        {
            CurrentState = tileState.Bomb;
        }
        gameActive = false;
    }
}
