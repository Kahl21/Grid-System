using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : Ability
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
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + ", and DEALS " + Mathf.Abs(targets[i].Defense.CalculateDamage(totalDamage, _element, false)).ToString() + " to " + targets[i].Name + "\n");
            targets[i].TakeDamage(totalDamage, _element, false);
        }
        activator.CurrMana -= _mpCost;
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
            if(targets[i] == activator)
            {
                HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name +  ", and HEALS themselves for " + Mathf.Abs(targets[i].Defense.CalculateDamage(totalDamage, _element, false)).ToString() + " Health" + "\n");
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
public class FlipFlop : Magic
{
    public FlipFlop() : base()
    {
        _name = "Flip Flop";
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
            HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + "\n");
            GridHandler.SwapEnemies(activator, targets[i]);
        }
        activator.CurrMana -= _mpCost;
    }
}

public class Chaos : Magic
{
    public Chaos() : base()
    {
        _name = "Chaos";
        _mpCost = 35;
        _damage = 0;
        _targetRange = 4;
        _damageRange = 4;
        _skillShape = DamageShape.SELFAREA;
        _element = DamageType.DARK;
    }

    public override void ActivateSkill(Character activator, List<Character> targets)
    {
        if(!targets.Contains(activator))
        {
            targets.Add(activator);
        }
        HistoryHandler.AddToCurrentAction(activator.Name + " CASTS " + _name + "!!!\n");
        GridHandler.SwapEnemies(targets);

        activator.CurrMana -= _mpCost;
    }
}