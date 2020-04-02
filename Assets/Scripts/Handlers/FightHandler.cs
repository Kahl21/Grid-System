using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FightHandler
{

    static int _enemiesToMake;
    static int _currentEnemyNum;

    static List<Character> _spawnedEnemies;
    public static List<Character> GetEnemies { get { return _spawnedEnemies; } }
   // static List<Character> _turnOrder;
   // public static List<Character> GetTurnOrder;

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

    public static void Init(int numberOfEnemies, int numberOfTeams)
    {
        _currentEnemyNum = 0;
        _enemiesToMake = numberOfEnemies;
        _spawnedEnemies = new List<Character>();
        CreateEnemies(numberOfTeams);
        _spawnedEnemies = RollForInitiative(_spawnedEnemies);
        HistoryHandler.AddToCurrentAction("Battle Start!");
        HistoryHandler.FinalizeAction();
    }

    //spawns enemies
    static void CreateEnemies()
    {
        for (int i = 0; i < _enemiesToMake; i++)
        {
            Vector2 spawn = GridHandler.GetSpawn();
            Character newCharacter = new Character("Enemy", (i+1).ToString(), -1, spawn);
            GridHandler.PlaceEnemyOnBoard(newCharacter);
            _spawnedEnemies.Add(newCharacter);
        }
    }

    static void CreateEnemies(int numberOfTeams)
    {
        for (int v = 0; v < numberOfTeams; v++)
        {
            for (int i = 0; i < _enemiesToMake; i++)
            {
                Vector2 spawn = GridHandler.GetSpawn();
                Character newCharacter = new Character("Enemy", (i + 1 + (v *_enemiesToMake)).ToString(), v + 1, spawn);
                GridHandler.PlaceEnemyOnBoard(newCharacter);
                _spawnedEnemies.Add(newCharacter);
            }
        }
    }

    static List<Character> RollForInitiative(List<Character> enemies)
    {
        List<Character> unsortedEnemies = new List<Character>(enemies);
        List<Character> sortedEnemies = new List<Character>();

        while(sortedEnemies.Count != _spawnedEnemies.Count)
        {
            Character temp = unsortedEnemies[0];
            for (int i = 0; i < unsortedEnemies.Count; i++)
            {
                if(enemies[i].Speed > temp.Speed)
                {
                    temp = unsortedEnemies[i];
                }
            }
            unsortedEnemies.Remove(temp);
            sortedEnemies.Add(temp);
        }

        for (int i = 0; i < sortedEnemies.Count; i++)
        {
            Debug.Log(sortedEnemies[i].Name + ", " + sortedEnemies[i].Team + ", " + sortedEnemies[i].Speed);
        }

        return sortedEnemies;
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
            Character check = _spawnedEnemies[0];
            for (int i = 0; i < _spawnedEnemies.Count; i++)
            {
                if (_spawnedEnemies[i].Team != check.Team)
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            //Debug.Log("1 left");
            return true;
        }
    }

    static public Character FindRandomEnemy(Character me, int teamNum)
    {
        //Debug.Log("finding enemies");
        int randNum = Random.Range(0, _spawnedEnemies.Count);

        try
        {
            while (me == _spawnedEnemies[randNum] || _spawnedEnemies[randNum].IsDead || _spawnedEnemies[randNum].Team == teamNum)
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
        //Debug.Log("attack called");
        //add beginning of statment
        HistoryHandler.AddToCurrentAction(attacker.Name + " attacks " + target.Name);

        //add what took place on the turn (HIT, CRIT, or MISS)
        int randNum = Random.Range(0, 100);
        if(attacker.Offense.Accuracy > randNum || randNum == 0)
        {
            int damage = attacker.Offense.Strength + attacker.HeldWeapon.StrengthMod;
            randNum = Random.Range(0, 100);
            if (attacker.Offense.CriticalChance > randNum || randNum == 0)
            {
                HistoryHandler.AddToCurrentAction("and CRITS for " + Mathf.Abs(target.Defense.CalculateDamage(damage, attacker.HeldWeapon.WeaponElement, true)).ToString() + "\n");
                target.TakeDamage(damage, attacker.HeldWeapon.WeaponElement, true);
            }
            else
            {
                HistoryHandler.AddToCurrentAction("and HITS for " + Mathf.Abs(target.Defense.CalculateDamage(damage, attacker.HeldWeapon.WeaponElement, false)).ToString() + "\n");
                target.TakeDamage(damage, attacker.HeldWeapon.WeaponElement, false);
            }
            target.LastAttacker = attacker;
        }
        else
        {
            HistoryHandler.AddToCurrentAction("and MISSED \n");
        }
    }
}
