using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Ability
{

    public Skill() : base()
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
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + ", and DEALS " + Mathf.Abs(targets[i].Defense.CalculateDamage(totalDamage, _element, false)).ToString() + " to " + targets[i].Name + "\n");
            targets[i].TakeDamage(totalDamage, _element, false);
        }
        activator.CurrMana -= _mpCost;
    }
}

// --------------NON-ELEMENTAL SKILLS---------//
public class DoubleSlash : Skill
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
            FightHandler.AttackEnemy(activator, targets[i]);
            FightHandler.AttackEnemy(activator, targets[i]);
        }
    }
}

// --------------FIRE SKILLS---------//
public class FlameStrike : Skill
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
public class IcyStrike : Skill
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
public class ThunderSlash : Skill
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
public class DarkBlade : Skill
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
public class LightSlash : Skill
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
public class HealingPalm : Skill
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
        int totalDamage = activator.Offense.Strength + activator.HeldWeapon.MagicMod + _damage;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == activator)
            {
                HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + ", and HEALS themselves for " + Mathf.Abs(targets[i].Defense.CalculateDamage(totalDamage, _element, false)).ToString() + " Health" + "\n");
            }
            else
            {
                HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + " on " + targets[i].Name + ", and HEALS " + Mathf.Abs(targets[i].Defense.CalculateDamage(totalDamage, _element, false)).ToString() + " Health" + "\n");
            }
            targets[i].TakeDamage(totalDamage, _element, false);
        }
        activator.CurrMana -= _mpCost;
    }
}

// --------------HYBRID SKILLS---------//
public class WarpStrike : Skill
{
    public WarpStrike() : base()
    {
        _name = "WarpStrike";
        _mpCost = 20;
        _damage = 20;
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
            HistoryHandler.AddToCurrentAction(activator.Name + " DEALS " + Mathf.Abs(targets[i].Defense.CalculateDamage(totalDamage, _element, false)).ToString() + " damage to " + targets[i].Name + "\n");
            targets[i].TakeDamage(totalDamage, _element, false);
            GridHandler.SwapEnemies(activator, targets[i]);
        }
        activator.CurrMana -= _mpCost;
    }
}

public class Yi_Q : Skill
{
    public Yi_Q() : base()
    {
        _name = "Yi Q";
        _mpCost = 35;
        _damage = 15;
        _targetRange = 2;
        _damageRange = 2;
        _skillShape = DamageShape.SELFAREA;
        _element = DamageType.PHYSICAL;
    }

    public override void ActivateSkill(Character activator, List<Character> targets)
    {
        HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + "\n");
        for (int i = 0; i < targets.Count; i++)
        {
            int totalDamage = activator.Offense.Strength + activator.HeldWeapon.MagicMod + _damage;
            if (targets[i] != activator)
            {
                HistoryHandler.AddToCurrentAction(activator.Name + " DEALS " + Mathf.Abs(targets[i].Defense.CalculateDamage(totalDamage, _element, false)).ToString() + " damage to " + targets[i].Name + "\n");
                GridHandler.SwapEnemies(activator, targets[i]);
                targets[i].TakeDamage(totalDamage, _element, false);
            }
        }
        activator.CurrMana -= _mpCost;
    }
}