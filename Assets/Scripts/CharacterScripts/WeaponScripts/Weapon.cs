using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    PHYSICAL,
    FIRE,
    ICE,
    THUNDER,
    LIGHT,
    DARK,
    HEALING
}

public enum Weapons
{
    SWORD,
    SPEAR,
    AXE,
    BOW,
    STAFF,
    GAUNTLET
}

public class Weapon 
{
    protected string _name;
    public string Name { get { return _name; } }

    protected int _strengthMod;
    public int StrengthMod { get { return _strengthMod; } }

    protected int _magicMod;
    public int MagicMod { get { return _magicMod; } }

    protected int _accuracyMod;
    public int Accuracy { get { return _accuracyMod; } }

    protected int _critMod;
    public int Crit { get { return _critMod; } }

    protected int _attackRange;
    public int Range { get { return _attackRange; } }

    protected bool _ranged;
    public bool IsRanged { get {return _ranged; } }

    protected DamageType _damageType;
    public DamageType WeaponElement { get { return _damageType; } }

    public Weapon()
    {
        _name = "Weapon";
        _strengthMod = 10;
        _magicMod = 10;
        _accuracyMod = 0;
        _critMod = 0;
        _attackRange = 1;
        _ranged = false;
        _damageType = DamageType.PHYSICAL;
    }

    public Weapon(string name, int damage, int magicDamage, int accuracy, int critical, int range, bool isRanged, DamageType element)
    {
        _name = name;
        _strengthMod = damage;
        _magicMod = magicDamage;
        _accuracyMod = accuracy;
        _critMod = critical;
        _attackRange = range;
        _ranged = isRanged;
        _damageType = element;
    }
}

public class Sword : Weapon
{
    public Sword() : base ()
    {
        _name = "Sword";
        _strengthMod = Random.Range(20, 41);
        _magicMod = Random.Range(0, 16);
        _accuracyMod = Random.Range(0, 31);
        _critMod = Random.Range(-10, 11);
        _attackRange = 1;
        _ranged = false;
        _damageType = (DamageType)Random.Range(0, (int)DamageType.DARK + 1);
    }
}

public class Spear : Weapon
{
    public Spear() : base()
    {
        _name = "Spear";
        _strengthMod = Random.Range(25, 51);
        _magicMod = Random.Range(-10, 16);
        _accuracyMod = Random.Range(-5, 31);
        _critMod = Random.Range(-10, 21);
        _attackRange = 2;
        _ranged = false;
        _damageType = (DamageType)Random.Range(0, (int)DamageType.DARK + 1);
    }
}

public class Axe : Weapon
{
    public Axe() : base()
    {
        _attackRange = Random.Range(1,3);

        if(_attackRange == 1)
        {
            _name = "Axe";
            _strengthMod = Random.Range(30, 66);
            _magicMod = Random.Range(-30, 6);
            _accuracyMod = Random.Range(-20, 21);
            _critMod = Random.Range(-5, 16);
        }
        else
        {
            _name = "Hand Axe";
            _strengthMod = Random.Range(25, 46);
            _magicMod = Random.Range(-20, 11);
            _accuracyMod = Random.Range(-30, 16);
            _critMod = Random.Range(-20, 11);
        }

        _ranged = false;
        _damageType = (DamageType)Random.Range(0, (int)DamageType.DARK + 1);
    }
}

public class Bow : Weapon
{
    public Bow() : base()
    {
        _strengthMod = Random.Range(25, 51);
        _magicMod = Random.Range(-5, 21);
        _accuracyMod = Random.Range(-5, 21);
        _critMod = Random.Range(0, 21);
        _attackRange = Random.Range(2, 5);
        _ranged = true;
        _damageType = (DamageType)Random.Range(0, (int)DamageType.DARK + 1);

        switch (_attackRange)
        {
            case 2:
                _name = "Bow";
                break;
            case 3:
                _name = "Long Bow";
                break;
            case 4:
                _name = "Very Long Bow";
                break;
            default:
                break;
        }
    }
}

public class Staff : Weapon
{
    public Staff() : base()
    {
        _name = "Staff";
        _strengthMod = Random.Range(5, 21);
        _magicMod = Random.Range(10, 41);
        _accuracyMod = Random.Range(0, 36);
        _critMod = Random.Range(-5, 6);
        _attackRange = 1;
        _ranged = false;
        _damageType = (DamageType)Random.Range(1, (int)DamageType.DARK + 1);
    }
}

public class Gauntlets : Weapon
{
    public Gauntlets() : base()
    {
        _name = "Sword";
        _strengthMod = Random.Range(5, 51);
        _magicMod = Random.Range(0, 21);
        _accuracyMod = Random.Range(10, 36);
        _critMod = Random.Range(5, 21);
        _attackRange = 1;
        _ranged = false;
        _damageType = (DamageType)Random.Range(0, (int)DamageType.DARK + 1);
    }
}