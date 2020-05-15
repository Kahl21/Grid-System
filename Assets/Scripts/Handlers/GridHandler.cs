using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridSize
{
    Small,
    Medium,
    Large
}

public enum GridTraits
{
    NONE,
    FORESTS,
    MARSH,
    HOLES,
    RANDOM
}

public static class GridHandler
{

    static Character[,] _battleGrid;
    public static int GetGridLength { get { return _battleGrid.GetLength(0); } }
    static TerrainSpace[,] _terrainGrid;
    static int[,] _moveGrid;

    static WorldGridHandler _worldGrid;

    public static void Init(WorldGridHandler trackref)
    {
        _worldGrid = trackref;
    }

    public static void CreateNewGrid(GridSize size, GridTraits trait, float maxheight)
    {
        switch (size)
        {
            case GridSize.Small:
                _battleGrid = new Character[6, 6];
                break;
            case GridSize.Medium:
                _battleGrid = new Character[8, 8];
                break;
            case GridSize.Large:
                _battleGrid = new Character[10, 10];
                break;
            default:
                break;
        }

        _moveGrid = new int[_battleGrid.GetLength(0), _battleGrid.GetLength(1)];
        ResetMoveGrid();

        GenerateTerrain(trait, maxheight);

        _worldGrid.GenerateWorldGrid(_terrainGrid);
    }

    public static void GenerateTerrain(GridTraits terrainTrait, float maxHeight)
    {
        _terrainGrid = new TerrainSpace[_battleGrid.GetLength(0), _battleGrid.GetLength(1)];
        for (int y = 0; y < _terrainGrid.GetLength(1); y++)
        {
            for (int x = 0; x < _terrainGrid.GetLength(0); x++)
            {
                _terrainGrid[x, y] = GenerateTile(terrainTrait, maxHeight);
            }
        }
    }

    static TerrainSpace GenerateTile(GridTraits terrain, float maxheight)
    {
        int rand = UnityEngine.Random.Range(0, 101);
        float heightfloatvalue = UnityEngine.Random.Range(0f, maxheight);
        float height = (float)Math.Round(heightfloatvalue * 2f, MidpointRounding.AwayFromZero) / 2;
        //Debug.Log(height);
        int forestChance, waterChance, holeChance;
        switch (terrain)
        {
            case GridTraits.NONE:
                return new TerrainSpace(height);
            case GridTraits.FORESTS:
                forestChance = 75;
                if (rand < forestChance)
                {
                    return new ForestSpace(height);
                }
                else
                {
                    return new TerrainSpace(height);
                }
            case GridTraits.MARSH:
                waterChance = 50;
                if (rand < waterChance)
                {
                    return new WaterSpace(height);
                }
                else
                {
                    return new TerrainSpace(height);
                }
            case GridTraits.HOLES:
                holeChance = 30;
                if (rand < holeChance)
                {
                    return new HoleSpace(height);
                }
                else
                {
                    return new TerrainSpace(height);
                }
            case GridTraits.RANDOM:
                forestChance = 30;
                waterChance = 20;
                holeChance = 10;
                if (rand < forestChance)
                {
                    return new ForestSpace(height);
                }
                else if(rand < forestChance + waterChance)
                {
                    return new WaterSpace(height);
                }
                else if(rand < forestChance + waterChance + holeChance)
                {
                    return new HoleSpace(height);
                }
                else
                {
                    return new TerrainSpace(height);
                }
            default:
                return new TerrainSpace();
        }
    }

    public static void PlaceEnemyOnBoard(Character newCharacter)
    {
        _battleGrid[(int)newCharacter.CurrentPosition.x, (int)newCharacter.CurrentPosition.y] = newCharacter;
        _worldGrid.SpawnCharacter(newCharacter, (int)newCharacter.CurrentPosition.x, (int)newCharacter.CurrentPosition.y);
    }

    public static Vector2 GetSpawn(bool isPlayerCharacter)
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

        int randspot = UnityEngine.Random.Range(0, availableSpawns.Count);

        return availableSpawns[randspot];
    }

    public static void ClearSpace(Vector2 posToClear)
    {
        //Debug.Log("space " + posToClear + " cleared");
        _battleGrid[(int)posToClear.x, (int)posToClear.y] = null;
    }

    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Weapon heldWeapon, TeamType team)
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

        return RemoveTeamMembers(enemiesInRange, team);
    }

    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Weapon heldWeapon, TeamType team, Character target)
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

        return RemoveTeamMembers(enemiesInRange, team);
    }

    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Ability ability, TeamType team)
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

        return RemoveTeamMembers(enemiesInRange, team);
    }



    public static List<Character> GetEnemiesinRange(Vector2 characterPos, Ability ability, TeamType team, Character target)
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

        return RemoveTeamMembers(enemiesInRange, team);
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


    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Weapon heldWeapon, TeamType team)
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
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos,tileYPos].Team != team)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Weapon heldWeapon, TeamType team, Character target)
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
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos, tileYPos].Team != team)
                    {
                        return true;
                    }
                }
                else if ((Mathf.Abs(x) + Mathf.Abs(y)) == heldWeapon.Range && IsSpaceOccupied(tileXPos, tileYPos, target))
                {
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos, tileYPos].Team != team)
                    {
                        return true;
                    }
                }
                
            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Ability ability, TeamType team)
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
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos, tileYPos].Team != team)
                    {
                        return true;
                    }
                }

            }
        }

        return false;
    }

    public static bool CheckForEnemyWithinRange(Vector2 characterPos, Ability ability, TeamType team, Character target)
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
                    if (tileXPos >= 0 && tileYPos >= 0 && characterPos != checkedPos && _battleGrid[tileXPos,tileYPos].Team != team)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static List<Character> RemoveTeamMembers(List<Character> enemyList, TeamType team)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].Team == team)
            {
                enemyList.RemoveAt(i);
                i--;
            }
        }

        return enemyList;
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
        Debug.Log(c1.CurrentPosition + " --- " + temp2);
        Debug.Log(c2.CurrentPosition + " --- " + temp1);
        HistoryHandler.AddToCurrentAction(c1.Name + " and " + c2.Name + " have swapped places!\n");
    }

    public static void SwapEnemies(List<Character> enemiesToFuckWith)
    {
        int rand1 = 0, rand2 = 0;


        for (int i = 0; i < enemiesToFuckWith.Count; i++)
        {
            while(rand1 == rand2)
            {
                rand1 = UnityEngine.Random.Range(0, enemiesToFuckWith.Count);
                rand2 = UnityEngine.Random.Range(0, enemiesToFuckWith.Count);
            }

            SwapEnemies(enemiesToFuckWith[rand1], enemiesToFuckWith[rand2]);

            rand1 = 0;
            rand2 = 0;
        }
    }

    //checks to see if the space is generically occupied
    public static bool IsSpaceOccupied(int pointx, int pointy)
    {
        try
        {
            if (_battleGrid[pointx, pointy] != null && _terrainGrid[pointx, pointy].GetTerrainType != 'O')
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
            if (_battleGrid[(int)checkPosition.x, (int)checkPosition.y] != null && _terrainGrid[(int)checkPosition.x, (int)checkPosition.y].GetTerrainType != 'O')
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
            if (_battleGrid[(int)checkPosition.x, (int)checkPosition.y] == target && _terrainGrid[(int)checkPosition.x, (int)checkPosition.y].GetTerrainType != 'O')
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