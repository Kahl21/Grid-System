using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellBook
{
    static List<Ability> _allSpells;
    public static List<Ability> AllSpells { get { return _allSpells; } }

    //loads all skills and magic
    public static void LoadSpellBook()
    {
        _allSpells = new List<Ability>();

        LoadSkills();
        LoadMagic();

        //Debug.Log(_allSpells.Count + ", Spells loaded");
    }

    //creates new instances of every skill
    //adds them to the master list
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

    //creates new instances of every magic
    //adds them to the master list
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

    //creates a list of abilities available for character
    //randomly assigns either "only skill", "only magic", or "all"
    //passes ability list back to character
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

    //used by AddAbilities<T>
    //checks to see if the current ability is the type that the character can use
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

    //used by AddAbilities<T>
    //checks to see if the current ability has already been learned by the character
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
