using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IFadeable
{
    void FadeInUI();

    void FadeOutUI();
}

public interface IMoveable
{
    void CalculateMove();

    void StartMove();

    void MoveUI();
}
