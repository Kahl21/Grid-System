
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    protected CharacterStrategy _stratRef;

    protected TeamType _myTeam;
    public TeamType Team { get { return _myTeam; } }

    protected string _myName;
    public string Name { get { return _myName; } }

    protected Sprite _mySprite;
    public Sprite GetSprite { get { return _mySprite; } }

    protected string _myDescription;
    public string Description { get { return _myDescription; } }

    protected Weapon _myWeapon;
    public Weapon HeldWeapon { get { return _myWeapon; } }

    protected List<Ability> _myAbilites;
    public List<Ability> Abilities { get { return _myAbilites; } set { _myAbilites = value; } }

    protected int _myHealth;
    protected int _myMaxHealth;
    public int CurrHealth { get { return _myHealth; } set { _myHealth = value; } }
    public int MaxHealth { get { return _myMaxHealth; } }

    protected int _myMana;
    protected int _myMaxMana;

    public int CurrMana { get { return _myMana; } set { _myMana = value; } }
    public int MaxMana { get { return _myMaxMana; } }

    protected DefenseStats _myDefenses;
    public DefenseStats Defense { get { return _myDefenses; } }

    protected AttackStats _myOffense;
    public AttackStats Offense { get { return _myOffense; } }

    protected int _mySpeed;
    public int Speed { get { return _mySpeed; } }

    protected int _myMovement;
    public int Movement { get { return _myMovement; } }

    protected bool _dead;
    public bool IsDead { get { return _dead; } }

    protected string _myTileInfo;
    public string GetInfoIdentifier { get { return _myTileInfo; } }

    protected Vector2 _gridPos;
    public Vector2 CurrentPosition { get { return _gridPos; } set { _gridPos = value; } }

    protected List<Vector2> _myMoves;
    public List<Vector2> SpacesICanMove { get { return _myMoves; } }

    protected Character _lastToHitMe;
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
    public Character(string name, string tileMoniker, TeamType team, Vector2 startPos)
    {
        _myTeam = team;
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = Random.Range(50, 201);
        _myMaxHealth = _myHealth; 
        _myMana = 100;
        _myMaxMana = 100;

        int _myStrength = Random.Range(5, 20);
        int _myMagic = Random.Range(5, 20);
        int _myAccuracy = Random.Range(10, 101);
        int _myCritChance = Random.Range(10, 101);
        _myOffense = new AttackStats(_myStrength, _myMagic, _myAccuracy, _myCritChance);

        int _baseDef = Random.Range(10, 51);
        int _physRes = Random.Range(-100, 101);
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
    public Character(string name, string tileMoniker, TeamType team, int hp, int mp, int att, int mag, int acc, int crit, int baseDef, int PhysRes, int fireRes, int iceRes, int thunRes, int lightRes, int darkRes,  int speed,  int move, Weapons weapon, Vector2 startPos)
    {
        _myTeam = team;
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = hp;
        _myMaxHealth = hp;
        _myMana = mp;
        _myMaxMana = mp;

        _myOffense = new AttackStats(att, mag, acc, crit);
        _myDefenses = new DefenseStats(baseDef, PhysRes, fireRes, iceRes, thunRes, lightRes, darkRes);

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
            _stratRef.WhatDo();
            //Debug.Log("Action Taken");
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
        _myMoves = GridHandler.WhereCanIMove(_gridPos, _myMovement);
        LastAttacker = target;
        Vector2 movingTo = CurrentPosition;

        for (int i = 0; i < _myMoves.Count; i++)
        {
            if (target.CurrentPosition.x >= _gridPos.x && target.CurrentPosition.y >= _gridPos.y)
            {
                if (_myMoves[i].x >= movingTo.x && _myMoves[i].y >= movingTo.y)
                {
                    movingTo = _myMoves[i];
                }
            }
            else if (target.CurrentPosition.x >= _gridPos.x && target.CurrentPosition.y <= _gridPos.y)
            {
                if (_myMoves[i].x >= movingTo.x && _myMoves[i].y <= movingTo.y)
                {
                    movingTo = _myMoves[i];
                }
            }
            else if (target.CurrentPosition.x <= _gridPos.x && target.CurrentPosition.y >= _gridPos.y)
            {
                if (_myMoves[i].x <= movingTo.x && _myMoves[i].y >= movingTo.y)
                {
                    movingTo = _myMoves[i];
                }
            }
            else if (target.CurrentPosition.x <= _gridPos.x && target.CurrentPosition.y <= _gridPos.y)
            {
                if (_myMoves[i].x <= movingTo.x && _myMoves[i].y <= movingTo.y)
                {
                    movingTo = _myMoves[i];
                }
            }
            
        }

        GridHandler.MoveEnemy(this, movingTo);
    }

    //checks for enemies nearby and attacks
    //indiscriminately
    public void Attack()
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeam))
        {
            List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, _myWeapon, _myTeam);
            if(_enemiesNearMe.Count > 0)
            {
                int randNum = Random.Range(0, _enemiesNearMe.Count);

                FightHandler.AttackEnemy(this, _enemiesNearMe[randNum]);
            }
            else
            {
                Debug.Log("no enemy");
            }
        }
    }

    //attacks target if it is in range
    //if calls base attack method
    public void Attack(Character currTarget)
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeam, currTarget))
        {
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
        List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill, _myTeam); 
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
        List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill, _myTeam, currTarget);

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


    public void TakeDamage(int damage, DamageType element, bool crit)
    {
        int damageTaken = _myDefenses.CalculateDamage(damage, element, crit);
        _myHealth += damageTaken;

        if(_myHealth < 0)
        {
            HistoryHandler.DeclareDeath(this);
            _dead = true;
        }
    }
}

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

        switch (damageType)
        {
            case DamageType.PHYSICAL:
                actualDamage = BaseDefense - (damage - (damage * (PhysRes / 100)));
                break;
            case DamageType.FIRE:
                actualDamage = BaseDefense - (damage - (damage * (FireRes / 100)));
                break;
            case DamageType.ICE:
                actualDamage = BaseDefense - (damage - (damage * (IceRes / 100)));
                break;
            case DamageType.THUNDER:
                actualDamage = BaseDefense - (damage - (damage * (ThunderRes / 100)));
                break;
            case DamageType.LIGHT:
                actualDamage = BaseDefense - (damage - (damage * (LightRes / 100)));
                break;
            case DamageType.DARK:
                actualDamage = BaseDefense - (damage - (damage * (DarkRes / 100)));
                break;
            case DamageType.HEALING:
                actualDamage = damage;
                break;
            default:
                break;
        }

        if(actualDamage > 0 && damageType != DamageType.HEALING)
        {
            actualDamage = 0;
        }

        if(crit)
        {
            actualDamage *= 2;
        }

        //Debug.Log(actualDamage);

        return actualDamage;
    }
}
