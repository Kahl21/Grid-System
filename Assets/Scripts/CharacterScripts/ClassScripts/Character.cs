
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{

    protected Strategy _stratRef;
    public Strategy Strategy { get { return _stratRef; } }

    protected TeamType _myTeam;
    public TeamType Team { get { return _myTeam; } }

    protected string _myClassName;
    public string ClassName { get { return _myClassName; } }

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


    protected int _currMovement;
    public int CurrentMovement { get { return _currMovement; } }
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
        _myClassName = "Default";
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
        _dead = false;
    }

    //randomized constructor
    public Character(string name, string tileMoniker, TeamType team, Vector2 startPos)
    {
        _myTeam = team;
        _myClassName = "Default";
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = UnityEngine.Random.Range(50, 201);
        _myMaxHealth = _myHealth; 
        _myMana = UnityEngine.Random.Range(50, 201);
        _myMaxMana = _myMana;

        int _myStrength = UnityEngine.Random.Range(5, 10);
        int _myMagic = UnityEngine.Random.Range(5, 10);
        int _myAccuracy = UnityEngine.Random.Range(10, 101);
        int _myCritChance = UnityEngine.Random.Range(10, 101);
        _myOffense = new AttackStats(_myStrength, _myMagic, _myAccuracy, _myCritChance);

        int _baseDef = UnityEngine.Random.Range(10, 11);
        int _physRes = UnityEngine.Random.Range(-100, 101);
        int _fireRes = UnityEngine.Random.Range(-100, 101);
        int _iceRes = UnityEngine.Random.Range(-100, 101);
        int _thunderRes = UnityEngine.Random.Range(-100, 101);
        int _lightRes = UnityEngine.Random.Range(-100, 101);
        int _darkRes = UnityEngine.Random.Range(-100, 101);
        _myDefenses = new DefenseStats(_baseDef, _physRes, _fireRes, _iceRes, _thunderRes, _lightRes, _darkRes);

        Weapons newWeapon = (Weapons)UnityEngine.Random.Range(0, (int)Weapons.GAUNTLET + 1);
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

        _mySpeed = UnityEngine.Random.Range(1, 101);
        _myMovement = UnityEngine.Random.Range(1, 3);
        _gridPos = startPos;
        _dead = false;
    }

    //incredibly specific constructor
    public Character(string name, string tileMoniker, TeamType team, int hp, int mp, int att, int mag, int acc, int crit, int baseDef, int PhysRes, int fireRes, int iceRes, int thunRes, int lightRes, int darkRes,  int speed,  int move, Weapons weapon, Vector2 startPos)
    {
        _myTeam = team;
        _myClassName = "Default";
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
        _dead = false;
    }

    //Main method called
    //starts this units turn
    public virtual void StartTurn()
    {
    }

    //Moves unit randomly to any space it can travel
    public virtual void Move()
    {
    }

    //Moves towards the targeted Unit
    //Uses Dijstrka to find its path (not implemented)
    //follows path for as long as it has movement
    public virtual void Move(Character target)
    {
    }

    public virtual bool CheckForMoreMove(int xpos, int ypos)
    {
        return false;
    }

    public virtual void StartAttack()
    {
        GridHandler.ShowReleventGrid(_gridPos, _myWeapon.Range, Color.red, Actions.ATTACK);
    }

    public virtual void Attack(Character target)
    {
        FightHandler.AttackEnemy(this, target);
    }

    //checks for enemies nearby and attacks
    //indiscriminately
    /*public void Attack()
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
    }*/

    //attacks target if it is in range
    //if calls base attack method
    /*public void Attack(Character currTarget)
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeam, currTarget))
        {
            FightHandler.AttackEnemy(this, currTarget);
        }
        else
        {
            Attack();
        }
    }*/

    //Use Skill passed to it
    //gets all enemies near and blindly choose one to hit
    //regardless of Spell Shape
    public virtual void UseSkill(Ability currSkill)
    {
        List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill, _myTeam); 
        int rand = UnityEngine.Random.Range(0, _enemiesNearMe.Count);
        if(_enemiesNearMe.Count > 0)
        {
            _enemiesNearMe = GridHandler.GetTargetsInSplashZone(_enemiesNearMe[rand].CurrentPosition,  currSkill);

            currSkill.ActivateSkill(this, _enemiesNearMe);
        }

    }

    //Use Skill passed to it
    //gets the enemy that is being targeted
    //checks the splash zone for collatoral
    public virtual void UseSkill(Ability currSkill, Character currTarget)
    {
        List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill, _myTeam, currTarget);

        //Debug.Log("using " + currSkill.Name);
        if(_enemiesNearMe.Count > 0)
        {
            _enemiesNearMe = GridHandler.GetTargetsInSplashZone(_enemiesNearMe[0].CurrentPosition, currSkill);

            currSkill.ActivateSkill(this, _enemiesNearMe);
        }
    }

    //Use heal Skill passed to it
    //gets al enemies near and blindly choose one to hit
    public virtual void UseHeal(Ability currSkill)
    {
        List<Character> _enemiesNearMe = new List<Character>();
        _enemiesNearMe.Add(this);
        currSkill.ActivateSkill(this, _enemiesNearMe);
    }

    //Use Heal Skill passed to it
    //gets all Allies nearby and chooses one with the least health
    public virtual void UseHeal(Ability currSkill, Character currTarget)
    {
        //List<Character> _enemiesNearMe = GridHandler.GetAlliesinRange(_gridPos, currSkill);
        //currSkill.ActivateSkill(this, _enemiesNearMe, currTarget);
    }


    public virtual void TakeDamage(int damage) 
    { 
        _myHealth += damage;
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

    public int CalculateDamage(int damage, DamageType damageType)
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
            default:
                currResistance = 0;
                break;
        }
        
        if(damageType != DamageType.HEALING)
        {
            actualDamage = (int)(BaseDefense - (damage * (1 - (currResistance / 100))));
        }

        if(actualDamage > 0 && damageType != DamageType.HEALING)
        {
            actualDamage = 0;
        }
        //DebugDamage(damage, currResistance);
        //Debug.Log(actualDamage);

        //Debug.Log(actualDamage);

        return actualDamage;
    }

    void DebugDamage(int dam, float res)
    {
        Debug.Log(BaseDefense.ToString() + " - (" + dam + " * (1 - (" + res + " / 100)))");
        Debug.Log(BaseDefense.ToString() + " - (" + dam + " * (1 - " + (res / 100).ToString() + "))");
        Debug.Log(BaseDefense.ToString() + " - (" + dam + " * " + (1 - (res / 100)).ToString() + ")");
        Debug.Log(BaseDefense.ToString() + " - " + (dam * (1 - (res / 100))).ToString());
        Debug.Log((BaseDefense - (dam * (1 - (res / 100)))).ToString());
    }
}
