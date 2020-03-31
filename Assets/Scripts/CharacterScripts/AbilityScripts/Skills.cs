using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Skills : Abilities
{

    public Skills() : base()
    {
        _name = "Nothing Skill";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 2;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.PHYSICAL;
    }

    public override void ActivateSkill(Character activator, List<Character> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            int totalDamage = activator.Offense.Strength + activator.HeldWeapon.MagicMod + _damage;
            targets[i].TakeDamage(totalDamage, _element);
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + ", dealing " + totalDamage + "\n");
        }
    }
}

// --------------NON-ELEMENTAL SKILLS---------//
public class DoubleSlash : Skills
{
    public DoubleSlash() : base()
    {
        _name = "Double Slash";
        _mpCost = 20;
        _damage = 10;
        _targetRange = 1;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.PHYSICAL;
    }
    public override void ActivateSkill(Character activator, List<Character> targets)
    {

        HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + "\n");
        for (int i = 0; i < targets.Count; i++)
        {
            int totalDamage = activator.Offense.Strength + activator.HeldWeapon.StrengthMod + _damage;
            FightHandler.AttackEnemy(activator, targets[i]);
            FightHandler.AttackEnemy(activator, targets[i]);
        }
    }
}

// --------------FIRE SKILLS---------//
public class FlameStrike : Skills
{ 
    public FlameStrike() : base()
    {
        _name = "FireStrike";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 1;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.FIRE;
    }

}

// --------------ICE SKILLS---------//
public class IcyStrike : Skills
{
    public IcyStrike() : base()
    {
        _name = "Icy Strike";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 1;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.ICE;
    }
}

// --------------THUNDER SKILLS---------//
public class ThunderSlash : Skills
{
    public ThunderSlash() : base()
    {
        _name = "Thunder Slash";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 1;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.THUNDER;
    }

}

// --------------DARK SKILLS---------//
public class DarkBlade : Skills
{
    public DarkBlade() : base()
    {
        _name = "Dark Blade";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 1;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.DARK;
    }
}

// --------------LIGHT SKILLS----------//
public class LightSlash : Skills
{
    public LightSlash() : base()
    {
        _name = "Light Slash";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 1;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.LIGHT;
    }
}

// --------------HEAL SKILLS----------//
public class HealingPalm : Skills
{
    public HealingPalm() : base()
    {
        _name = "Healing Palm";
        _mpCost = 15;
        _damage = 0;
        _targetRange = 1;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.HEALING;
    }
    public override void ActivateSkill(Character activator, List<Character> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            int totalDamage = activator.Offense.Strength + activator.HeldWeapon.MagicMod + _damage;
            targets[i].TakeDamage(totalDamage, _element);
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + " on " + targets[0] + ", healing " + totalDamage + "\n");
        }
    }
}

// --------------HYBRID SKILLS---------//
public class WarpStrike : Skills
{
    public WarpStrike() : base()
    {
        _name = "WarpStrike";
        _mpCost = 20;
        _damage = 15;
        _targetRange = 3;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.DARK;
    }

    public override void ActivateSkill(Character activator, List<Character> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            int totalDamage = activator.Offense.Strength + activator.HeldWeapon.MagicMod + _damage;
            targets[i].TakeDamage(totalDamage, _element);
            GridHandler.SwapPlaces(activator, targets[i]);
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + ", dealing " + totalDamage + "\n"
                + activator.Name + " and " + targets[i] + " have swapped places!");
        }
    }
}