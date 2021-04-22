using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamType
{
    PLAYER,
    ENEMY,
    OTHER
}

public static class FightHandler
{
    static UIHolder _uiRef;

    static int _enemiesToMake;

    static List<Character> _spawnedCharacters;
    public static List<Character> GetAllCharacters { get { return _spawnedCharacters; } }
   // static List<Character> _turnOrder;
   // public static List<Character> GetTurnOrder;

    //Base Constructor
    public static void Init()
    {
        _uiRef = UIHolder.UIInstance;
        _enemiesToMake = 2;
        _spawnedCharacters = new List<Character>();
        CreateEnemies();
    }

    //"Constructor" if UI is used
    public static void Init(int numberOfEnemies)
    {
        _uiRef = UIHolder.UIInstance;
        _enemiesToMake = numberOfEnemies;
        _spawnedCharacters = new List<Character>();
        CreateEnemies();
        //HistoryHandler.AddToCurrentAction("Battle Start!");
        //HistoryHandler.FinalizeAction();
    }

    public static void Init(int numberOfEnemies, int numberOfTeams)
    {
        _uiRef = UIHolder.UIInstance;
        _enemiesToMake = numberOfEnemies;
        _spawnedCharacters = new List<Character>();
        CreateEnemies(numberOfTeams);
        _spawnedCharacters = RollForInitiative(_spawnedCharacters);
        //HistoryHandler.AddToCurrentAction("Battle Start!");
        //HistoryHandler.FinalizeAction();
    }

    public static void Init(int numberOfEnemies, List<DraggableCharacter> playerstospawn)
    {
        _uiRef = UIHolder.UIInstance;
        _enemiesToMake = numberOfEnemies;
        _spawnedCharacters = new List<Character>();
        CreatePlayerTeam(playerstospawn);
        CreateEnemies(numberOfEnemies);
        _spawnedCharacters = RollForInitiative(_spawnedCharacters);
        //HistoryHandler.AddToCurrentAction("Battle Start!");
        //HistoryHandler.FinalizeAction();
    }

    //spawns enemies and adds them to the Data Grid in the GridHandler
    static void CreateEnemies()
    {
        for (int i = 0; i < _enemiesToMake; i++)
        {
            Vector2 spawn = GridHandler.GetSpawn(false);
            Character newCharacter = new EnemyClass("Enemy", (i+1).ToString(), TeamType.ENEMY, spawn);
            GridHandler.PlaceEnemyOnBoard(newCharacter);
            _spawnedCharacters.Add(newCharacter);
        }
    }
    static void CreateEnemies(int numberOfEnemiesToSpawn)
    {
        for (int i = 0; i < _enemiesToMake; i++)
        {
            Vector2 spawn = GridHandler.GetSpawn(false);
            Character newCharacter = new EnemyClass("Enemy", (i + 1).ToString(), TeamType.ENEMY, spawn);
            GridHandler.PlaceEnemyOnBoard(newCharacter);
            _spawnedCharacters.Add(newCharacter);
        }
    }
    /*static void CreateEnemies(int numberOfTeams)
    {
        for (int v = 0; v < numberOfTeams; v++)
        {
            for (int i = 0; i < _enemiesToMake; i++)
            {
                Vector2 spawn = GridHandler.GetSpawn();
                Character newCharacter = new Character("Enemy", (i + 1 + (v *_enemiesToMake)).ToString(), v + 1, spawn);
                GridHandler.PlaceEnemyOnBoard(newCharacter);
                _spawnedCharacters.Add(newCharacter);
            }
        }
    }*/
    static void CreatePlayerTeam(List<DraggableCharacter> playerTeam)
    {
        for (int i = 0; i < playerTeam.Count; i++)
        {
            Vector2 spawn = GridHandler.GetSpawn(true);
            Character newCharacter;
            switch (playerTeam[i].GetClass)
            {
                case PlayerClasses.WARRIOR:
                    newCharacter = new Warrior("Xavier", "0", TeamType.PLAYER, spawn);
                    break;
                case PlayerClasses.MAGE:
                    newCharacter = new Mage();
                    break;
                default:
                    newCharacter = new Character();
                    break;
            }

            GridHandler.PlacePlayerOnBoard(newCharacter);
            _spawnedCharacters.Add(newCharacter);
        }
    }

    //Looks through all of the players and enemies and sorts them by how fast they are
    //by checking Speed stat
    static List<Character> RollForInitiative(List<Character> enemies)
    {
        List<Character> unsortedEnemies = new List<Character>(enemies);
        List<Character> sortedEnemies = new List<Character>();

        while(sortedEnemies.Count != _spawnedCharacters.Count)
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
            //Debug.Log(sortedEnemies[i].Name + ", " + sortedEnemies[i].Team + ", " + sortedEnemies[i].Speed);
        }

        return sortedEnemies;
    }

    static public void CheckForEnd(Character removable)
    {
        Debug.Log("ending called");
        GridHandler.ClearSpace(removable.CurrentPosition);
        _spawnedCharacters.Remove(removable);

        if(_spawnedCharacters.Count > 1)
        {
            Character check = _spawnedCharacters[0];
            for (int i = 0; i < _spawnedCharacters.Count; i++)
            {
                if (_spawnedCharacters[i].Team != check.Team)
                {
                    return;
                }
            }
        }
        else
        {
            //Debug.Log("1 left");
            
        }
    }

    static public Character FindRandomEnemy(Character me, TeamType team)
    {
        //Debug.Log("finding enemies");
        int randNum = Random.Range(0, _spawnedCharacters.Count);

        try
        {
            while (me == _spawnedCharacters[randNum] || _spawnedCharacters[randNum].IsDead || _spawnedCharacters[randNum].Team == team)
            {
               
                randNum = Random.Range(0, _spawnedCharacters.Count);
            }
        }
        catch
        {
            //Debug.Log("died in finding");
        }

        return _spawnedCharacters[randNum];
    }

    //Attack a random Enemy
    static public void AttackEnemy(Character attacker, Character target)
    {
        //Debug.Log("attack called");
        //add beginning of statment
        //HistoryHandler.AddToCurrentAction(attacker.Name + " attacks " + target.Name);

        //add what took place on the turn (HIT, CRIT, or MISS)
        int randNum = Random.Range(0, 100);

        string damageString = "";

        if(attacker.Offense.Accuracy > randNum || randNum == 0)
        {
            int damage = attacker.Offense.Strength + attacker.HeldWeapon.StrengthMod;
            string tempDamage = damage.ToString();
            randNum = Random.Range(0, 100);
            if (attacker.Offense.CriticalChance > randNum || randNum == 0)
            {
                damage *= 2;
                tempDamage = damage.ToString() + "!";
                //HistoryHandler.AddToCurrentAction("and CRITS for " + Mathf.Abs(target.Defense.CalculateDamage(damage, attacker.HeldWeapon.WeaponElement, true)).ToString() + "\n");
            }

            int finalDamage = target.Defense.CalculateDamage(damage, attacker.HeldWeapon.WeaponElement);
            

            target.TakeDamage(finalDamage);
            target.LastAttacker = attacker;
        }
        else
        {
            damageString = "MISS";
        }
        WorldGridHandler.WorldInstance.SpawnDamageUI(target.CurrentPosition, damageString);
        CheckDeath(target);
    }

    static public void AbilityEnemy(int damage, DamageType element, Character target)
    {
        string damageString = "";

        int FinalDamage = target.Defense.CalculateDamage(damage, element);

        target.TakeDamage(FinalDamage);

        damageString += FinalDamage;

        WorldGridHandler.WorldInstance.SpawnDamageUI(target.CurrentPosition, damageString);
        CheckDeath(target);
    }

    static private void CheckDeath(Character check)
    {
        Debug.Log("Death called");
        if(check.CurrHealth <= 0)
        {
            Debug.Log("character dead");
            CheckForEnd(check);
        }
    }

    static public void ForceEndTurn()
    {
        WorldGridHandler.WorldInstance.CharacterForceEnd();
    }
}
