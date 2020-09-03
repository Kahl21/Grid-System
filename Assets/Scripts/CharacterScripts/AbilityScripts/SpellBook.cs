using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellBook
{
    static List<Ability> _allSpells;
    public static List<Ability> AllSpells { get { return _allSpells; } }

    public static void LoadSpellBook()
    {
        _allSpells = new List<Ability>();

        LoadSkills();
        LoadMagic();

        //Debug.Log(_allSpells.Count + ", Spells loaded");
    }

    static void LoadSkills()
    {
        _allSpells.Add(new DoubleSlash());
        _allSpells.Add(new FlameStrike());
        _allSpells.Add(new IcyStrike());
        _allSpells.Add(new ThunderSlash());
        _allSpells.Add(new LightSlash());
        _allSpells.Add(new DarkBlade());
        _allSpells.Add(new HealingPalm());
        _allSpells.Add(new WarpStrike());
        _allSpells.Add(new Yi_Q());
    }

    static void LoadMagic()
    {
        _allSpells.Add(new Fireball());
        _allSpells.Add(new Icicle());
        _allSpells.Add(new Thunder());
        _allSpells.Add(new Shine());
        _allSpells.Add(new Darkness());
        _allSpells.Add(new Heal());
        _allSpells.Add(new FlipFlop());
        _allSpells.Add(new Chaos());
    }

    public static List<Ability> AddAbilities<T>(int amountOfAbilitiesToLearn) where T : Ability
    {
        List<Ability> abilitiesToLearn = new List<Ability>();

        int randNum;

        for (int i = 0; i < amountOfAbilitiesToLearn; i++)
        {
            randNum = Random.Range(0, _allSpells.Count);

            //Debug.Log(typeof(T).ToString() + ", " + _allSpells[randNum].GetType().ToString());

            if (_allSpells[randNum].GetType().IsSubclassOf(typeof(T)) && !AlreadyLearnedSkill(_allSpells[randNum], abilitiesToLearn))
            {
                abilitiesToLearn.Add(_allSpells[randNum]);
                //Debug.Log("skill added");
            }
            else
            {
                i--;
                //Debug.Log("no skill added");
            }
        }

        return abilitiesToLearn;
    }

    public static Ability AddSpecificAbility(string abilityName)
    {
        for (int i = 0; i < _allSpells.Count; i++)
        {
            if (_allSpells[i].Name == abilityName)
            {
                return _allSpells[i];
            }
        }

        return null;
    }

    static bool AlreadyLearnedSkill<T>(T skillchecked, List<Ability> abiliesToCheck)
    {
        if (abiliesToCheck.Count > 0)
        {
            for (int i = 0; i < abiliesToCheck.Count; i++)
            {
                if (abiliesToCheck[i].GetType() == skillchecked.GetType())
                {
                    return true;
                }
            }
        }
        
        return false;
    }
}
