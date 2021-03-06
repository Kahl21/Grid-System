﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum for grid size
//shown in UI
public enum GridSize
{
    Small,
    Medium,
    Large
}

//list of traits that change characteristics of Terrain
//shown in UI
public enum GridTraits
{
    NONE,
    //FORESTS,
    //MARSH,
    //HOLES
    //RANDOM
}

public enum Actions
{
    MOVE,
    ATTACK,
    ABILITY
}

public static class GridHandler
{
    //Vars
    static Character[,] _battleGrid;
    public static int GetGridLength { get { return _battleGrid.GetLength(0); } }
    static TerrainSpace[,] _terrainGrid;
    static int[,] _moveGrid;
    static int[,] _distanceGrid;

    static WorldGridHandler _worldGrid;

    //direction arrays for simplification of getting neighbor 
    static int[] dx = { -1, 0, 1, 0 };
    static int[] dy = { 0, 1, 0, -1 };

    //base Initialize
    public static void Init()
    {
        _worldGrid = WorldGridHandler.WorldInstance;
    }

    //creates a new array of terrain based on the size and traits passed to it
    //called whenever the player changes something in the GridSetupUI
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

        _distanceGrid = new int[_battleGrid.GetLength(0), _battleGrid.GetLength(1)];

        GenerateTerrain(trait, maxheight);

        _worldGrid.GenerateWorldGrid(_terrainGrid);
    }

    //creates a single instance of a full grid
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

    //creates a single Tile(Terrain script) that populates the grid in data
    static TerrainSpace GenerateTile(GridTraits terrain, float maxheight)
    {
        int rand = UnityEngine.Random.Range(0, 101);
        float heightfloatvalue = UnityEngine.Random.Range(0f, maxheight);
        float height = (float)Math.Round(heightfloatvalue * 2f, MidpointRounding.AwayFromZero) / 2;
        //Debug.Log(height);
        //int forestChance, waterChance, holeChance;
        switch (terrain)
        {
            case GridTraits.NONE:
                return new TerrainSpace(height);
            //case GridTraits.FORESTS:
            //    forestChance = 75;
            //    if (rand < forestChance)
            //    {
            //        return new ForestSpace(height);
            //    }
            //    else
            //    {
            //        return new TerrainSpace(height);
            //    }
            //case GridTraits.MARSH:
            //    waterChance = 50;
            //    if (rand < waterChance)
            //    {
            //        return new WaterSpace(height);
            //    }
            //    else
            //    {
            //        return new TerrainSpace(height);
            //    }
            //case GridTraits.HOLES:
            //    holeChance = 30;
            //    if (rand < holeChance)
            //    {
            //        return new HoleSpace(height);
            //    }
            //    else
            //    {
            //        return new TerrainSpace(height);
            //    }
            //case GridTraits.RANDOM:
            //    forestChance = 30;
            //    waterChance = 20;
            //    holeChance = 10;
            //    if (rand < forestChance)
            //    {
            //        return new ForestSpace(height);
            //    }
            //    else if(rand < forestChance + waterChance)
            //    {
            //        return new WaterSpace(height);
            //    }
            //    else if(rand < forestChance + waterChance + holeChance)
            //    {
            //        return new HoleSpace(height);
            //    }
            //    else
            //    {
            //        return new TerrainSpace(height);
            //    }
            default:
                return new TerrainSpace();
        }
    }
    
    //once a character is done with their turn
    //save the grid layout as a string
    //and send it to the HistoryHandler
    static public void DebugDataGrid()
    {
        string _gridDetail = "";
        for (int y = 0; y < _terrainGrid.GetLength(1); y++)
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

        Debug.Log("Data Grid: \n" + _gridDetail);
    }

    //Places an enemy on the board randomly
    public static void PlaceEnemyOnBoard(Character newCharacter)
    {
        _battleGrid[(int)newCharacter.CurrentPosition.x, (int)newCharacter.CurrentPosition.y] = newCharacter;
        _worldGrid.SpawnEnemy((int)newCharacter.CurrentPosition.x, (int)newCharacter.CurrentPosition.y);
    }

    public static void PlacePlayerOnBoard(Character newCharacter)
    {
        _worldGrid.SpawnPlayer(newCharacter);
        _battleGrid[(int)newCharacter.CurrentPosition.x, (int)newCharacter.CurrentPosition.y] = newCharacter;
    }

    //returns a spawn position for enemies or players
    //If player, it spawns in the main spawn area
    //If enemy, it gets placed randomly on the board (outside of main spawn)
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

    //removes a character from a place on the board
    public static void ClearSpace(Vector2 posToClear)
    {
        //Debug.Log("space " + posToClear + " cleared");
        _battleGrid[(int)posToClear.x, (int)posToClear.y] = null;
    }

    //returns a Character from the grid from the position
    public static Character RetrieveCharacter(int xpos, int ypos)
    {
        return _battleGrid[xpos, ypos];
    }
    public static Character RetrieveCharacter(Vector2 pos)
    {
        return _battleGrid[(int)pos.x, (int)pos.y];
    }

    public static GridToken RetrieveToken(Vector2 gridPos)
    {
        return _worldGrid.GetGridToken(gridPos);
    }

    public static Tile RetrieveTile(Vector2 tilePos)
    {
        return _worldGrid.GetTile(tilePos);
    }

// ---------------- TARGETTING FUNCTIONS FOR AI -----------------//

    //called if the character is attacking indescriminantly against the enemy team
    //attacking with weapon
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

    //called if the character is trying to attack a particular enemy target
    //attacking with weapon
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

    //called if the character is attacking indescriminantly against the enemy team
    //attacking with ability
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

    //called if the character is trying to attack a particular enemy target
    //attacking with ability
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

    //called when a character is trying to use an ability on someone
    //Checks for what type of spell is it and gets the zone it covers
    //returns all characters that will get hit 
    public static List<Character> GetTargetsInSplashZone(Vector2 targetPos, Ability ability)
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


    // ---------------- LOOKING FUNCTIONS FOR AI -----------------//

    //called if the character is looking for any enemies around them
    //attacking with weapon
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

    //called if the character is looking for specific enemy in its perimeter
    //attacking with weapon
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

    //called if the character is looking for any enemies around them
    //attacking with ability
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

    //called if the character is looking for specific enemy in its perimeter
    //attacking with ability
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

    public static bool CheckForEnemyWithinSpashZone(Vector2 targetPos, int splashZone)
    {

        int currX = (int)targetPos.x;
        int currY = (int)targetPos.y;

        if (splashZone > 0)
        {

            for (int y = -splashZone; y < splashZone; y++)
            {
                int tileYPos = currY + y;
                for (int x = -splashZone; x < splashZone; x++)
                {
                    int tileXPos = currX + x;
                    Vector2 checkedPos = new Vector2(tileXPos, tileYPos);

                    //Debug.Log((Mathf.Abs(x) + Mathf.Abs(y)) + " --> " + splashZone);
                    if ((Mathf.Abs(x) + Mathf.Abs(y)) <= splashZone && IsSpaceOccupied(tileXPos, tileYPos))
                    {
                        if (tileXPos >= 0 && tileYPos >= 0 && targetPos != checkedPos)
                        {
                            //Debug.Log("true");
                            return true;
                        }
                    }
                }
            }
            //Debug.Log("false");
            return false;
        }
        else
        {
            if (currX >= 0 && currY >= 0)
            {
                if (IsSpaceOccupied(currX, currY))
                {
                    return true;
                }
            }

            return false;
        }

    }

    //removes any Characters that are on the same team as the Character attacking
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


// ---------------- MOVEMENT FUNCTIONS -----------------//

    //Moves character from one grid space to another one in range
    //then adds the action of moving to the HistoryHandler
    public static void MoveEnemy(Character movingCharacter, Vector2 desiredPos)
    {
        //HistoryHandler.AddToCurrentAction(movingCharacter.Name + " MOVES \n");
        _battleGrid[(int)desiredPos.x, (int)desiredPos.y] = movingCharacter;
        ClearSpace(movingCharacter.CurrentPosition);

        movingCharacter.CurrentPosition = desiredPos;

        

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
        //HistoryHandler.AddToCurrentAction(c1.Name + " and " + c2.Name + " have swapped places!\n");
    }

    //randomly swaps and list of characters with one another 
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

    //checks to see if the space is occupied by a certain character
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


// ---------------- MOVEMENT FUNCTIONS THAT MESS WITH THE MOVEGRID -----------------//

    public static void DebugMoveGrid()
    {
        string _gridDetail = "";
        for (int y = 0; y < _terrainGrid.GetLength(1); y++)
        {
            for (int x = 0; x < _terrainGrid.GetLength(0); x++)
            {
                string _currentTile;
                if (_moveGrid[x, y] == -1)
                {
                    _currentTile = "[ ] ";
                }
                else
                {
                    _currentTile = "[" + _moveGrid[x, y] + "] ";
                }
                _gridDetail += _currentTile;
            }
            _gridDetail += "\n";
        }

        Debug.Log("Data Grid: \n" + _gridDetail);
    }

    public static List<Vector2> GetShortestMoves(Character moveableChar, Vector2 targetedSpace)
    {
        //Debug.Log(moveableChar.GetInfoIdentifier + " --> " + targetedSpace);
        return UseDijkstra(moveableChar.CurrentPosition, targetedSpace);
    }

    // Method sets up distance grid
    // then returns a set of vectors(tile placements) as move instructions
    static List<Vector2> UseDijkstra(Vector2 startPos, Vector2 endPos)
    {
        Vector2 checkingEnd = endPos;

        // initializing distance array by INT_MAX 
        for (int i = 0; i < _battleGrid.GetLength(0); i++)
        {
            for (int j = 0; j < _battleGrid.GetLength(1); j++)
            {
                _distanceGrid[i, j] = 1000;
            }
        }


        List<Vector2> CalculatingTiles = new List<Vector2>();
        List<Vector2> NextMoves = new List<Vector2>();

        // insert (0, 0) cell with 0 distance 
        CalculatingTiles.Add(startPos);
        NextMoves.Add(startPos);

        // initialize distance of (0, 0) with its grid value 
        _distanceGrid[(int)startPos.x, (int)startPos.y] = 0;

        // loop for standard dijkstra's algorithm 
        while (CalculatingTiles.Count > 0)
        {
            // get the cell with minimum distance and delete 
            // it from the set

            Vector2 checkingPoint = CalculatingTiles[0];
            Vector2 whichtoaddtopath = checkingPoint;
            CalculatingTiles.RemoveAt(0);


            // looping through all neighbours 
            for (int i = 0; i < 4; i++)
            {
                int x = (int)checkingPoint.x + dx[i];
                int y = (int)checkingPoint.y + dy[i];
                Vector2 check = new Vector2(x, y);

                // if not inside boundary, ignore them 
                if (!IsInsideGrid(x, y))
                {
                    continue;
                }

                // If distance from current cell is smaller, then 
                // update distance of neighbour cell 
                if (_distanceGrid[x, y] > _distanceGrid[(int)checkingPoint.x, (int)checkingPoint.y] + _terrainGrid[x, y].GetTerrainCost)
                {
                    // If cell is already there in set, then 
                    // remove its previous entry 
                    if (_distanceGrid[x, y] != 1000)
                    {
                        if (CalculatingTiles.Contains(check))
                        {
                            //Debug.Log("Removed vector: " + st[num]);
                            CalculatingTiles.Remove(check);
                        }
                    }

                    // update the distance and insert new updated 
                    // cell in set 
                    _distanceGrid[x, y] = _distanceGrid[(int)checkingPoint.x, (int)checkingPoint.y] + _terrainGrid[x, y].GetTerrainCost;

                    //if()


                    // Debug.Log("added vector: " + x + ", " + y);
                    CalculatingTiles.Add(check);
                    NextMoves.Add(whichtoaddtopath);
                }
            }

            //Debug.Log(st[st.Count - 1].ToString() + "--> " + endPos);
            if (CalculatingTiles.Contains(checkingEnd))
            {
                //Debug.Log("exit dijkstra");


                break;
            }
        }

        AddEnemiesToDistanceMap(startPos);

        return GetShortestPath(startPos, checkingEnd);
    }



    //After Dijkstra
    //"hollow out" spaces that have characters on them except for the player trying to move
    static void AddEnemiesToDistanceMap(Vector2 characterPostion)
    {
        for (int j = 0; j < _distanceGrid.GetLength(1); j++)
        {
            for (int i = 0; i < _distanceGrid.GetLength(0); i++)
            {
                Vector2 newpos = new Vector2(i, j);
                if (_battleGrid[i, j] != null && newpos != characterPostion)
                {
                    _distanceGrid[i, j] = 1000;
                }
            }
        }
    }

    //After Distance map is ready(after dijkstra)
    static List<Vector2> GetShortestPath(Vector2 start, Vector2 end)
    {
        //Start actual Path vector list that we will pass back
        //Start checking vector list for parsing
        List<Vector2> path = new List<Vector2>();
        List<Vector2> check = new List<Vector2>();
        path.Add(start);
        check.Add(start);

        while (check.Count > 0)
        {
            //set space we will check(nextspace) and remove space from check vector list
            Vector2 currentspace = check[0];
            Vector2 nextspace = check[0];
            check.RemoveAt(0);

            for (int i = 0; i < 4; i++)
            {
                int x = (int)currentspace.x + dx[i];
                int y = (int)currentspace.y + dy[i];
                Vector2 newSpace = new Vector2(x, y);

                if (!IsInsideGrid(x, y))
                {
                    //Debug.Log("spot does not exist");
                    continue;
                }

                //Debug.Log(end);

                //check
                if (_distanceGrid[x, y] < 1000 && !path.Contains(newSpace))
                {
                    if (_distanceGrid[x, y] > _distanceGrid[(int)nextspace.x, (int)nextspace.y])
                    {
                        nextspace = newSpace;
                    }
                    else if (_distanceGrid[x, y] == _distanceGrid[(int)nextspace.x, (int)nextspace.y])
                    {
                        if (Vector2.Distance(nextspace, end) > Vector2.Distance(newSpace, end))
                        {
                            nextspace = newSpace;
                        }
                    }
                }
            }

            if (nextspace == end || nextspace == currentspace)
            {
                break;
            }

            check.Add(nextspace);
            path.Add(nextspace);
        }

        if (!IsSpaceOccupied(end))
        {
            path.Add(end);
        }
        
        
        string pathDebug = "";
        for (int i = 0; i < path.Count; i++)
        {
            pathDebug += path[i].x + ", " + path[i].y + "\n";
        }
        Debug.Log(pathDebug);
        
        return path;
    }

    public static void DijkstraMove(Character moveableChar, Tile targetedSpace)
    {
        //DebugDataGrid();

        List<Vector2> tilesToDestination = new List<Vector2>();

        Vector2 moveSpot = new Vector2(targetedSpace.GetXPosition, targetedSpace.GetYPosition);

        tilesToDestination = GetShortestMoves(moveableChar, moveSpot);
        
        _worldGrid.StartCharacterMove(tilesToDestination);

        MoveEnemy(moveableChar, moveSpot);
    }
    
    public static void DijkstraMove(Character moveableChar, Tile targetedSpace, int maxmovement)
    {
        //DebugDataGrid();

        List<Vector2> tilesToDestination = new List<Vector2>();

        Vector2 moveSpot = new Vector2(targetedSpace.GetXPosition, targetedSpace.GetYPosition);

        tilesToDestination = GetShortestMoves(moveableChar, moveSpot);

        //Debug.Log("tiles -> " + tilesToDestination.Count + ", movement = " + maxmovement);

        for (int i = tilesToDestination.Count; i > maxmovement+1; i--)
        {
            tilesToDestination.RemoveAt(i - 1);
        }

        _worldGrid.StartCharacterMove(tilesToDestination);

        MoveEnemy(moveableChar, tilesToDestination[tilesToDestination.Count - 1]);
    }

    //resets and calculates all spaces that the character can move to
    public static List<Vector2> WhereCanIMove(Vector2 startPosition, int movement)
    {
        ResetMoveGrid();
        List<Vector2> availableSpaces = new List<Vector2>();

        int startX = (int)startPosition.x;
        int startY = (int)startPosition.y;

        _moveGrid[startX, startY] = movement;

        availableSpaces.Add(startPosition);

        availableSpaces = AddTilesInRange(availableSpaces, Actions.MOVE);

        return availableSpaces;    
    }

    public static void ShowReleventGrid(Vector2 startPosition, int range, Color panelColor, Actions whatdoing)
    {
        ResetMoveGrid();
        _worldGrid.ResetPanels();
        List<Vector2> availableSpaces = new List<Vector2>();

        int startX = (int)startPosition.x;
        int startY = (int)startPosition.y;


        _moveGrid[startX, startY] = range;

        availableSpaces.Add(startPosition);

        availableSpaces = AddTilesInRange(availableSpaces, whatdoing);
                

        //Debug.Log(movement);
        //DebugMoveGrid();

        _worldGrid.LightUpBoard(availableSpaces, panelColor);
    }

    public static int GetDistanceMoved(int xpos, int ypos)
    {
        return _moveGrid[xpos, ypos];
    }
    
    //resets the movement tracking grid to null values
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

    //grabs all four grid positions around a certain poition
    //returns all available positions
    static List<Vector2> AddTilesInRange(List<Vector2> spaces, Actions action)
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
                    if (action == Actions.MOVE)
                    {
                        if ((Mathf.Abs(x) + Mathf.Abs(y)) <= 1 && CanMoveFarther(currX, currY, tileXPos, tileYPos) && !IsSpaceOccupied(tileXPos, tileYPos))
                        {
                            Vector2 newSpace = new Vector2(tileXPos, tileYPos);
                            _moveGrid[tileXPos, tileYPos] = _moveGrid[currX, currY] - _terrainGrid[tileXPos, tileYPos].GetTerrainCost;
                            spaces.Add(newSpace);
                        }
                    }
                    else
                    {
                        if ((Mathf.Abs(x) + Mathf.Abs(y)) <= 1 && IsInAttackRange(currX, currY, tileXPos, tileYPos))
                        {
                            Vector2 newSpace = new Vector2(tileXPos, tileYPos);
                            _moveGrid[tileXPos, tileYPos] = _moveGrid[currX, currY] - _terrainGrid[tileXPos, tileYPos].GetTerrainCost;
                            spaces.Add(newSpace);
                        }
                    }
                }
            }
        }

        if(action != Actions.ABILITY)
        {
            spaces.RemoveAt(0);
        }

        return spaces;
    }

    
    //checks to see if the player can move any farther 
    //references the movement tracking grid to see if the player has any more movement
    //returns if they can or not(bool)
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

    static bool IsInAttackRange(int xPos, int yPos, int xCheck, int yCheck)
    {
        try
        {
            if ((_moveGrid[xPos, yPos] - 1 >= 0 && _moveGrid[xCheck, yCheck] == -1))
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