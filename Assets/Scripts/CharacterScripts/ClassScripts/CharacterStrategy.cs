using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStrategy 
{
    private enum AbilityStrats
    {
        SKILLFUL,
        MAGICAL,
        COMPLETELY_RANDOM,
    }

    private enum BattleStrats
    {
        BLOODTHIRSTY,
        CAUTIOUS,
        RANGED,
    }


    Character _me;

    BattleStrats _myStrat;
    bool _hasAttacked, _hasMoved;
    public bool HasAttacked { get { return _hasAttacked; } set { _hasAttacked = value; } }
    public bool HasMoved { get { return _hasMoved; } set { _hasMoved = value; } }
    int _abilitesICanRemeber;

    Character _currTarget;

    public CharacterStrategy()
    {
        AbilityStrats _startSkills = AbilityStrats.COMPLETELY_RANDOM;

        _myStrat = BattleStrats.BLOODTHIRSTY;

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
        _currTarget = null;
    }

    public CharacterStrategy(Character characterToControl)
    {
        _me = characterToControl;

        int stratNum = Random.Range(0, (int)AbilityStrats.COMPLETELY_RANDOM + 1);
        AbilityStrats _startSkills = (AbilityStrats)stratNum;

        stratNum = Random.Range(0, (int)BattleStrats.RANGED + 1);
        _myStrat = (BattleStrats)stratNum;

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
        _currTarget = null;
    }

    public void WhatDo()
    {
        if(_currTarget == null || _currTarget.IsDead)
        {
            _currTarget = FightHandler.FindRandomEnemy(_me, _me.Team);
        }

        //Debug.Log("what do");
        switch (_myStrat)
        {
            case BattleStrats.BLOODTHIRSTY:
                TryMove();
                TryAttack();
                TryAbilites(_currTarget);
                TryAbilites();
                break;
            case BattleStrats.CAUTIOUS:
                TryHeal(); 
                TryAbilites();
                TryAbilites(_currTarget);
                TryMove();
                TryAttack();
                break;
            case BattleStrats.RANGED:
                TryAbilites(_currTarget);
                TryAbilites();
                TryMove();
                TryHeal();
                TryAttack();
                break;
            default:
                break;
        }

        _hasAttacked = false;
        _hasMoved = false;
    }

    void TryMove()
    {
        if (!_hasMoved)
        {
            _me.Move(_currTarget);
            _hasMoved = true;
        }
    }

    void TryAttack()
    {
        if (!_hasAttacked)
        {
            if (GridHandler.CheckForEnemyWithinRange(_me.CurrentPosition, _me.HeldWeapon,_me.Team, _currTarget))
            {
                _me.Attack(_currTarget);
                _hasAttacked = true;
            }
            else if (GridHandler.CheckForEnemyWithinRange(_me.CurrentPosition, _me.HeldWeapon, _me.Team))
            {
                //_me.Attack();
                _hasAttacked = true;
            }
        }
    }

    void TryAbilites()
    {
        if (!_hasAttacked && _me.Abilities.Count > 0)
        {
            List<Ability> spellsICanUse = GetMyAbilities();

            int rand = Random.Range(0, spellsICanUse.Count);
            
            if(spellsICanUse.Count > 0)
            {
                _me.UseSkill(spellsICanUse[rand]);
                _hasAttacked = true;
            }
        }
    }

    void TryAbilites(Character target)
    {
        if (!_hasAttacked && _me.Abilities.Count > 0)
        {
            List<Ability> spellsICanUse = GetMyAbilities();

            int rand = Random.Range(0, spellsICanUse.Count);

            if (spellsICanUse.Count > 0)
            {
                _me.UseSkill(spellsICanUse[rand], target);
                _hasAttacked = true;
            }
        }
    }

    void TryHeal()
    {
        if(_me.Abilities.Count > 0 && !_hasAttacked && _me.CurrHealth < _me.MaxHealth * .25 && HasAbilityType(DamageType.HEALING))
        {
            List<Ability> spellsICanUse = GetMyAbilities(DamageType.HEALING);

            int rand = Random.Range(0, spellsICanUse.Count);

            if (spellsICanUse.Count > 0)
            {
                _me.UseHeal(spellsICanUse[rand]);
                _hasAttacked = true;
            }
        }
    }
    void TryHeal(Character target)
    {
        if (_me.Abilities.Count > 0 && !_hasAttacked && _me.CurrHealth < _me.MaxHealth * .25 && HasAbilityType(DamageType.HEALING))
        {
            List<Ability> spellsICanUse = GetMyAbilities(DamageType.HEALING);

            int rand = Random.Range(0, spellsICanUse.Count);

            if (spellsICanUse.Count > 0)
            {
                _me.UseHeal(spellsICanUse[rand], target);
                _hasAttacked = true;
            }
        }
    }

    bool HasAbilityType(DamageType skillType)
    {
        for (int i = 0; i < _me.Abilities.Count; i++)
        {
            if(_me.Abilities[i].Element == skillType)
            {
                return true;
            }
        }

        return false;
    }

    List<Ability> GetMyAbilities()
    {
        List<Ability> usables = new List<Ability>();

        for (int i = 0; i < _me.Abilities.Count; i++)
        {
            if (_me.Abilities[i].Element != DamageType.HEALING && GridHandler.CheckForEnemyWithinRange(_me.CurrentPosition, _me.Abilities[i], _me.Team))
            {
                if(_me.CurrMana > _me.Abilities[i].Cost)
                {
                    usables.Add(_me.Abilities[i]);
                }
            }
        }

        return usables;
    }


    List<Ability> GetMyAbilities(DamageType skillType)
    {
        List<Ability> usables = new List<Ability>();

        for (int i = 0; i < _me.Abilities.Count; i++)
        {
            if (_me.Abilities[i].Element == skillType && GridHandler.CheckForEnemyWithinRange(_me.CurrentPosition, _me.Abilities[i], _me.Team))
            {
                if (_me.CurrMana > _me.Abilities[i].Cost)
                {
                    usables.Add(_me.Abilities[i]);
                }
            }
        }

        return usables;
    }

    List<Ability> GetMyAbilities(Character target)
    {
        List<Ability> usables = new List<Ability>();

        for (int i = 0; i < _me.Abilities.Count; i++)
        {
            if (_me.Abilities[i].Element != DamageType.HEALING && GridHandler.CheckForEnemyWithinRange(_me.CurrentPosition, _me.Abilities[i], _me.Team, _currTarget))
            {
                if (_me.CurrMana > _me.Abilities[i].Cost)
                {
                    usables.Add(_me.Abilities[i]);
                }
            }
        }

        return usables;
    }

    List<Ability> GetMyAbilities(DamageType skillType, Character target)
    {
        List<Ability> usables = new List<Ability>();

        for (int i = 0; i < _me.Abilities.Count; i++)
        {
            if (_me.Abilities[i].Element == skillType && GridHandler.CheckForEnemyWithinRange(_me.CurrentPosition, _me.Abilities[i], _me.Team, _currTarget))
            {
                if (_me.CurrMana > _me.Abilities[i].Cost)
                {
                    usables.Add(_me.Abilities[i]);
                }
            }
        }

        return usables;
    }
}
