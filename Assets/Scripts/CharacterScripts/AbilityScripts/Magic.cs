using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : Abilities
{
    public Magic() : base ()
    {
        _name = "Nothing Magic";
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
            int totalDamage = activator.Offense.Magic + activator.HeldWeapon.MagicMod + _damage;
            targets[i].TakeDamage(totalDamage, _element);
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + ", dealing " + totalDamage + "\n");
        }
    }
}

// --------------FIRE SKILLS---------//
public class Fireball : Magic
{ 
    public Fireball() : base()
    {
        _name = "Fireball";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 2;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.FIRE;
    }

}

// --------------ICE SKILLS---------//
public class Icicle : Magic
{
    public Icicle() : base()
    {
        _name = "Icicle";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 2;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.ICE;
    }
}

// --------------THUNDER SKILLS---------//
public class Thunder : Magic
{
    public Thunder() : base()
    {
        _name = "Thunder";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 2;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.THUNDER;
    }

}

// --------------DARK SKILLS---------//
public class Darkness : Magic
{
    public Darkness() : base()
    {
        _name = "Darkness";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 2;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.DARK;
    }
}

// --------------LIGHT SKILLS----------//
public class Shine : Magic
{
    public Shine() : base()
    {
        _name = "Shine";
        _mpCost = 10;
        _damage = 10;
        _targetRange = 2;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.LIGHT;
    }
}

// --------------HEAL SKILLS----------//
public class Heal : Magic
{
    public Heal() : base()
    {
        _name = "Heal";
        _mpCost = 15;
        _damage = 10;
        _targetRange = 2;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.HEALING;
    }
    public override void ActivateSkill(Character activator, List<Character> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            int totalDamage = activator.Offense.Magic + activator.HeldWeapon.MagicMod + _damage;
            targets[i].TakeDamage(totalDamage, _element);
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + " on " + targets[0] + ", healing " + totalDamage + "\n");
        }
    }
}

// --------------HYBRID SKILLS---------//
public class FlipFlop : Magic
{
    public FlipFlop() : base()
    {
        _name = "Flip-Flop";
        _mpCost = 20;
        _damage = 0;
        _targetRange = 4;
        _damageRange = 0;
        _skillShape = DamageShape.SINGLE;
        _element = DamageType.DARK;
    }

    public override void ActivateSkill(Character activator, List<Character> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            GridHandler.SwapPlaces(activator, targets[i]);
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + "\n" + activator.Name + " and " + targets[i] + " have swapped places!");
        }
    }
}