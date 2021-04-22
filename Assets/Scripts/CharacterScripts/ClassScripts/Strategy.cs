using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strategy
{
    protected enum AbilityStrats
    {
        SKILLFUL,
        MAGICAL,
        COMPLETELY_RANDOM,
    }

    protected enum BattleStrats
    {
        BLOODTHIRSTY,
        CAUTIOUS,
        RANGED,
    }


    protected Character _me;

    protected bool _hasAttacked, _hasMoved;
    public bool HasAttacked { get { return _hasAttacked; } set { _hasAttacked = value; } }
    public bool HasMoved { get { return _hasMoved; } set { _hasMoved = value; } }
    protected int _abilitesICanRemeber;

    public Strategy()
    {
        AbilityStrats _startSkills = AbilityStrats.COMPLETELY_RANDOM;

        _abilitesICanRemeber = 1;
        _me.Abilities = new List<Ability>();

        switch (_startSkills)
        {
            case AbilityStrats.SKILLFUL:
                _me.Abilities = SpellBook.AddAbilities<Skill>(_abilitesICanRemeber);
                break;
            case AbilityStrats.MAGICAL:
                _me.Abilities = SpellBook.AddAbilities<Magic>(_abilitesICanRemeber);
                break;
            case AbilityStrats.COMPLETELY_RANDOM:
                _me.Abilities = SpellBook.AddAbilities<Ability>(_abilitesICanRemeber);
                break;
            default:
                break;
        }
        
        _hasAttacked = false;
        _hasMoved = false;
    }

    public Strategy(Character characterToControl)
    {
        _me = characterToControl;

        int stratNum = Random.Range(0, (int)AbilityStrats.COMPLETELY_RANDOM + 1);
        AbilityStrats _startSkills = (AbilityStrats)stratNum;


        _abilitesICanRemeber = Random.Range(1, 6);
        _me.Abilities = new List<Ability>();

        //Debug.Log(_me.Name + " is " + _startSkills.ToString() + " and i have " + _abilitesICanRemeber + " abilities");

        switch (_startSkills)
        {
            case AbilityStrats.SKILLFUL:
                _me.Abilities = SpellBook.AddAbilities<Skill>(_abilitesICanRemeber);
                break;
            case AbilityStrats.MAGICAL:
                _me.Abilities = SpellBook.AddAbilities<Magic>(_abilitesICanRemeber);
                break;
            case AbilityStrats.COMPLETELY_RANDOM:
                _me.Abilities = SpellBook.AddAbilities<Ability>(_abilitesICanRemeber);
                break;
            default:
                break;
        }

        _hasAttacked = false;
        _hasMoved = false;
    }
    public Strategy(Character characterToControl, int skillType, int numOfSkills)
    {
        _me = characterToControl;

        AbilityStrats _startSkills = (AbilityStrats)skillType;


        _abilitesICanRemeber = numOfSkills;
        _me.Abilities = new List<Ability>();

        //Debug.Log(_me.Name + " is " + _startSkills.ToString() + " and i have " + _abilitesICanRemeber + " abilities");

        switch (_startSkills)
        {
            case AbilityStrats.SKILLFUL:
                _me.Abilities = SpellBook.AddAbilities<Skill>(_abilitesICanRemeber);
                break;
            case AbilityStrats.MAGICAL:
                _me.Abilities = SpellBook.AddAbilities<Magic>(_abilitesICanRemeber);
                break;
            case AbilityStrats.COMPLETELY_RANDOM:
                _me.Abilities = SpellBook.AddAbilities<Ability>(_abilitesICanRemeber);
                break;
            default:
                break;
        }

        _hasAttacked = false;
        _hasMoved = false;
    }

    public virtual void WhatDo()
    {
        _hasAttacked = false;
        _hasMoved = false;
    }

    public virtual void ContinueTurn()
    {

    }

    public virtual void StartWaiting()
    {

    }

    public virtual void WaitingTime()
    {

    }
}
