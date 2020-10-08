using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FightHandler
{

    static int _enemiesToMake;
    static int _currentEnemyNum;

    static List<Character> _spawnedEnemies;
    public static List<Character> GetEnemies { get { return _spawnedEnemies; } }

    //Called
    public static void Init(int numberOfEnemies, int numberOfTeams)
    {
        _currentEnemyNum = 0;
        _enemiesToMake = numberOfEnemies;
        CreateEnemies(numberOfTeams);
        _spawnedEnemies = RollForInitiative(_spawnedEnemies);

        DebugList(_spawnedEnemies);

        HistoryHandler.AddToCurrentAction("Battle Start!");
        HistoryHandler.FinalizeAction();
    }

    //creates Character scripts and adds them to the _spawnedEnemies list
    //creates an amount dependent on how many the player wants to make
    static void CreateEnemies(int numberOfTeams)
    {
        _spawnedEnemies = new List<Character>();
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

    //debug for debugging all occupants inside of the list that is passed
    static void DebugList(List<Character> list)
    {
        string speedDebug = "";
        for (int i = 0; i < list.Count; i++)
        {
            speedDebug += list[i].Name + ", " + list[i].Team + ", " + list[i].Speed +  ", " + list[i].IsDead + "\n";
        }
        //Debug.Log(speedDebug);
    }

    //called by Init
    //sorts and returns all the characters based on speed(fastest to slowest)
    static List<Character> RollForInitiative(List<Character> enemies)
    {
        List<Character> unsortedEnemies = new List<Character>();
        unsortedEnemies = enemies;
        List<Character> sortedEnemies = new List<Character>();


        for (int i = 0; i < enemies.Count; i=0)
        {
            Character temp = unsortedEnemies[0];
            for (int j = 0; j < unsortedEnemies.Count; j++)
            {
                if (unsortedEnemies[j].Speed > temp.Speed)
                {
                    temp = unsortedEnemies[j];
                }
            }
            sortedEnemies.Add(temp);
            unsortedEnemies.Remove(temp);
        }

        DebugList(sortedEnemies);

        return sortedEnemies;
    }

    //Starts the fight and asks enemies what they want to do
    //does not end until 1 enemy is left
    public static void ContinueFight()
    {
        if(_currentEnemyNum < _spawnedEnemies.Count)
        {
            _spawnedEnemies[_currentEnemyNum].TakeAction();
            DebugList(_spawnedEnemies);
            HistoryHandler.FinalizeAction(_spawnedEnemies[_currentEnemyNum]);
                
            _currentEnemyNum++;
        }
        else
        {
            _currentEnemyNum = 0;
            ContinueFight();
        }
    }

    //called by HistoryHandler
    //called by character when they die
    //checks to see if there is only one person or team left
    //ends the fight if returns true
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
            Debug.Log("1 left");
            return true;
        }
    }

    //called by characters
    //returns a random character to the character that called
    //returned character cannot be: someone on the same team, the character that called, or a dead character
    static public Character FindRandomEnemy(Character me, int teamNum)
    {
        //Debug.Log("finding enemies");
        List<Character> actualTargets = new List<Character>();

        for (int i = 0; i < _spawnedEnemies.Count; i++)
        {
            if(me != _spawnedEnemies[i] && !_spawnedEnemies[i].IsDead && _spawnedEnemies[i].Team != teamNum)
            {
                actualTargets.Add(_spawnedEnemies[i]);
            }
        }

        int randNum = Random.Range(0, actualTargets.Count);

        //Debug.Log(actualTargets[randNum].Name);
        return actualTargets[randNum];
    }

    //called by characters
    //asks FightHandler to check for hit/crit/miss
    //FightHandler passes string of result to HistoryHandler
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
