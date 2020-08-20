using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{

    CanvasGroup _group;
    RectTransform _myRect;

    float _currTime, _startTime;
    float _fadespeed = 1f;
    float _movespeed = .5f;
    

    public void Init()
    {
        _myRect = GetComponent<RectTransform>();
        _group = GetComponent<CanvasGroup>();
        _group.alpha = 1;

        _startTime = Time.time;
        GameUpdate.Subscribe += MoveUp;
    }

    void MoveUp()
    {
        _currTime = (Time.time - _startTime) / _fadespeed; 
        
        _group.alpha = RandomThings.Interpolate(_currTime, 1, 0);

        _myRect.position += (Vector3.up * _movespeed);

        if (_group.alpha == 0)
        {
            GameUpdate.Subscribe -= MoveUp;
            Destroy(gameObject);
        }
    }
}
