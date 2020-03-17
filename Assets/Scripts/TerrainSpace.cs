using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpace
{
    protected int _terrainCost;

    public int GetTerrainCost { get { return _terrainCost; } }

    protected char _terrainType;
    public char GetTerrainType { get { return _terrainType; } }

    public TerrainSpace()
    {
        _terrainType = ' ';
        _terrainCost = 1;
    }

}
