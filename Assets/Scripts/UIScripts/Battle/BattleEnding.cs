using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnding : MonoBehaviour, IFadeable
{

    CanvasGroup _endImage;
    Text _endText;

    [SerializeField]
    float _moveFadeSpeed = 100f;
    float _startTime;
    float _fadeCurrTime;

    public void Init()
    {
        _endImage = GetComponent<CanvasGroup>();
        _endImage.alpha = 0;
        SetUI();
    }

    public void SetUI()
    {
        GameObject temp = _endImage.transform.GetChild(0).gameObject;
        _endText = temp.transform.GetChild(0).GetComponent<Text>();
    }

    public void StartEnding()
    {
        Debug.Log("end starting");
        if(FightHandler.GetAllCharacters[0].Team == TeamType.PLAYER)
        {
            _endText.text = "V I C T O R Y !";
        }
        else
        {
            _endText.text = "D E F E A T ...";
        }

        _startTime = Time.time;
        GameUpdate.UISubscribe += FadeInUI;
    }

    public void FadeInUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime >= 1)
        {
            GameUpdate.UISubscribe -= FadeInUI;
            _startTime = Time.time;
            GameUpdate.UISubscribe += FadeOutUI;
        }

    }

    public void FadeOutUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / (_moveFadeSpeed*2);
        if (_fadeCurrTime >= 1)
        {
            GameUpdate.UISubscribe -= FadeOutUI;
            Debug.Log("ending done");
            UIHolder.UIInstance.ResetToTeamSelect();
        }
        _endImage.alpha = RandomThings.Interpolate(_fadeCurrTime, 1, 0);
    }
}