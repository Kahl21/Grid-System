using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{

    CanvasGroup _group;
    RectTransform _myRect;

    float _currTime, _startTime;
    float _fadespeed = 1f;
    float _movespeed = 50f;

    Vector3 _objectWorldPos;

    public void Init(Vector3 objectworldpos)
    {
        _myRect = GetComponent<RectTransform>();
        _group = GetComponent<CanvasGroup>();
        _group.alpha = 1;

        _objectWorldPos = objectworldpos;
        _startTime = Time.time;
        GameUpdate.UISubscribe += MoveUp;
    }

    void MoveUp()
    {
        _currTime = (Time.time - _startTime) / _fadespeed; 
        
        _group.alpha = RandomThings.Interpolate(_currTime, 1, 0);

        Vector3 movepos = UIHolder.UIInstance.GetBattleUI.BattleCamera.WorldToScreenPoint(_objectWorldPos);


        movepos += (Vector3.up * _movespeed) * _currTime;

        _myRect.position = movepos;

        if (_currTime >= 1)
        {
            GameUpdate.UISubscribe -= MoveUp;
            Destroy(this.gameObject);
            Debug.Log("text destroyed");
        }
    }
}
