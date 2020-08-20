using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageShape
{
    SINGLE,
    STAR,
    SELFAREA,
    LINE,
    CONE,
    ALL
}

public class Ability
{
    protected string _name;
    public string Name { get { return _name; } }

    protected string _desc;
    public string Description { get { return _desc; } }

    protected int _mpCost;
    public int Cost { get { return _mpCost; } }

    protected int _damage;
    public int SpellDamage { get { return _damage; } }

    protected int _targetRange;
    public int TargetRange { get { return _targetRange; } }

    protected int _damageRange;
    public int SplashRange { get { return _damageRange; } }

    protected DamageShape _skillShape;
    public DamageShape Shape { get { return _skillShape; } }

    protected DamageType _element;
    public DamageType Element { get { return _element; } }


    public Ability()
    {
        _name = "Nothing";
        _desc = "Base ability no skill here";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 2;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.PHYSICAL;
    }

    public virtual void ActivateSkill(Character activator, List<Character> targets)
    {

    }
}