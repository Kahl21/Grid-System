
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    CharacterStrategy _stratRef;

    int _myTeamNumber;
    public int Team { get { return _myTeamNumber; } }


    string _myName;
    public string Name { get { return _myName; } }

    Weapon _myWeapon;
    public Weapon HeldWeapon { get { return _myWeapon; } }
    
    List<Ability> _myAbilites;
    public List<Ability> Abilities { get { return _myAbilites; } set { _myAbilites = value; } }

    int _myHealth;
    int _myMaxHealth;
    public int CurrHealth { get { return _myHealth; } set { _myHealth = value; } }
    public int MaxHealth { get { return _myMaxHealth; } }

    int _myMana;
    int _myMaxMana;

    public int CurrMana { get { return _myMana; } set { _myMana = value; } }
    public int MaxMana { get { return _myMaxMana; } }

    DefenseStats _myDefenses;
    public DefenseStats Defense { get { return _myDefenses; } }

    AttackStats _myOffense;
    public AttackStats Offense { get { return _myOffense; } }

    int _mySpeed;
    public int Speed { get { return _mySpeed; } }

    int _myMovement;
    public int Movement { get { return _myMovement; } }
    
    bool _dead;
    public bool IsDead { get { return _dead; } }

    string _myTileInfo;
    public string GetInfoIdentifier { get { return _myTileInfo; } }

    Vector2 _gridPos;
    public Vector2 CurrentPosition { get { return _gridPos; } set { _gridPos = value; } }

    List<Vector2> _myMoves;
    public List<Vector2> SpacesICanMove { get { return _myMoves; } }

    Character _lastToHitMe;
    public Character LastAttacker { get { return _lastToHitMe; } set { _lastToHitMe = value; } }

    //base constructor
    public Character()
    {
        _myName = "Bert";
        _myHealth = 100;
        _myMaxHealth = 100; 
        
        _myMana = 100;
        _myMaxMana = 100;

        _myOffense = new AttackStats(10, 10, 75, 25);
        _myDefenses = new DefenseStats(10, 0, 0, 0, 0, 0, 0);

        _myWeapon = new Weapon();

        _mySpeed = 10;
        _myMovement = 2;
        _gridPos = Vector2.zero;
        _stratRef = new CharacterStrategy();
        _dead = false;
    }

    //randomized constructor
    public Character(string name, string tileMoniker, int teamNumber, Vector2 startPos)
    {
        _myTeamNumber = teamNumber;
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = Random.Range(50, 301);
        _myMaxHealth = _myHealth; 
        _myMana = 100;
        _myMaxMana = 100;

        int _myStrength = Random.Range(5, 41);
        int _myMagic = Random.Range(5, 41);
        int _myAccuracy = Random.Range(10, 101);
        int _myCritChance = Random.Range(10, 101);
        _myOffense = new AttackStats(_myStrength, _myMagic, _myAccuracy, _myCritChance);

        int _baseDef = Random.Range(10, 31);
        int _physRes = UnityEngine.Random.Range(-100, 101);
        int _fireRes = Random.Range(-100, 101);
        int _iceRes = Random.Range(-100, 101);
        int _thunderRes = Random.Range(-100, 101);
        int _lightRes = Random.Range(-100, 101);
        int _darkRes = Random.Range(-100, 101);
        _myDefenses = new DefenseStats(_baseDef, _physRes, _fireRes, _iceRes, _thunderRes, _lightRes, _darkRes);

        Weapons newWeapon = (Weapons)Random.Range(0, (int)Weapons.GAUNTLET + 1);
        switch (newWeapon)
        {
            case Weapons.SWORD:
                _myWeapon = new Sword();
                break;
            case Weapons.SPEAR:
                _myWeapon = new Spear();
                break;
            case Weapons.AXE:
                _myWeapon = new Axe();
                break;
            case Weapons.BOW:
                _myWeapon = new Bow();
                break;
            case Weapons.STAFF:
                _myWeapon = new Staff();
                break;
            case Weapons.GAUNTLET:
                _myWeapon = new Gauntlets();
                break;
            default:
                break;
        }

        _mySpeed = Random.Range(1, 101);
        _myMovement = Random.Range(1, 3);
        _gridPos = startPos;
        _stratRef = new CharacterStrategy(this);
        _dead = false;
    }

    //incredibly specific constructor
    public Character(string name, string tileMoniker, int teamNumber, int hp, int mp, int att, int mag, int acc, int crit, int baseDef, int physRes, int fireRes, int iceRes, int thunRes, int lightRes, int darkRes,  int speed,  int move, Weapons weapon, Vector2 startPos)
    {
        _myTeamNumber = teamNumber;
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = hp;
        _myMaxHealth = hp;
        _myMana = mp;
        _myMaxMana = mp;

        _myOffense = new AttackStats(att, mag, acc, crit);
        _myDefenses = new DefenseStats(baseDef, physRes, fireRes, iceRes, thunRes, lightRes, darkRes);

        switch (weapon)
        {
            case Weapons.SWORD:
                _myWeapon = new Sword();
                break;
            case Weapons.SPEAR:
                _myWeapon = new Spear();
                break;
            case Weapons.AXE:
                _myWeapon = new Axe();
                break;
            case Weapons.BOW:
                _myWeapon = new Bow();
                break;
            case Weapons.STAFF:
                _myWeapon = new Staff();
                break;
            case Weapons.GAUNTLET:
                _myWeapon = new Gauntlets();
                break;
            default:
                break;
        }

        _mySpeed = speed;
        _myMovement = move;
        _gridPos = startPos;
        _stratRef = new CharacterStrategy(this);
        _dead = false;
    }

    //Main method called
    //starts this units turn
    public void TakeAction()
    {
        if(!_dead)
        {
            //Debug.Log(Name + "Action Taken");
            _stratRef.WhatDo();
        }
    }

    //Moves unit randomly to any space it can travel
    public void Move()
    {
        _myMoves = GridHandler.WhereCanIMove(_gridPos, _myMovement);
        int randNum = Random.Range(0, _myMoves.Count);
        GridHandler.MoveEnemy(this, _myMoves[randNum]);
    }

    //Moves towards the targeted Unit
    //Uses Dijstrka to find its path (not implemented)
    //follows path for as long as it has movement
    public void Move(Character target)
    {
        _myMoves = GridHandler.GetShortestMoves(this, target.CurrentPosition);
        LastAttacker = target;
        if(!GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeamNumber, _lastToHitMe))
        {
            if (_myMoves.Count > Movement)
            {
                GridHandler.MoveEnemy(this, _myMoves[Movement - 1]);
            }
            else if (_myMoves.Count == 0)
            {
                GridHandler.MoveEnemy(this, CurrentPosition);
            }
            else
            {
                GridHandler.MoveEnemy(this, _myMoves[0]);
            }
        }
        
    }

    //checks for enemies nearby and attacks
    //indiscriminately
    public void Attack()
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeamNumber))
        {
            List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, _myWeapon, _myTeamNumber);
            if(_enemiesNearMe.Count > 0)
            {
                //Debug.Log("attacking random");
                int randNum = Random.Range(0, _enemiesNearMe.Count);

                FightHandler.AttackEnemy(this, _enemiesNearMe[randNum]);
            }
            else
            {
                //Debug.Log("no enemy");
            }
        }
    }

    //attacks target if it is in range
    //if calls base attack method
    public void Attack(Character currTarget)
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeamNumber, currTarget))
        {
            //Debug.Log("attacking target");
            FightHandler.AttackEnemy(this, currTarget);
        }
        else
        {
            Attack();
        }
    }

    //Use Skill passed to it
    //gets all enemies near and blindly choose one to hit
    //regardless of Spell Shape
    public void UseSkill(Ability currSkill)
    {
        List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill, _myTeamNumber); 
        int rand = Random.Range(0, _enemiesNearMe.Count);
        if(_enemiesNearMe.Count > 0)
        {
            _enemiesNearMe = GridHandler.GetTargetsInSplashZone(_enemiesNearMe[rand].CurrentPosition, _gridPos, currSkill);

            currSkill.ActivateSkill(this, _enemiesNearMe);
        }

    }

    //Use Skill passed to it
    //gets the enemy that is being targeted
    //checks the splash zone for collatoral
    public void UseSkill(Ability currSkill, Character currTarget)
    {
        List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill, _myTeamNumber, currTarget);

        //Debug.Log("using " + currSkill.Name);
        if(_enemiesNearMe.Count > 0)
        {
            _enemiesNearMe = GridHandler.GetTargetsInSplashZone(_enemiesNearMe[0].CurrentPosition, _gridPos, currSkill);

            currSkill.ActivateSkill(this, _enemiesNearMe);
        }
    }

    //Use heal Skill passed to it
    //gets al enemies near and blindly choose one to hit
    public void UseHeal(Ability currSkill)
    {
        List<Character> _enemiesNearMe = new List<Character>();
        _enemiesNearMe.Add(this);
        currSkill.ActivateSkill(this, _enemiesNearMe);
    }

    //Use Heal Skill passed to it
    //gets all Allies nearby and chooses one with the least health
    public void UseHeal(Ability currSkill, Character currTarget)
    {
        //List<Character> _enemiesNearMe = GridHandler.GetAlliesinRange(_gridPos, currSkill);
        //currSkill.ActivateSkill(this, _enemiesNearMe, currTarget);
    }

    //called by FightHandler
    //calculates damage and subtracts damage from health
    //if health is less than or equal to 0, character dies
    public void TakeDamage(int damage, DamageType element, bool crit)
    {
        int damageTaken = _myDefenses.CalculateDamage(damage, element, crit);
        _myHealth += damageTaken;

        if(_myHealth <= 0)
        {
            HistoryHandler.DeclareDeath(this);
            _dead = true;
        }
    }
}

//struct for damage variables
public struct AttackStats
{
    public int Strength;
    public int Magic;
    public int Accuracy;
    public int CriticalChance;

    public AttackStats(int str, int mag, int acc, int crit)
    {
        Strength = str;
        Magic = mag;
        Accuracy = acc;
        CriticalChance = crit;
    }
}

//struct for defense variables
//also calculates damage taken and pass back to TakeDamage()
public struct DefenseStats
{
    public int BaseDefense;
    public int PhysRes;
    public int FireRes;
    public int IceRes;
    public int ThunderRes;
    public int LightRes;
    public int DarkRes;

    public DefenseStats(int def, int physRes, int fireRes, int iceRes, int thunRes, int lightRes, int darkRes)
    {
        BaseDefense = def;
        PhysRes = physRes;
        FireRes = fireRes;
        IceRes = iceRes;
        ThunderRes = thunRes;
        LightRes = lightRes;
        DarkRes = darkRes;
    }

    public int CalculateDamage(int damage, DamageType damageType, bool crit)
    {
        int actualDamage = 0;
        float currResistance = 0;

        switch (damageType)
        {
            case DamageType.PHYSICAL:
                currResistance = PhysRes;
                break;
            case DamageType.FIRE:
                currResistance = FireRes;
                break;
            case DamageType.ICE:
                currResistance = IceRes;
                break;
            case DamageType.THUNDER:
                currResistance = ThunderRes;
                break;
            case DamageType.LIGHT:
                currResistance = LightRes;
                break;
            case DamageType.DARK:
                currResistance = DarkRes;
                break;
            case DamageType.HEALING:
                actualDamage = damage;
                break;
            default:
                break;
        }

        actualDamage = (int)(BaseDefense - (damage * (1 - (currResistance / 100))));

        //Debug.Log(actualDamage);

        if (actualDamage > 0 && damageType != DamageType.HEALING)
        {
            actualDamage = 0;
        }

        if (crit)
        {
            actualDamage *= 2;
        }

        //Debug.Log(actualDamage);

        return actualDamage;
    }
}
