using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Character
{
    public Warrior() : base()
    {
        _myName = "Landorin";

        _myHealth = 200;
        _myMaxHealth = _myHealth;

        _myMana = 100;
        _myMaxMana = _myMana;

        _myOffense = new AttackStats(20, 0, 75, 20);
        _myDefenses = new DefenseStats(5, 10, -5, -5, -5, -5, -5);

        _myWeapon = new Axe();

        _myAbilites.Add(new DoubleSlash());

        _mySpeed = Random.Range(5, 50);
        _myMovement = 2;
        _currMovement = _myMovement;
        _gridPos = Vector2.zero;
        _dead = false;
    }

    public Warrior(string name, string tileMoniker, TeamType team, Vector2 startPos) : base(name, tileMoniker, team, startPos)
    {
        _myTeam = team;
        _myName = name + tileMoniker;
        _myTileInfo = tileMoniker;
        _myHealth = Random.Range(50, 201);
        _myMaxHealth = _myHealth;
        _myMana = 100;
        _myMaxMana = 100;

        int _myStrength = Random.Range(5, 20);
        int _myMagic = Random.Range(5, 20);
        int _myAccuracy = Random.Range(10, 101);
        int _myCritChance = Random.Range(10, 101);
        _myOffense = new AttackStats(_myStrength, _myMagic, _myAccuracy, _myCritChance);

        int _baseDef = Random.Range(10, 51);
        int _physRes = Random.Range(-100, 101);
        int _fireRes = Random.Range(-100, 101);
        int _iceRes = Random.Range(-100, 101);
        int _thunderRes = Random.Range(-100, 101);
        int _lightRes = Random.Range(-100, 101);
        int _darkRes = Random.Range(-100, 101);
        _myDefenses = new DefenseStats(_baseDef, _physRes, _fireRes, _iceRes, _thunderRes, _lightRes, _darkRes);

        Weapons newWeapon = (Weapons)Random.Range(0, (int)Weapons.GAUNTLET + 1);
        switch (newWeapon)
        {
            case Weapons.SWORD:
                _myWeapon = new Sword();
                break;
            case Weapons.SPEAR:
                _myWeapon = new Spear();
                break;
            case Weapons.AXE:
                _myWeapon = new Axe();
                break;
            case Weapons.BOW:
                _myWeapon = new Bow();
                break;
            case Weapons.STAFF:
                _myWeapon = new Staff();
                break;
            case Weapons.GAUNTLET:
                _myWeapon = new Gauntlets();
                break;
            default:
                break;
        }

        _mySpeed = Random.Range(1, 101);
        _myMovement = Random.Range(1, 3);
        _currMovement = _myMovement;
        _gridPos = startPos;
        _dead = false;
    }
    
}
