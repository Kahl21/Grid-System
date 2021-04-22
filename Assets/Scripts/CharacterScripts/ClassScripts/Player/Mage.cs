using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : PlayerClass
{
    public Mage() : base()
    {
        _myName = "Xavier";

        _myHealth = 100;
        _myMaxHealth = _myHealth;

        _myMana = 200;
        _myMaxMana = _myMana;

        _myOffense = new AttackStats(5, 40, 60, 10);
        _myDefenses = new DefenseStats(5, 0, 20, 20, 20, 20, 20);

        _myWeapon = new Staff();

        _mySpeed = Random.Range(0, 31);
        _myMovement = 2;
        _gridPos = Vector2.zero;
        _dead = false;
    }

}
