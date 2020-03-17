using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    CharacterStrategy _stratRef;
    
    string _myName;
    public string Name { get { return _myName; } }

    int _myHealth;
    public int Health { get { return _myHealth; } set { _myHealth = value; } }

    int _myStrength;
    public int Strength { get { return _myStrength; } }

    int _myAccuracy;
    public int Accuracy { get { return _myAccuracy; } }

    int _myCritChance;
    public int Crit { get { return _myCritChance; } }

    int _myMovement;
    public int Movement { get { return _myMovement; } }
    
    bool _dead;
    public bool IsDead { get { return _dead; } }

    string _myTileInfo;
    public string GetInfoIdentifier { get { return _myTileInfo; } }

    Vector2 _gridPos;
    public Vector2 CurrentPosition { get { return _gridPos; } set { _gridPos = value; } }

    List<Vector2> _myMoves;
    Character _lastToHitMe;
    public Character LastAttacker { get { return _lastToHitMe; } }
    public Character()
    {
        _myName = "Bert";
        _myHealth = 100;
        _myStrength = 10;
        _myAccuracy = 75;
        _myCritChance = 25;
        _myMovement = 2;
        _gridPos = Vector2.zero;
        _stratRef = new CharacterStrategy();
        _dead = false;
    }

    public Character(string name, string tileMoniker, Vector2 startPos)
    {
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = Random.Range(50, 201);
        _myStrength = Random.Range(5, 20);
        _myAccuracy = Random.Range(10, 101);
        _myCritChance = Random.Range(10, 101);
        _myMovement = Random.Range(1, 3);
        _gridPos = startPos;
        _stratRef = new CharacterStrategy();
        _dead = false;
    }

    public Character(string name, string tileMoniker, int hp, int att, int acc, int crit, int move, Vector2 startPos)
    {
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = hp;
        _myStrength = att;
        _myAccuracy = acc;
        _myCritChance = crit;
        _myMovement = move;
        _gridPos = startPos;
        _stratRef = new CharacterStrategy();
        _dead = false;
    }

    public void TakeAction()
    {
        if(!_dead)
        {
            _myMoves = GridHandler.WhereCanIMove(_gridPos, _myMovement);
            _stratRef.WhatDo(this);
            //Debug.Log("Action Taken");
        }
    }

    public void Move()
    {
        int randNum = Random.Range(0, _myMoves.Count);
        GridHandler.MoveEnemy(this, _myMoves[randNum]);
    }

    public void Move(Character target)
    {
        Vector2 movingTo = CurrentPosition;

        for (int i = 0; i < _myMoves.Count; i++)
        {
            if (target.CurrentPosition.x >= _gridPos.x && target.CurrentPosition.y >= _gridPos.y)
            {
                if (_myMoves[i].x >= movingTo.x && _myMoves[i].y >= movingTo.y)
                {
                    movingTo = _myMoves[i];
                }
            }
            else if (target.CurrentPosition.x >= _gridPos.x && target.CurrentPosition.y <= _gridPos.y)
            {
                if (_myMoves[i].x >= movingTo.x && _myMoves[i].y <= movingTo.y)
                {
                    movingTo = _myMoves[i];
                }
            }
            else if (target.CurrentPosition.x <= _gridPos.x && target.CurrentPosition.y >= _gridPos.y)
            {
                if (_myMoves[i].x <= movingTo.x && _myMoves[i].y >= movingTo.y)
                {
                    movingTo = _myMoves[i];
                }
            }
            else if (target.CurrentPosition.x <= _gridPos.x && target.CurrentPosition.y <= _gridPos.y)
            {
                if (_myMoves[i].x <= movingTo.x && _myMoves[i].y <= movingTo.y)
                {
                    movingTo = _myMoves[i];
                }
            }
            
        }

        GridHandler.MoveEnemy(this, movingTo);
    }

    public void Attack()
    {
        if (GridHandler.CheckForEnemies(_gridPos))
        {
            List<Character> _enemiesNearMe = GridHandler.GetEnemiesAroundMe(_gridPos);

            int randNum = Random.Range(0, _enemiesNearMe.Count);
            FightHandler.AttackEnemy(this, _enemiesNearMe[randNum]);
        }
    }

    public void Attack(Character currTarget)
    {
        if (GridHandler.CheckForEnemies(_gridPos, currTarget))
        {
            FightHandler.AttackEnemy(this, currTarget);
        }
    }

    public void RunAway()
    {
        for (int i = 0; i < _myMoves.Count; i++)
        {
            if (LastAttacker.CurrentPosition.x >= _gridPos.x)
            {
                _myMoves.Remove(_myMoves[i]);
            }
            else if (LastAttacker.CurrentPosition.x <= _gridPos.x)
            {
                _myMoves.Remove(_myMoves[i]);
            }
            else if (LastAttacker.CurrentPosition.y >= _gridPos.y)
            {
                _myMoves.Remove(_myMoves[i]);
            }
            else if (LastAttacker.CurrentPosition.y <= _gridPos.y)
            {
                _myMoves.Remove(_myMoves[i]);
            }
        }

        Move();
    }

    public void TakeDamage(Character attacker, int damage)
    {
        _lastToHitMe = attacker;
        _myHealth -= damage;
        if(_myHealth < 0)
        {
            HistoryHandler.DeclareDeath(this);
            _dead = true;
        }
    }

    
}
