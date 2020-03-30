using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    CharacterStrategy _stratRef;
    
    string _myName;
    public string Name { get { return _myName; } }

    int _myHealth;
    int _myMaxHealth;
    public int CurrHealth { get { return _myHealth; } set { _myHealth = value; } }
    public int MaxHealth { get { return _myMaxHealth; } }
    int _myStrength;
    public int Strength { get { return _myStrength; } }

    int _myAccuracy;
    public int Accuracy { get { return _myAccuracy; } }

    int _myCritChance;
    public int Crit { get { return _myCritChance; } }

    int _mySpeed;
    public int Speed { get { return _mySpeed; } }

    int _myMovement;
    public int Movement { get { return _myMovement; } }
    
    bool _dead;
    public bool IsDead { get { return _dead; } }

    string _myTileInfo;
    public string GetInfoIdentifier { get { return _myTileInfo; } }

    Vector2 _gridPos;
    public Vector2 CurrentPosition { get { return _gridPos; } set { _gridPos = value; } }

    List<Vector2> _myMoves;
    public List<Vector2> SpacesICanMove { get { return _myMoves; } }

    Character _lastToHitMe;
    public Character LastAttacker { get { return _lastToHitMe; } set { _lastToHitMe = value; } }

    public Character()
    {
        _myName = "Bert";
        _myHealth = 100;
        _myMaxHealth = 100;
        _myStrength = 10;
        _myAccuracy = 75;
        _myCritChance = 25;
        _mySpeed = 10;
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
        _myMaxHealth = _myHealth;
        _myStrength = Random.Range(5, 20);
        _myAccuracy = Random.Range(10, 101);
        _myCritChance = Random.Range(10, 101);
        _mySpeed = Random.Range(1, 101);
        _myMovement = Random.Range(1, 3);
        _gridPos = startPos;
        _stratRef = new CharacterStrategy();
        _dead = false;
    }

    public Character(string name, string tileMoniker, int hp, int att, int acc, int speed, int crit, int move, Vector2 startPos)
    {
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = hp;
        _myMaxHealth = hp;
        _myStrength = att;
        _myAccuracy = acc;
        _myCritChance = crit;
        _mySpeed = speed;
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
        LastAttacker = target;
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
        else
        {
            Attack();
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

    public void TakeDamage(int damage)
    {
        _myHealth -= damage;
        if(_myHealth < 0)
        {
            HistoryHandler.DeclareDeath(this);
            _dead = true;
        }
    }
}
