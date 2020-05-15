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
        _desc = "Blast your target with a ball of fire.";
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
        _desc = "Conjure an Ice Shard to impale your target.";
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
        _desc = "Call upon the cloud to strike your target with a jolt of Electricity.";
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
        _desc = "Envelope your target in darkness, and allow the horrors from beyond to destroy them.";
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
        _desc = "Strike you target with a Searing beam of light.";
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
        _desc = "Envelope you or an ally in a calm light, allowing them to recover their wounds.";
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
        _desc = "A childs spell, swaps the place of you and whatever you are looking at.";
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
        _desc = "A spell from the god of tricks, randomly swaps places of everyone targeted and gives them all a mild headache.";
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