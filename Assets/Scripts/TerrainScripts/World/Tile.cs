using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    GridToken _whosOnTopOfMe;
    public GridToken PersonOnMe { get { return _whosOnTopOfMe; } set { _whosOnTopOfMe = value; } }

    MeshRenderer _myRend;
    Material _myMat;
    Color _myBaseColor;
    Color _myCurrentColor;
    bool _targetable;
    public bool IsTargetable { get { return _targetable; } }

    int _xPos, _yPos;
    public int GetXPosition { get { return _xPos; } }
    public int GetYPosition { get { return _yPos; } }

    //initialize
    public void Init(Color baseColor, int xstartpos, int ystartpos)
    {
        _xPos = xstartpos;
        _yPos = ystartpos;

        _myRend = GetComponent<MeshRenderer>();
        _myMat = _myRend.materials[0];
        _myBaseColor = _myMat.color;
        _myBaseColor = baseColor;
        _myCurrentColor = baseColor;

        _myMat.color = _myBaseColor;
        _myRend.materials[0] = _myMat;
        _targetable = false;
    }

    //set color of the tile
    public void SetColor(Color newtileColor)
    {
        _myCurrentColor = newtileColor;

        _myMat.color = _myCurrentColor;
        _myRend.materials[0] = _myMat;
        _targetable = true;
    }

    //reset tile color to base color
    public void ResetColor()
    {
        _myCurrentColor = _myBaseColor;

        _myMat.color = _myCurrentColor;
        _myRend.materials[0] = _myMat;
        _targetable = false;
    }
}
