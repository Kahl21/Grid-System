using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class GridToken : MonoBehaviour
{
    Tile _myTile;
    public Tile GetTile { get { return _myTile; } set { _myTile = value; } }

    Character _myRepresentative;
    public Character GetCharacter { get { return _myRepresentative; } set { _myRepresentative = value; } }

}
