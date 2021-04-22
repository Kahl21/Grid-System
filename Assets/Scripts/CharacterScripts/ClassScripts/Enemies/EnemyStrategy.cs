using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStrategy : Strategy
{

    protected Character _currTarget;
    protected BattleStrats _myStrat;

    protected List<UnityAction> _OrderOfActions;
    protected int _currentAction;

    protected float _startTime;
    protected float _waitTime = 1f;
    protected float _currTime;

    public EnemyStrategy() : base()
    {
        _currTarget = null;
        _OrderOfActions = new List<UnityAction>();

        switch (_myStrat)
        {
            case BattleStrats.BLOODTHIRSTY:
                _OrderOfActions.Add(() => TryMove()); 
                _OrderOfActions.Add(() => TryAttack()); 
                _OrderOfActions.Add(() => TryAbilites(_currTarget));
                _OrderOfActions.Add(() => TryAbilites());
                break;
            case BattleStrats.CAUTIOUS:
                _OrderOfActions.Add(() => TryHeal());
                _OrderOfActions.Add(() => TryAbilites());
                _OrderOfActions.Add(() => TryAbilites(_currTarget));
                _OrderOfActions.Add(() => TryMove());
                _OrderOfActions.Add(() => TryAttack());
                break;
            case BattleStrats.RANGED:
                _OrderOfActions.Add(() => TryAbilites(_currTarget));
                _OrderOfActions.Add(() => TryAbilites());
                _OrderOfActions.Add(() => TryMove());
                _OrderOfActions.Add(() => TryHeal());
                _OrderOfActions.Add(() => TryAttack());
                break;
            default:
                break;
        }
    }

    public EnemyStrategy(Character characterToControl) : base(characterToControl)
    {
        _currTarget = null; 
        _OrderOfActions = new List<UnityAction>();

        switch (_myStrat)
        {
            case BattleStrats.BLOODTHIRSTY:
                _OrderOfActions.Add(() => TryMove());
                _OrderOfActions.Add(() => TryAttack());
                _OrderOfActions.Add(() => TryAbilites(_currTarget));
                _OrderOfActions.Add(() => TryAbilites());
                break;
            case BattleStrats.CAUTIOUS:
                _OrderOfActions.Add(() => TryHeal());
                _OrderOfActions.Add(() => TryAbilites());
                _OrderOfActions.Add(() => TryAbilites(_currTarget));
                _OrderOfActions.Add(() => TryMove());
                _OrderOfActions.Add(() => TryAttack());
                break;
            case BattleStrats.RANGED:
                _OrderOfActions.Add(() => TryAbilites(_currTarget));
                _OrderOfActions.Add(() => TryAbilites());
                _OrderOfActions.Add(() => TryMove());
                _OrderOfActions.Add(() => TryHeal());
                _OrderOfActions.Add(() => TryAttack());
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
        if (_currentAction < _OrderOfActions.Count)
        {
            Debug.Log("acting");
            _currentAction++;
            _OrderOfActions[_currentAction-1]();
        }
        else
        {
            Debug.Log("Forcing End");
            FightHandler.ForceEndTurn();
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
                _me.Attack(_currTarget);
                _hasAttacked = true;
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
                _me.UseSkill(spellsICanUse[rand]);
                _hasAttacked = true;
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
                _me.UseSkill(spellsICanUse[rand], target);
                _hasAttacked = true;
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
                _me.UseHeal(spellsICanUse[rand]);
                _hasAttacked = true;
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
                _me.UseHeal(spellsICanUse[rand], target);
                _hasAttacked = true;
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

        GameUpdate.Subscribe += WaitingTime;
    }

    public override void WaitingTime()
    {
        _currTime = (Time.time - _startTime) / _waitTime;
        Debug.Log("waiting");
        if (_currTime > 1)
        {
            Debug.Log("Waiting Done");
            _currTime = 1;

            GameUpdate.Subscribe -= WaitingTime;
            ContinueTurn();
        }
    }
}
