using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHandler
{

    static Character[,] _battleGrid;
    static TerrainSpace[,] _terrainGrid;
    static int[,] _moveGrid;
    static int[,] _distanceGrid;
    static bool[,] _visitCheckGrid;

    public static void CreateNewGrid(int numOfRows, int numOfColms)
    {
        _battleGrid = new Character[numOfRows, numOfColms];

        _moveGrid = new int[numOfRows, numOfColms];
        ResetIntGrid(_moveGrid);

        _distanceGrid = new int[numOfRows, numOfColms];

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
        //Debug.Log("space " + posToClear + " cleared");
        _battleGrid[(int)posToClear.x, (int)posToClear.y] = null;
    }

    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Weapon heldWeapon, int TeamNum)
    {
        List<Character> enemiesInRange = new List<Character>();

        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -heldWeapon.Range; y < heldWeapon.Range; y++)
        {
            int tileYPos = currY + y;
            for (int x = -heldWeapon.Range; x < heldWeapon.Range; x++)
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

        return RemoveTeamMembers(enemiesInRange, TeamNum);
    }

    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Weapon heldWeapon, int TeamNum, Character target)
    {
        List<Character> enemiesInRange = new List<Character>();

        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -heldWeapon.Range; y < heldWeapon.Range; y++)
        {
            int tileYPos = currY + y;
            for (int x = -heldWeapon.Range; x < heldWeapon.Range; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);
                if (!heldWeapon.IsRanged && (Mathf.Abs(x) + Mathf.Abs(y)) <= heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        enemiesInRange.Add(_battleGrid[tileXPos, tileYPos]);
                    }
                }
                else if ((Mathf.Abs(x) + Mathf.Abs(y)) == heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        enemiesInRange.Add(_battleGrid[tileXPos, tileYPos]);
                    }
                }
            }
        }

        return RemoveTeamMembers(enemiesInRange, TeamNum);
    }

    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Ability ability, int TeamNum)
    {
        List<Character> enemiesInRange = new List<Character>();

        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -ability.TargetRange; y < ability.TargetRange; y++)
        {
            int tileYPos = currY + y;
            for (int x = -ability.TargetRange; x < ability.TargetRange; x++)
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

        return RemoveTeamMembers(enemiesInRange, TeamNum);
    }



    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Ability ability, int TeamNum, Character target)
    {
        List<Character> enemiesInRange = new List<Character>();

        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -ability.TargetRange; y < ability.TargetRange; y++)
        {
            int tileYPos = currY + y;
            for (int x = -ability.TargetRange; x < ability.TargetRange; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);
                if ((Mathf.Abs(x) + Mathf.Abs(y)) <= ability.TargetRange && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos)
                    {
                        enemiesInRange.Add(_battleGrid[tileXPos, tileYPos]);
                    }
                }
            }
        }

        return RemoveTeamMembers(enemiesInRange, TeamNum);
    }

    public static List<Character> GetTargetsInSplashZone(Vector2 targetPos, Vector2 castPos, Ability ability)
    {
        List<Character> enemiesInRange = new List<Character>();

        int currX = (int)targetPos.x;
        int currY = (int)targetPos.y;

        switch (ability.Shape)
        {
            case DamageShape.SINGLE:
                enemiesInRange.Add(_battleGrid[currX, currY]);
                break;
            case DamageShape.STAR:
                for (int y = -ability.SplashRange; y < ability.SplashRange; y++)
                {
                    int tileYPos = currY + y;
                    for (int x = -ability.SplashRange; x < ability.SplashRange; x++)
                    {
                        int tileXPos = currX + x;
                        if ((Mathf.Abs(x) + Mathf.Abs(y)) <= ability.SplashRange && IsSpaceOccupied(tileXPos, tileYPos))
                        {
                            if (tileXPos >= 0 && tileYPos >= 0)
                            {
                                enemiesInRange.Add(_battleGrid[tileXPos, tileYPos]);
                            }
                        }
                    }
                }
                break;
            case DamageShape.SELFAREA:
                currX = (int)castPos.x;
                currY = (int)castPos.y;

                for (int y = -ability.SplashRange; y < ability.SplashRange; y++)
                {
                    int tileYPos = currY + y;
                    for (int x = -ability.SplashRange; x < ability.SplashRange; x++)
                    {
                        int tileXPos = currX + x;
                        if ((Mathf.Abs(x) + Mathf.Abs(y)) <= ability.SplashRange && IsSpaceOccupied(tileXPos, tileYPos))
                        {
                            if (tileXPos >= 0 && tileYPos >= 0)
                            {
                                enemiesInRange.Add(_battleGrid[tileXPos, tileYPos]);
                            }
                        }
                    }
                }
                break;
            case DamageShape.LINE:
                break;
            case DamageShape.CONE:
                break;
            case DamageShape.ALL:
                break;
            default:
                break;
        }

        return enemiesInRange;
    }


    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Weapon heldWeapon, int TeamNum)
    {
        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -heldWeapon.Range; y < heldWeapon.Range; y++)
        {
            int tileYPos = currY + y;
            for (int x = -heldWeapon.Range; x < heldWeapon.Range; x++)
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
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos, tileYPos].Team != TeamNum)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Weapon heldWeapon, int TeamNum, Character target)
    {
        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -heldWeapon.Range; y < heldWeapon.Range; y++)
        {
            int tileYPos = currY + y;
            for (int x = -heldWeapon.Range; x < heldWeapon.Range; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);

                if (!heldWeapon.IsRanged && (Mathf.Abs(x) + Mathf.Abs(y)) <= heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos, tileYPos].Team != TeamNum)
                    {
                        return true;
                    }
                }
                else if ((Mathf.Abs(x) + Mathf.Abs(y)) == heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos, tileYPos].Team != TeamNum)
                    {
                        return true;
                    }
                }

            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Ability ability, int TeamNum)
    {
        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -ability.TargetRange; y < ability.TargetRange; y++)
        {
            int tileYPos = currY + y;
            for (int x = -ability.TargetRange; x < ability.TargetRange; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);

                if ((Mathf.Abs(x) + Mathf.Abs(y)) <= ability.TargetRange && IsSpaceOccupied(tileXPos, tileYPos))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos, tileYPos].Team != TeamNum)
                    {
                        return true;
                    }
                }

            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Ability ability, int TeamNum, Character target)
    {
        int currX = (int)characterPos.x;
        int currY = (int)characterPos.y;

        for (int y = -ability.TargetRange; y < ability.TargetRange; y++)
        {
            int tileYPos = currY + y;
            for (int x = -ability.TargetRange; x < ability.TargetRange; x++)
            {
                int tileXPos = currX + x;
                Vector2 checkedPos = new Vector2(tileXPos, tileYPos);

                if ((Mathf.Abs(x) + Mathf.Abs(y)) <= ability.TargetRange && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos, tileYPos].Team != TeamNum)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static List<Character> RemoveTeamMembers(List<Character> enemyList, int TeamNum)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].Team == TeamNum)
            {
                enemyList.RemoveAt(i);
                i--;
            }
        }

        return enemyList;
    }


    public static void MoveEnemy(Character movingCharacter, Vector2 desiredPos)
    {
        if (movingCharacter.CurrentPosition != desiredPos)
        {
            HistoryHandler.AddToCurrentAction(movingCharacter.Name + " MOVES \n");
            ClearSpace(movingCharacter.CurrentPosition);

            movingCharacter.CurrentPosition = desiredPos;
            _battleGrid[(int)desiredPos.x, (int)desiredPos.y] = movingCharacter;
        }
        else if (movingCharacter.SpacesICanMove.Count == 1)
        {
            HistoryHandler.AddToCurrentAction(movingCharacter.Name + " CAN'T MOVE \n");
        }
        else
        {
            HistoryHandler.AddToCurrentAction(movingCharacter.Name + " STANDS IN PLACE \n");
        }
    }

    //swap two characters to each others position
    public static void SwapEnemies(Character c1, Character c2)
    {
        Vector2 temp1 = new Vector2(c1.CurrentPosition.x, c1.CurrentPosition.y);
        Vector2 temp2 = new Vector2(c2.CurrentPosition.x, c2.CurrentPosition.y);

        ClearSpace(temp2);
        c1.CurrentPosition = temp2;
        _battleGrid[(int)temp2.x, (int)temp2.y] = c1;

        c2.CurrentPosition = temp1;
        _battleGrid[(int)temp1.x, (int)temp1.y] = c2;
        //Debug.Log(c1.CurrentPosition + " --- " + temp2);
        //Debug.Log(c2.CurrentPosition + " --- " + temp1);
        HistoryHandler.AddToCurrentAction(c1.Name + " and " + c2.Name + " have swapped places!\n");
    }

    public static void SwapEnemies(List<Character> enemiesToFuckWith)
    {
        int rand1 = 0, rand2 = 0;


        for (int i = 0; i < enemiesToFuckWith.Count; i++)
        {
            while (rand1 == rand2)
            {
                rand1 = Random.Range(0, enemiesToFuckWith.Count);
                rand2 = Random.Range(0, enemiesToFuckWith.Count);
            }

            SwapEnemies(enemiesToFuckWith[rand1], enemiesToFuckWith[rand2]);

            rand1 = 0;
            rand2 = 0;
        }
    }

    // Utility method to check whether a point is 
    // inside the grid or not 
    static bool IsInsideGrid(int pointx, int pointy)
    {
        if (pointx >= 0 && pointx < _battleGrid.GetLength(0) && pointy >= 0 && pointy < _battleGrid.GetLength(1))
        {
            return true;
        }
        else
        {
            return false;
        }
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

    public static void GetShortestMoves(Character moveableChar, Vector2 targetedSpace)
    {
        Shortest(moveableChar.CurrentPosition, targetedSpace);
        

    }

    static bool CompareSpace(Vector2 a, Vector2 b) 
    {
        if (_terrainGrid[(int)a.x,(int)a.y].GetTerrainCost == _terrainGrid[(int)b.x, (int)b.y].GetTerrainCost) 
        { 
            if (a.x != b.x) 
                return (a.x < b.x); 
            else
                return (a.y < b.y); 
        } 
        return (_terrainGrid[(int)a.x, (int)a.y].GetTerrainCost < _terrainGrid[(int)b.x, (int)b.y].GetTerrainCost); 
    } 


    // Method returns minimum cost to reach bottom 
    // right from top left 
    static int Shortest(Vector2 startPos, Vector2 endPos)
    {
  
        // initializing distance array by INT_MAX 
        for (int i = 0; i < _battleGrid.GetLength(0); i++) 
            for (int j = 0; j < _battleGrid.GetLength(1); j++) 
                _distanceGrid[i,j] = 1000; 
  
        // direction arrays for simplification of getting 
        // neighbour 
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        List<Vector2> st = new List<Vector2>();

        // insert (0, 0) cell with 0 distance 
        st.Add(new Vector2((int)startPos.x,(int) startPos.y));

        // initialize distance of (0, 0) with its grid value 
        _distanceGrid[(int)startPos.x, (int)startPos.y] = _terrainGrid[(int)startPos.x, (int)startPos.y].GetTerrainCost; 
  
            // loop for standard dijkstra's algorithm 
            while (st.Count > 0) 
            { 
                // get the cell with minimum distance and delete 
                // it from the set 
                Vector2 k = st[0];
                st.RemoveAt(0); 
  
            // looping through all neighbours 
            for (int i = 0; i< 4; i++) 
            { 
                    int x = (int)k.x + dx[i];
                    int y = (int)k.y + dy[i]; 
  
                // if not inside boundary, ignore them 
                if (!IsInsideGrid(x, y)) 
                    continue; 
  
                // If distance from current cell is smaller, then 
                // update distance of neighbour cell 
                if (_distanceGrid[x,y] > _distanceGrid[(int)k.x,(int)k.y] + _terrainGrid[x,y].GetTerrainCost) 
                { 
                    // If cell is already there in set, then 
                    // remove its previous entry 
                    if (_distanceGrid[x,y] != 1000)
                    {
                        int num = GetListPlaceFromValue(st, new Vector2(x, y));
                        if (num != -1)
                        {
                            st.RemoveAt(num);
                        }
                    }

                    // update the distance and insert new updated 
                    // cell in set 
                    _distanceGrid[x,y] = _distanceGrid[(int)k.x, (int)k.y] + _terrainGrid[x,y].GetTerrainCost; 
                    
                    st.Add(new Vector2(x,y)); 
                } 
            } 
        }

        // print distance of each cell from (0, 0) 
        string debug = "";

        for (int j = 0; j < _distanceGrid.GetLength(1); j++)
        {
            for (int i = 0; i < _distanceGrid.GetLength(0); i++)
            {
                debug += "[" + _distanceGrid[i, j] + "] ";
            }
            debug += "\n";
        }

        Debug.Log(debug);


        // dis[row - 1][col - 1] will represent final 
        // distance of bottom right cell from top left cell
        Debug.Log(_distanceGrid[(int)endPos.x, (int)endPos.y] + ", " + endPos);
        return _distanceGrid[(int)endPos.x,(int)endPos.y]; 
    }

    static int GetListPlaceFromValue(List<Vector2> d, Vector2 thing)
    {
        for (int i = 0; i < d.Count; i++)
        {
            if(d[i] == thing)
            {
                return i;
            }
        }

        return -1;
    }

    public static List<Vector2> WhereCanIMove(Vector2 startPosition, int movement)
    {
        ResetIntGrid(_moveGrid);
        List<Vector2> availableSpaces = new List<Vector2>();

        int startX = (int)startPosition.x;
        int startY = (int)startPosition.y;

        _moveGrid[startX, startY] = movement;

        availableSpaces.Add(startPosition);

        availableSpaces = AddTilesSurroundingMe(availableSpaces);

        return availableSpaces;    
    }
    
    static void ResetIntGrid(int [,] grid)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, y] = -1;
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
        if (IsInsideGrid(xCheck, yCheck))
        {
            if ((_moveGrid[xPos, yPos] - _terrainGrid[xCheck, yCheck].GetTerrainCost) >= 0 && _moveGrid[xCheck, yCheck] == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
}