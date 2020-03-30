using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FightHandler
{

    static int _enemiesToMake;
    static int _currentEnemyNum;

    static List<Character> _spawnedEnemies;

    //Base Constructor
    public static void Init()
    {
        _enemiesToMake = 2;
        _currentEnemyNum = 0;
        _spawnedEnemies = new List<Character>();
        CreateEnemies();
    }

    //Constructor if UI is used
    public static void Init(int numberOfEnemies)
    {
        _currentEnemyNum = 0;
        _enemiesToMake = numberOfEnemies;
        _spawnedEnemies = new List<Character>();
        CreateEnemies();
        HistoryHandler.AddToCurrentAction("Battle Start!");
        HistoryHandler.FinalizeAction();
    }

    //spawns enemies
    static void CreateEnemies()
    {
        for (int i = 0; i < _enemiesToMake; i++)
        {
            Vector2 spawn = GridHandler.GetSpawn();
            Character newCharacter = new Character("Enemy", (i+1).ToString(), spawn);
            GridHandler.PlaceEnemyOnBoard(newCharacter);
            _spawnedEnemies.Add(newCharacter);
        }
    }

    //Starts the fight and asks enemies what they want to do
    //does not end until 1 enemy is left
    public static void ContinueFight()
    {
            if(_currentEnemyNum < _spawnedEnemies.Count)
            {
                _spawnedEnemies[_currentEnemyNum].TakeAction();
                HistoryHandler.FinalizeAction(_spawnedEnemies[_currentEnemyNum]);
                
                _currentEnemyNum++;
            }
            else
            {
                _currentEnemyNum = 0;
                ContinueFight();
            }
    }

    static public bool LastManStanding()
    {
        for (int i = 0; i < _spawnedEnemies.Count; i++)
        {
            Character check = _spawnedEnemies[i];
            if(check.IsDead)
            {
                GridHandler.ClearSpace(check.CurrentPosition);
                _spawnedEnemies.Remove(check);
                if (_currentEnemyNum >= i)
                {
                    i--;
                    _currentEnemyNum--;
                }
            }
        }

        if(_spawnedEnemies.Count > 1)
        {
            //Debug.Log("more than 1");
            return false;
        }
        else
        {
            //Debug.Log("1 left");
            return true;
        }
    }

/*    static public void LastManStanding(Character DeadFighter)
    {
        GridHandler.ClearSpace(DeadFighter.CurrentPosition);
        _spawnedEnemies.Remove(DeadFighter);
        _currentEnemyNum--;
    }*/

    static public Character FindRandomEnemy(Character me)
    {
        //Debug.Log("finding enemies");
        int randNum = Random.Range(0, _spawnedEnemies.Count);

        try
        {
            while (me == _spawnedEnemies[randNum] || _spawnedEnemies[randNum].IsDead)
            {
               
                randNum = Random.Range(0, _spawnedEnemies.Count);
            }
        }
        catch
        {
            //Debug.Log("died in finding");
        }

        return _spawnedEnemies[randNum];
    }

    //Attack a random Enemy
    static public void AttackEnemy(Character attacker, Character target)
    {
        //add beginning of statment
        HistoryHandler.AddToCurrentAction(attacker.Name + " attacks " + target.Name);

        //add what took place on the turn (HIT, CRIT, or MISS)
        int randNum = Random.Range(0, 100);
        if(attacker.Accuracy > randNum)
        {
            randNum = Random.Range(0, 100);
            if (attacker.Crit > randNum)
            {
                HistoryHandler.AddToCurrentAction("and CRITS for " + (attacker.Strength * 2).ToString());
                target.TakeDamage(attacker.Strength * 2);
                target.LastAttacker = attacker;
            }
            else
            {
                HistoryHandler.AddToCurrentAction("and HITS for " + (attacker.Strength).ToString());
                target.TakeDamage(attacker.Strength);
                target.LastAttacker = attacker;
            }
        }
        else
        {
            HistoryHandler.AddToCurrentAction("and MISSED");
        }
    }

    
}
