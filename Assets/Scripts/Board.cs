using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public delegate void GameWon();
    public GameWon OnGameWon;
    public delegate void GameLost();
    public GameLost OnGameLost;
    public delegate void BoardSquaresUpdated(uint remainingSquares);
    public BoardSquaresUpdated OnBoardSquaresUpdated;

    [SerializeField] private Grid grid;
    [SerializeField] const uint gridSizeX = 15;
    [SerializeField] const uint gridSizeY = 15;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] uint bombsToPlace = 25;
    public uint tilesNeededToClear;
    private Tile[,] board = new Tile[gridSizeX, gridSizeY];

    void Awake()
    {
        tilesNeededToClear = gridSizeX* gridSizeY - bombsToPlace;
        if (grid == null && GetComponent<Grid>() != null)
        {
            grid = GetComponent<Grid>();
        }
        if(grid != null)
        {
            //Center Grid on screen
            gameObject.transform.position = new Vector2(-(grid.cellSize.x * gridSizeX / 2), -(grid.cellSize.y * gridSizeY / 2));
            FillGrid(gridSizeX, gridSizeY);
        }
    }
    private void Start()
    {
        OnBoardSquaresUpdated?.Invoke(tilesNeededToClear);
    }
    void FillGrid(uint width, uint height)
    {
        List<Vector2Int> validBombLocations = new List<Vector2Int>();
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, grid.GetCellCenterWorld(new Vector3Int((int)x, (int)y)), Quaternion.identity);
                if (newTile.GetComponent<Tile>() != null)
                {
                    board[x, y] = newTile.GetComponent<Tile>();
                    board[x, y].positionOnBoard = new Vector2Int(x, y);
                    board[x, y].board = this;
                    validBombLocations.Add(new Vector2Int(x, y));
                }
            }
        }
        Shuffle(validBombLocations);
        //not the most efficient way of accomplishing this task as the complexity is not needed 
        while(validBombLocations.Count > 0 && bombsToPlace > 0)
        {
            if (!board[validBombLocations[0].x, validBombLocations[0].y].isBomb)
            {
                board[validBombLocations[0].x, validBombLocations[0].y].isBomb = true;
                bombsToPlace--;
            }
            validBombLocations.RemoveAt(0);
        }

    }
    public List<Tile> GetNeighbors(Vector2Int boardPosition)
    {
        List<Tile> tilesToReturn = new List<Tile>();
        for(int xToCheck = boardPosition.x - 1; xToCheck <= boardPosition.x + 1; xToCheck++)
        {
            for (int yToCheck = boardPosition.y - 1; yToCheck <= boardPosition.y + 1; yToCheck++)
            {
                if(CheckValidGridPosition(xToCheck, yToCheck) && !(xToCheck == boardPosition.x && yToCheck == boardPosition.y))
                {
                    tilesToReturn.Add(board[xToCheck, yToCheck]);
                }
            }
        }
        return tilesToReturn;
    }
    bool CheckValidGridPosition(int xPosition, int yPosition)
    {
        if(xPosition >= 0 && xPosition < gridSizeX && yPosition >= 0 && yPosition < gridSizeY)
        {
            return true;
        }
        return false;
    }
    void Shuffle<T>(List<T> listToShuffle)
    {
        for(int i = 0; i < listToShuffle.Count-1; i++)
        {
            T ValueToShuffle = listToShuffle[i];
            int newIndex = UnityEngine.Random.Range(i, listToShuffle.Count);
            listToShuffle[i] = listToShuffle[newIndex];
            listToShuffle[newIndex] = ValueToShuffle;
        }
    }
    public void CheckWin()
    {
        if(tilesNeededToClear <= 0)
        {
            OnGameWon?.Invoke();
        }
    }
}