using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStrategy : Strategy
{
    protected enum EnemyActions
    {
        NONE,
        MOVE,
        ATTACK,
        ABILITY,
        TARGETABILITY,
        HEAL,
        TARGETHEAL
    }

    protected Character _currTarget;
    protected BattleStrats _myStrat;
    protected List<EnemyActions> _orderedActions;

    protected int _currentAction;

    protected float _startTime;
    protected float _waitTime = .2f;
    protected float _currTime;

    public EnemyStrategy() : base()
    {
        _currTarget = null;
        _orderedActions = new List<EnemyActions>();


        switch (_myStrat)
        {
            case BattleStrats.BLOODTHIRSTY:
                _orderedActions.Add(EnemyActions.MOVE);
                _orderedActions.Add(EnemyActions.ATTACK);
                _orderedActions.Add(EnemyActions.TARGETABILITY);
                _orderedActions.Add(EnemyActions.ABILITY);
                break;
            case BattleStrats.CAUTIOUS:
                _orderedActions.Add(EnemyActions.HEAL);
                _orderedActions.Add(EnemyActions.ABILITY);
                _orderedActions.Add(EnemyActions.TARGETABILITY);
                _orderedActions.Add(EnemyActions.MOVE);
                _orderedActions.Add(EnemyActions.ATTACK);
                break;
            case BattleStrats.RANGED:
                _orderedActions.Add(EnemyActions.TARGETABILITY); 
                _orderedActions.Add(EnemyActions.ABILITY); 
                _orderedActions.Add(EnemyActions.MOVE); 
                _orderedActions.Add(EnemyActions.HEAL); 
                _orderedActions.Add(EnemyActions.ATTACK);
                break;
            default:
                break;
        }
    }

    public EnemyStrategy(Character characterToControl) : base(characterToControl)
    {
        _currTarget = null;
        _orderedActions = new List<EnemyActions>();


        switch (_myStrat)
        {
            case BattleStrats.BLOODTHIRSTY:
                _orderedActions.Add(EnemyActions.MOVE);
                _orderedActions.Add(EnemyActions.ATTACK);
                _orderedActions.Add(EnemyActions.TARGETABILITY);
                _orderedActions.Add(EnemyActions.ABILITY);
                break;
            case BattleStrats.CAUTIOUS:
                _orderedActions.Add(EnemyActions.HEAL);
                _orderedActions.Add(EnemyActions.ABILITY);
                _orderedActions.Add(EnemyActions.TARGETABILITY);
                _orderedActions.Add(EnemyActions.MOVE);
                _orderedActions.Add(EnemyActions.ATTACK);
                break;
            case BattleStrats.RANGED:
                _orderedActions.Add(EnemyActions.TARGETABILITY);
                _orderedActions.Add(EnemyActions.ABILITY);
                _orderedActions.Add(EnemyActions.MOVE);
                _orderedActions.Add(EnemyActions.HEAL);
                _orderedActions.Add(EnemyActions.ATTACK);
                break;
            default:
                break;
        }

        //Debug.Log(_OrderOfActions.Count);
    }

    public override void WhatDo()
    {

        Debug.Log("What Do Called");
        if(_currTarget == null || _currTarget.IsDead)
        {
            _currTarget = FightHandler.FindRandomEnemy(_me, _me.Team);
        }

        //Debug.Log("what do");

        _hasAttacked = false;
        _hasMoved = false;
        _currentAction = 0;

        ContinueTurn();
    }

    public override void ContinueTurn()
    {
        Debug.Log("continue Turn");
        if (_currentAction < _orderedActions.Count)
        {
            Debug.Log("acting");
            _currentAction++;
            switch (_orderedActions[_currentAction-1])
            {
                case EnemyActions.NONE:
                    break;
                case EnemyActions.MOVE:
                    TryMove();
                    break;
                case EnemyActions.ATTACK:
                    TryAttack();
                    break;
                case EnemyActions.ABILITY:
                    TryAbilites();
                    break;
                case EnemyActions.TARGETABILITY:
                    TryAbilites(_currTarget);
                    break;
                case EnemyActions.HEAL:
                    TryHeal();
                    break;
                case EnemyActions.TARGETHEAL:
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.Log("Forcing End");
            UIHolder.UIInstance.GetBattleUI.EndCurrentTurn();
        }
    }

    //AI sees if it can move 
    //does so if able
    void TryMove()
    {
        Debug.Log("Moving");
        if (!_hasMoved)
        {
            _hasMoved = true;
            _me.Move(_currTarget);
        }
    }

    //AI tries to attack any enemy near them
    void TryAttack()
    {
        //Debug.Log("trying to attack");
        if (!_hasAttacked)
        {
            if (GridHandler.CheckForEnemyWithinRange(_me.CurrentPosition, _me.HeldWeapon, _me.Team, _currTarget))
            {
                Debug.Log("trying to attack target");
                _hasAttacked = true;
                _me.Attack(_currTarget);
            }
            else
            {
                Debug.Log("for some reason i did not attack");
                StartWaiting();

            }
        }
        else
        {
            Debug.Log("I've already attacked");
            StartWaiting();
        }
    }

    //AI tries to use any ability they have
    //on any enemy nearby
    void TryAbilites()
    {
        Debug.Log("trying to use ability generic");
        if (!_hasAttacked && _me.Abilities.Count > 0)
        {
            List<Ability> spellsICanUse = GetMyAbilities();

            int rand = Random.Range(0, spellsICanUse.Count);

            if (spellsICanUse.Count > 0 && spellsICanUse[rand].Cost <= _me.CurrMana)
            {
                Debug.Log(spellsICanUse[rand].Name + " is trying to be used");
                _hasAttacked = true;
                _me.UseSkill(spellsICanUse[rand]);
            }
            else
            {
                Debug.Log("no magic");
                StartWaiting();
            }
        }
        else
        {
            Debug.Log("I've already attacked");
            StartWaiting();
        }
    }

    //AI tries to use any ability they have
    //Only if their target is nearby
    void TryAbilites(Character target)
    {
        Debug.Log("trying to use ability with target");
        if (!_hasAttacked && _me.Abilities.Count > 0)
        {
            List<Ability> spellsICanUse = GetMyAbilities();

            int rand = Random.Range(0, spellsICanUse.Count);

            if (spellsICanUse.Count > 0 && spellsICanUse[rand].Cost <= _me.CurrMana)
            {
                Debug.Log(spellsICanUse[rand].Name + " is trying to be used");
                _hasAttacked = true;
                _me.UseSkill(spellsICanUse[rand], target);
            }
            else
            {
                Debug.Log("no magic");
                StartWaiting();
            }
        }
        else
        {
            Debug.Log("I've already attacked");
            StartWaiting();
        }
    }

    //AI tries to heal a nearby teammate
    void TryHeal() 
    { 
        Debug.Log("Trying to heal");
    
        if (_me.Abilities.Count > 0 && !_hasAttacked && _me.CurrHealth < _me.MaxHealth * .25 && HasAbilityType(DamageType.HEALING))
        {
            List<Ability> spellsICanUse = GetMyAbilities(DamageType.HEALING);

            int rand = Random.Range(0, spellsICanUse.Count);

            if (spellsICanUse.Count > 0 && spellsICanUse[rand].Cost <= _me.CurrMana)
            {
                _hasAttacked = true;
                _me.UseHeal(spellsICanUse[rand]);
            }
            else
            {
                Debug.Log("no magic");
                StartWaiting();
            }
        }
        else
        {
            Debug.Log("I've already attacked");
            StartWaiting();
        }
    }

    //AI tries to heal a specific nearby teammate
    void TryHeal(Character target)
    {
        Debug.Log("Trying to heal (target)");
        if (_me.Abilities.Count > 0 && !_hasAttacked && _me.CurrHealth < _me.MaxHealth * .25 && HasAbilityType(DamageType.HEALING))
        {
            List<Ability> spellsICanUse = GetMyAbilities(DamageType.HEALING);

            int rand = Random.Range(0, spellsICanUse.Count);

            if (spellsICanUse.Count > 0 && spellsICanUse[rand].Cost <= _me.CurrMana)
            {
                _hasAttacked = true;
                _me.UseHeal(spellsICanUse[rand], target);
            }
            else
            {
                Debug.Log("no magic");
                StartWaiting();
            }
        }
        else
        {
            Debug.Log("I've already attacked");
            StartWaiting();
        }
    }

    //called GetMyAbilites() and overloads
    //returns true if the current abilites is the same type that its looking for
    bool HasAbilityType(DamageType skillType)
    {
        for (int i = 0; i < _me.Abilities.Count; i++)
        {
            if (_me.Abilities[i].Element == skillType)
            {
                return true;
            }
        }

        return false;
    }

    //Gets all abilities that DO NOT HEAL
    //returns list with all applicable abilities
    List<Ability> GetMyAbilities()
    {
        List<Ability> usables = new List<Ability>();

        for (int i = 0; i < _me.Abilities.Count; i++)
        {
            if (_me.Abilities[i].Element != DamageType.HEALING && GridHandler.CheckForEnemyWithinRange(_me.CurrentPosition, _me.Abilities[i], _me.Team))
            {
                if (_me.CurrMana > _me.Abilities[i].Cost)
                {
                    usables.Add(_me.Abilities[i]);
                }
            }
        }

        return usables;
    }

    //overload for GetMyAbilities
    //pass any specific ability element
    //returns all skills with specified ability element
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

    //overload for GetMyAbilities
    //pass a target
    //returns all skills that are within range of hitting target
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

    //overload for GetMyAbilities
    //pass any specific ability element and target
    //returns all skills with specified ability element and are withing range of target
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

    public override void StartWaiting()
    {
        Debug.Log("waiting called");
        _startTime = Time.time;

        GameUpdate.UISubscribe += WaitingTime;
    }

    public override void WaitingTime()
    {
        _currTime = (Time.time - _startTime) / _waitTime;
        Debug.Log("waiting");
        if (_currTime > 1)
        {
            Debug.Log("Waiting Done");
            _currTime = 1;

            GameUpdate.UISubscribe -= WaitingTime;
            ContinueTurn();
        }
    }
}
