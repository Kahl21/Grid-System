using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStrategy 
{
    private enum Strats
    {
        TARGET_LOCKED,
        REVENGER,
        //FROM_AFAR,
    }

    Strats myStrat;
    Character currTarget;

    public CharacterStrategy()
    {
        int stratNum = Random.Range(0, (int)Strats.REVENGER + 1);
        myStrat = (Strats)stratNum;

        currTarget = null;
    }

    public void WhatDo(Character me)
    {
        //Debug.Log("what do");
        switch (myStrat)
        {
            case Strats.TARGET_LOCKED:
                if (currTarget == null || currTarget.IsDead)
                {
                    currTarget = FightHandler.FindRandomEnemy(me);
                }

                me.Move(currTarget);
                me.Attack(currTarget);
                break;
            case Strats.REVENGER:
                if(me.LastAttacker != null)
                {
                    me.Move(me.LastAttacker);
                    me.Attack(me.LastAttacker);
                }
                else
                {
                    me.Move();
                    me.Attack();
                }
                break;
            default:
                break;
        }
    }
}
