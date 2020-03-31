using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellBook 
{
    static List<Abilities> _allSpells;
    public static List<Abilities> AllSpells { get { return _allSpells; } }

    public static void LoadSpellBook()
    {
        _allSpells = new List<Abilities>();
        _allSpells.Add(new DoubleSlash());
    }

}
