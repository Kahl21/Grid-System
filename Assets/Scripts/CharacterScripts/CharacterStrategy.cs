using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStrategy 
{
    private enum Strats
    {
        BLOODTHIRSTY,
        SKILLFUL,
        MAGICAL,
        COMPLETELY_RANDOM,
        FROM_AFAR,
    }

    Strats _myStrat;
    Character _currTarget;
    int _abilitesICanRemeber;
    List<Abilities> _myAbilites;

    public CharacterStrategy()
    {
        //int stratNum = Random.Range(0, (int)Strats.FROM_AFAR + 1);
        //_myStrat = (Strats)stratNum;
        _myStrat = Strats.SKILLFUL;

        _abilitesICanRemeber = Random.Range(1, 2);
        _myAbilites = new List<Abilities>();


        List<Abilities> currCheck = SpellBook.AllSpells;

        switch (_myStrat)
        {
            case Strats.BLOODTHIRSTY:
                break;
            case Strats.SKILLFUL:
                foreach (Skills item in currCheck)
                {
                    _myAbilites.Add(item);
                }

                while(_myAbilites.Count > _abilitesICanRemeber)
                {
                    int rand = Random.Range(0, _myAbilites.Count);
                    _myAbilites.RemoveAt(rand);
                }
                break;
            case Strats.MAGICAL:
                break;
            case Strats.COMPLETELY_RANDOM:
                break;
            case Strats.FROM_AFAR:
                break;
            default:
                break;
        }

        _currTarget = null;
    }

    public void WhatDo(Character me)
    {
        if (_currTarget == null || _currTarget.IsDead)
        {
            _currTarget = FightHandler.FindRandomEnemy(me);
        }


        //Debug.Log("what do");
        switch (_myStrat)
        {
            case Strats.BLOODTHIRSTY:
                me.Move(_currTarget);
                me.Attack(_currTarget);
                break;
            case Strats.SKILLFUL:
                me.Move(_currTarget);
                me.UseSkill(_myAbilites[0], _currTarget);
                break;
            default:
                break;
        }
    }
}
