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

    public void Init(Color baseColor)
    {
        _myRend = GetComponent<MeshRenderer>();
        _myMat = _myRend.materials[0];
        _myBaseColor = _myMat.color;
        _myBaseColor = baseColor;
        _myCurrentColor = baseColor;

        _myMat.color = _myBaseColor;
        _myRend.materials[0] = _myMat;
    }

    public void SetColor(Color newtileColor)
    {
        _myCurrentColor = newtileColor;

        _myMat.color = _myCurrentColor;
        _myRend.materials[0] = _myMat;
    }

    public void ResetColor()
    {
        _myCurrentColor = _myBaseColor;

        _myMat.color = _myCurrentColor;
        _myRend.materials[0] = _myMat;
    }
}
