using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHandler
{

    static Character[,] _battleGrid;
    static TerrainSpace[,] _terrainGrid;
    static int[,] _moveGrid;

    public static void CreateNewGrid(int numOfRows, int numOfColms)
    {
        _battleGrid = new Character[numOfRows, numOfColms];

        _moveGrid = new int[numOfRows, numOfColms];
        ResetMoveGrid();

        _terrainGrid = new TerrainSpace[numOfRows, numOfColms];
        for (int y = 0; y < _terrainGrid.GetLength(1); y++)
        {
            for (int x = 0; x < _terrainGrid.GetLength(0); x++)
            {
                _terrainGrid[x, y] = new TerrainSpace();
            }
        }
    }

    public static void PlaceEnemyOnBoard(Character newCharacter)
    {
        _battleGrid[(int)newCharacter.CurrentPosition.x, (int)newCharacter.CurrentPosition.y] = newCharacter;
    }

    public static Vector2 GetSpawn()
    {
        List<Vector2> availableSpawns = new List<Vector2>();

        for (int y = 0; y < _battleGrid.GetLength(1); y++)
        {
            for (int x = 0; x < _battleGrid.GetLength(0); x++)
            {
                if (x >= 0 && y >= 0 && !IsSpaceOccupied(x, y))
                {
                    availableSpawns.Add(new Vector2(x, y));
                }
            }
        }

        int randspot = Random.Range(0, availableSpawns.Count);

        return availableSpawns[randspot];
    }

    public static void ClearSpace(Vector2 posToClear)
    {
        //Debug.Log("space cleared");
        _battleGrid[(int)posToClear.x, (int)posToClear.y] = null;
    }

    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Weapon heldWeapon)
    {
        List<Character> enemiesInRange = new List<Character>();

        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -1; y < 1; y++)
        {
            int tileYPos = currY + y;
            for (int x = -1; x < 1; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);
                if (!heldWeapon.IsRanged && (Mathf.Abs(x) + Mathf.Abs(y)) <= heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        enemiesInRange.Add(_battleGrid[tileXPos, tileYPos]);
                    }
                }
                else if ((Mathf.Abs(x) + Mathf.Abs(y)) == heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        enemiesInRange.Add(_battleGrid[tileXPos, tileYPos]);
                    }
                }
            }
        }

        return enemiesInRange;
    }
    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Abilities ability)
    {
        List<Character> enemiesInRange = new List<Character>();

        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -1; y < 1; y++)
        {
            int tileYPos = currY + y;
            for (int x = -1; x < 1; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);
                if ((Mathf.Abs(x) + Mathf.Abs(y)) <= ability.TargetRange && IsSpaceOccupied(tileXPos, tileYPos))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        enemiesInRange.Add(_battleGrid[tileXPos, tileYPos]);
                    }
                }
            }
        }

        return enemiesInRange;
    }



    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Weapon heldWeapon)
    {
        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -1; y < 1; y++)
        {
            int tileYPos = currY + y;
            for (int x = -1; x < 1; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);
                if (!heldWeapon.IsRanged && (Mathf.Abs(x) + Mathf.Abs(y)) <= heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        return true;
                    }
                }
                else if ((Mathf.Abs(x) + Mathf.Abs(y)) == heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Weapon heldWeapon, Character target)
    {
        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -1; y < 1; y++)
        {
            int tileYPos = currY + y;
            for (int x = -1; x < 1; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);

                if (!heldWeapon.IsRanged && (Mathf.Abs(x) + Mathf.Abs(y)) <= heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        return true;
                    }
                }
                else if ((Mathf.Abs(x) + Mathf.Abs(y)) == heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        return true;
                    }
                }
                
            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Abilities ability)
    {
        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -1; y < 1; y++)
        {
            int tileYPos = currY + y;
            for (int x = -1; x < 1; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);

                if ((Mathf.Abs(x) + Mathf.Abs(y)) <= ability.TargetRange && IsSpaceOccupied(tileXPos, tileYPos))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        return true;
                    }
                }

            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Abilities ability, Character target)
    {
        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -1; y < 1; y++)
        {
            int tileYPos = currY + y;
            for (int x = -1; x < 1; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);

                if ((Mathf.Abs(x) + Mathf.Abs(y)) <= ability.TargetRange && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static void MoveEnemy(Character movingCharacter, Vector2 desiredPos)
    {
        if(movingCharacter.CurrentPosition != desiredPos)
        {
            HistoryHandler.AddToCurrentAction(movingCharacter.Name + " MOVES \n");
            ClearSpace(movingCharacter.CurrentPosition);

            movingCharacter.CurrentPosition = desiredPos;
            _battleGrid[(int)desiredPos.x, (int)desiredPos.y] = movingCharacter;
        }
        else if(movingCharacter.SpacesICanMove.Count == 1)
        {
            HistoryHandler.AddToCurrentAction(movingCharacter.Name + " CAN'T MOVE \n");
        }
        else
        {
            HistoryHandler.AddToCurrentAction(movingCharacter.Name + " STANDS IN PLACE \n");
        }
    }

    public static void SwapPlaces(Character character1, Character character2)
    {
        Vector2 temp = character1.CurrentPosition;
        _battleGrid[(int)character2.CurrentPosition.x, (int)character2.CurrentPosition.y] = character1;
        character1.CurrentPosition = character2.CurrentPosition;
        _battleGrid[(int)temp.x, (int)temp.y] = character2;
        character2.CurrentPosition = temp;
    }

    //checks to see if the space is generically occupied
    public static bool IsSpaceOccupied(int pointx, int pointy)
    {
        try
        {
            if (_battleGrid[pointx, pointy] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
            
    }
    public static bool IsSpaceOccupied(int pointx, int pointy, Character target)
    {
        try
        {
            if (_battleGrid[pointx, pointy] == target)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

    }
    public static bool IsSpaceOccupied(Vector2 checkPosition)
    {
        try
        {
            if (_battleGrid[(int)checkPosition.x, (int)checkPosition.y] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }  
    }
    public static bool IsSpaceOccupied(Vector2 checkPosition, Character target)
    {
        try
        {
            if (_battleGrid[(int)checkPosition.x, (int)checkPosition.y] == target)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

    }

    public static List<Vector2> WhereCanIMove(Vector2 startPosition, int movement)
    {
        ResetMoveGrid();
        List<Vector2> availableSpaces = new List<Vector2>();

        int startX = (int)startPosition.x;
        int startY = (int)startPosition.y;

        _moveGrid[startX, startY] = movement;

        availableSpaces.Add(startPosition);

        availableSpaces = AddTilesSurroundingMe(availableSpaces);

        return availableSpaces;    
    }
    
    static void ResetMoveGrid()
    {
        for (int y = 0; y < _moveGrid.GetLength(1); y++)
        {
            for (int x = 0; x < _moveGrid.GetLength(0); x++)
            {
                _moveGrid[x, y] = -1;
            }
        }
    }

    static public void FinalizeGridLayoutForTurn()
    {
        string _gridDetail = "";
        for (int y = 0; y <  _terrainGrid.GetLength(1); y++)
        {
            for (int x = 0; x < _terrainGrid.GetLength(0); x++)
            {
                string _currentTile;
                if (_battleGrid[x, y] == null)
                {
                    _currentTile = "[" + _terrainGrid[x, y].GetTerrainType + "] ";
                }
                else
                {
                    _currentTile = "[" + _battleGrid[x, y].GetInfoIdentifier + "] ";
                }
                _gridDetail += _currentTile;
            }
            _gridDetail += "\n";
        }

        HistoryHandler.SaveCurrentGridLayout(_gridDetail);
    }

    static List<Vector2> AddTilesSurroundingMe(List<Vector2> spaces)
    {
        for (int i = 0; i < spaces.Count; i++)
        {
            int currX = (int)spaces[i].x;
            int currY = (int)spaces[i].y;
            
            for (int y = -1; y <= 1; y++)
            {
                int tileYPos = currY + y;
                for (int x = -1; x <= 1; x++)
                {
                    int tileXPos = currX + x;
                    if ((Mathf.Abs(x) + Mathf.Abs(y)) <= 1 && CanMoveFarther(currX, currY, tileXPos, tileYPos) && !IsSpaceOccupied(tileXPos, tileYPos)) 
                    {
                        Vector2 newSpace = new Vector2(tileXPos, tileYPos);
                        _moveGrid[tileXPos, tileYPos] = _moveGrid[currX, currY] - _terrainGrid[tileXPos, tileYPos].GetTerrainCost;
                        spaces.Add(newSpace);
                    }
                }
            }
        }

        return spaces;
    }

    static bool CanMoveFarther(int xPos, int yPos, int xCheck, int yCheck)
    {
        try
        {
            if ((_moveGrid[xPos, yPos] - _terrainGrid[xCheck, yCheck].GetTerrainCost) >= 0 && _moveGrid[xCheck,yCheck] == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
}