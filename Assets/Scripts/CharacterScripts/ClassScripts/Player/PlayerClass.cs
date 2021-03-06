﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : Character
{

    //base constructor
    public PlayerClass() : base()
    {
        _stratRef = new Strategy(this);
        Debug.Log("Strategy Made");
    }

    //randomized constructor
    public PlayerClass(string name, string tileMoniker, TeamType team, Vector2 startPos) : base(name, tileMoniker, team, startPos)
    {
        _stratRef = new Strategy(this);
        //Debug.Log("Strategy Made");
    }

    //incredibly specific constructor
    public PlayerClass(string name, string tileMoniker, TeamType team, int hp, int mp, int att, int mag, int acc, int crit, int baseDef, int PhysRes, int fireRes, int iceRes, int thunRes, int lightRes, int darkRes,  int speed,  int move, Weapons weapon, Vector2 startPos) : base(name, tileMoniker, team, hp, mp, att, mag, acc, crit, baseDef, PhysRes, fireRes, iceRes, thunRes, lightRes, darkRes, speed, move, weapon, startPos)
    {
        _stratRef = new Strategy(this);
        Debug.Log("Strategy Made");
    }

    //Main method called
    //starts this units turn
    public override void StartTurn()
    {
        if(!_dead)
        {
            _stratRef.WhatDo();
            _currMovement = _myMovement;
            //Debug.Log("Action Taken");
        }
    }

    //Moves unit randomly to any space it can travel
    public override void Move()
    {
        Debug.Log("player move called");
        GridHandler.ShowReleventGrid(_gridPos, _currMovement, Color.blue, Actions.MOVE);
    }

    public override bool CheckForMoreMove(int xpos, int ypos)
    {
        int moveDiff = GridHandler.GetDistanceMoved(xpos, ypos) - _currMovement;
        _currMovement += moveDiff;
        if(_currMovement >= 1)
        {
            Move();
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void StartAttack()
    {
        Debug.Log("player attack active");
        GridHandler.ShowReleventGrid(_gridPos, _myWeapon.Range, Color.red, Actions.ATTACK);
    }

    public override void Attack(Character target)
    {
        Debug.Log("player attcking");
        FightHandler.AttackEnemy(this, target);
        _stratRef.HasAttacked = true;
    }

    //checks for enemies nearby and attacks
    //indiscriminately
    /*public void Attack()
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeam))
        {
            List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, _myWeapon, _myTeam);
            if(_enemiesNearMe.Count > 0)
            {
                int randNum = Random.Range(0, _enemiesNearMe.Count);

                FightHandler.AttackEnemy(this, _enemiesNearMe[randNum]);
            }
            else
            {
                Debug.Log("no enemy");
            }
        }
    }*/

    //attacks target if it is in range
    //if calls base attack method
    /*public void Attack(Character currTarget)
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeam, currTarget))
        {
            FightHandler.AttackEnemy(this, currTarget);
        }
        else
        {
            Attack();
        }
    }*/

    //Use Skill passed to it
    //gets all enemies near and blindly choose one to hit
    //regardless of Spell Shape
    public override void UseSkill(Ability currSkill)
    { 


        List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill, _myTeam); 
        int rand = UnityEngine.Random.Range(0, _enemiesNearMe.Count);
        if(_enemiesNearMe.Count > 0)
        {
            _enemiesNearMe = GridHandler.GetTargetsInSplashZone(_enemiesNearMe[rand].CurrentPosition,  currSkill);

            currSkill.ActivateSkill(this, _enemiesNearMe);
            _stratRef.HasAttacked = true;
        }

    }

    //Use Skill passed to it
    //gets the enemy that is being targeted
    //checks the splash zone for collatoral
    public override void UseSkill(Ability currSkill, Character currTarget)
    {

        Debug.Log("using " + currSkill.Name);
        List<Character> enemiesNearMe = new List<Character>();
        enemiesNearMe.Add(currTarget);
        currSkill.ActivateSkill(this, enemiesNearMe);
        _stratRef.HasAttacked = true;

    }

    //Use heal Skill passed to it on myself
    public override void UseHeal(Ability currSkill)
    {
        List<Character> enemiesNearMe = new List<Character>();
        enemiesNearMe.Add(this);
        currSkill.ActivateSkill(this, enemiesNearMe);
        _stratRef.HasAttacked = true;
    }

    //Use Heal Skill passed to it and heals target
    public override void UseHeal(Ability currSkill, Character currTarget)
    {
        List<Character> enemiesNearMe = new List<Character>();
        enemiesNearMe.Add(currTarget);
        currSkill.ActivateSkill(this, enemiesNearMe);
        _stratRef.HasAttacked = true;
    }
}