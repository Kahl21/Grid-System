
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : Character
{

    //base constructor
    public EnemyClass() : base()
    {
        _stratRef = new EnemyStrategy(this);
        _myClassName = "EnemyBase";
    }

    //randomized constructor
    public EnemyClass(string name, string tileMoniker, TeamType team, Vector2 startPos) : base(name, tileMoniker, team, startPos)
    {
        _stratRef = new EnemyStrategy(this);
        _myClassName = "EnemyNormal";
    }

    //incredibly specific constructor
    public EnemyClass(string name, string tileMoniker, TeamType team, int hp, int mp, int att, int mag, int acc, int crit, int baseDef, int PhysRes, int fireRes, int iceRes, int thunRes, int lightRes, int darkRes,  int speed,  int move, Weapons weapon, Vector2 startPos) : base(name,  tileMoniker,  team,  hp,  mp,  att,  mag,  acc,  crit,  baseDef,  PhysRes,  fireRes,  iceRes,  thunRes,  lightRes,  darkRes,  speed,  move, weapon, startPos)
    {
        _stratRef = new EnemyStrategy(this);
        _myClassName = "EnemyComplex";
    }

    //Main method called
    //starts this units turn
    public override void StartTurn()
    {
        if(!_dead)
        {
            //_stratRef.WhatDo();
            //Debug.Log("Enemy start");
            _currMovement = _myMovement;
            _stratRef.WhatDo();
            //Debug.Log("Action Taken");
        }
    }

    //Moves unit randomly to any space it can travel
    public override void Move()
    {
        _myMoves = GridHandler.WhereCanIMove(_gridPos, _myMovement);
        int randNum = UnityEngine.Random.Range(0, _currMovement);
        GridHandler.MoveEnemy(this, _myMoves[randNum]);
    }

    //Moves towards the targeted Unit
    //Uses Dijstrka to find its path (not implemented)
    //follows path for as long as it has movement
    public override void Move(Character target)
    {
        LastAttacker = target;
        if (!GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeam, _lastToHitMe))
        {
            GridHandler.DijkstraMove(this, GridHandler.RetrieveTile(target.CurrentPosition), _currMovement);
            return;
        }
        else
        {
            _stratRef.StartWaiting();
        }

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

    public override void Attack(Character target)
    {
        if (GridHandler.CheckForEnemyWithinRange(_gridPos, _myWeapon, _myTeam))
        {
            List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, _myWeapon, _myTeam);
            if (_enemiesNearMe.Count > 0)
            {
                //Debug.Log("attacking random");
                int randNum = UnityEngine.Random.Range(0, _enemiesNearMe.Count);

                FightHandler.AttackEnemy(this, _enemiesNearMe[randNum]);
                return;
            }
            else
            {
                //Debug.Log("no enemy");
                _stratRef.StartWaiting();
            }
        }
        else
        {
            _stratRef.StartWaiting();
        }

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
        }

    }

    //Use Skill passed to it
    //gets the enemy that is being targeted
    //checks the splash zone for collatoral
    public override void UseSkill(Ability currSkill, Character currTarget)
    {
        List<Character> _enemiesNearMe = GridHandler.GetEnemiesinRange(_gridPos, currSkill, _myTeam, currTarget);

        //Debug.Log("using " + currSkill.Name);
        if(_enemiesNearMe.Count > 0)
        {
            _enemiesNearMe = GridHandler.GetTargetsInSplashZone(_enemiesNearMe[0].CurrentPosition, currSkill);

            currSkill.ActivateSkill(this, _enemiesNearMe);
        }
    }

    //Use heal Skill passed to it
    //gets al enemies near and blindly choose one to hit
    public override void UseHeal(Ability currSkill)
    {
        List<Character> _enemiesNearMe = new List<Character>();
        _enemiesNearMe.Add(this);
        currSkill.ActivateSkill(this, _enemiesNearMe);
    }

    //Use Heal Skill passed to it
    //gets all Allies nearby and chooses one with the least health
    public override void UseHeal(Ability currSkill, Character currTarget)
    {
        //List<Character> _enemiesNearMe = GridHandler.GetAlliesinRange(_gridPos, currSkill);
        //currSkill.ActivateSkill(this, _enemiesNearMe, currTarget);
    }
}
