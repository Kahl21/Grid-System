using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpace
{
    protected int _terrainCost;

    public int GetTerrainCost { get { return _terrainCost; } }

    protected float _terrainHeight;
    public float GetTerrainHeight { get { return _terrainHeight; } }

    protected char _terrainType;
    public char GetTerrainType { get { return _terrainType; } }

    protected Color _terrainColor;
    public Color GetTerrainColor { get { return _terrainColor; } }


    public TerrainSpace()
    {
        _terrainType = ' ';
        _terrainCost = 1;
        _terrainHeight = 0f;
        _terrainColor = Color.white;
    }

    public TerrainSpace(float height)
    {
        _terrainType = ' ';
        _terrainCost = 1;
        _terrainHeight = height;
        _terrainColor = Color.white;
    }

}

public class ForestSpace : TerrainSpace
{

    public ForestSpace() : base()
    {
        _terrainType = 'F';
        _terrainCost = 2;
        _terrainHeight = 0f;
        _terrainColor = Color.green;
    }
    public ForestSpace(float height) : base(height)
    {
        _terrainType = 'F';
        _terrainCost = 2;
        _terrainHeight = height;
        _terrainColor = Color.green;
    }
}

public class WaterSpace : TerrainSpace
{
    public WaterSpace() : base()
    {
        _terrainType = 'W';
        _terrainCost = 3;
        _terrainHeight = 0f;
        _terrainColor = Color.blue;
    }
    public WaterSpace(float height) : base(height)
    {
        _terrainType = 'W';
        _terrainCost = 3;
        _terrainHeight = height;
        _terrainColor = Color.blue;
    }
}

public class HoleSpace : TerrainSpace
{
    public HoleSpace() : base()
    {
        _terrainType = 'O';
        _terrainCost = 100;
        _terrainHeight = 0f;
        _terrainColor = Color.black;
    }
    public HoleSpace(float height) : base(height)
    {
        _terrainType = 'O';
        _terrainCost = 100;
        _terrainHeight = height;
        _terrainColor = Color.black;
    }
}
