
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    CharacterStrategy _stratRef;
    
    string _myName;
    public string Name { get { return _myName; } }

    Weapon _myWeapon;
    public Weapon HeldWeapon { get { return _myWeapon; } }

    int _myHealth;
    int _myMaxHealth;
    public int CurrHealth { get { return _myHealth; } set { _myHealth = value; } }
    public int MaxHealth { get { return _myMaxHealth; } }

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

    public Character()
    {
        _myName = "Bert";
        _myHealth = 100;
        _myMaxHealth = 100;

        _myOffense = new AttackStats(10, 10, 75, 25);
        _myDefenses = new DefenseStats(10, 0, 0, 0, 0, 0);

        _myWeapon = new Weapon();

        _mySpeed = 10;
        _myMovement = 2;
        _gridPos = Vector2.zero;
        _stratRef = new CharacterStrategy();
        _dead = false;
    }

    public Character(string name, string tileMoniker, Vector2 startPos)
    {
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = Random.Range(50, 201);
        _myMaxHealth = _myHealth;

        int _myStrength = Random.Range(5, 20);
        int _myMagic = Random.Range(5, 20);
        int _myAccuracy = Random.Range(10, 101);
        int _myCritChance = Random.Range(10, 101);
        _myOffense = new AttackStats(_myStrength, _myMagic, _myAccuracy, _myCritChance);

        int _baseDef = Random.Range(10, 51);
        int _fireRes = Random.Range(-100, 101);
        int _iceRes = Random.Range(-100, 101);
        int _thunderRes = Random.Range(-100, 101);
        int _lightRes = Random.Range(-100, 101);
        int _darkRes = Random.Range(-100, 101);
        _myDefenses = new DefenseStats(_baseDef, _fireRes, _iceRes, _thunderRes, _lightRes, _darkRes);

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
        _stratRef = new CharacterStrategy();
        _dead = false;
    }

    public Character(string name, string tileMoniker, int hp, int att, int mag, int acc, int crit, int baseDef, int fireRes, int iceRes, int thunRes, int lightRes, int darkRes,  int speed,  int move, Weapons weapon, Vector2 startPos)
    {
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = hp;
        _myMaxHealth = hp;

        _myOffense = new AttackStats(att, mag, acc, crit);
        _myDefenses = new DefenseStats(baseDef, fireRes, iceRes, thunRes, lightRes, darkRes);

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
        _stratRef = new CharacterStrategy();
        _dead = false;
    }

    public void TakeAction()
    {
        if(!_dead)
        {
            _myMoves = GridHandler.WhereCanIMove(_gridPos, _myMovement);
            _stratRef.WhatDo(this);
            //Debug.Log("Action Taken");
        }
    }

    public void Move()
    {
        int randNum = Random.Range(0, _myMoves.Count);
        GridHandler.MoveEnemy(this, _myMoves[randNum]);
    }

    public void Move(Character target)
    {
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

    public void Attack()
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon))
        {
            List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, _myWeapon);
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

    public void Attack(Character currTarget)
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, currTarget))
        {
            FightHandler.AttackEnemy(this, currTarget);
        }
        else
        {
            Attack();
        }
    }

    public void UseSkill(Abilities currSkill, Character currTarget)
    { 
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, currSkill, currTarget))
        {
            List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill);
            currSkill.ActivateSkill(this, _enemiesNearMe);
        }
        else
        {
            return;
        }
    }

    public void TakeDamage(int damage, DamageType element)
    {
        int damageTaken = _myDefenses.CalculateDamage(damage, element);
        _myHealth -= damageTaken;

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
    public int FireRes;
    public int IceRes;
    public int ThunderRes;
    public int LightRes;
    public int DarkRes;

    public DefenseStats(int def, int fireRes, int iceRes, int thunRes, int lightRes, int darkRes)
    {
        BaseDefense = def;
        FireRes = fireRes;
        IceRes = iceRes;
        ThunderRes = thunRes;
        LightRes = lightRes;
        DarkRes = darkRes;
    }

    public int CalculateDamage(int damage, DamageType damageType)
    {
        int actualDamage = 0;

        switch (damageType)
        {
            case DamageType.PHYSICAL:
                actualDamage = BaseDefense - damage;
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

        return Mathf.Abs(actualDamage);
    }
}
