using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CharInfoState
{
    NONE,
    FADEIN,
    FADEOUT,
}

public class CharacterOptions : MonoBehaviour
{
    CanvasGroup _mainPanel;
    AbilityOptions _abilityPanel;

    float _moveFadeSpeed = .3f;
    float _startTime;
    float _fadeCurrTime, _moveCurrTime;

    bool _hidden, _moving;

    RectTransform _myRect;
    Vector3 _startPos;
    float _moveThreshold;
    Vector3 _currPos;
    Vector3 _movePos;

    Character _currCharacter;
    BattleUI _uiRef;
  
    public void Init(BattleUI uiref)
    {
        _uiRef = uiref;
        _myRect = GetComponent<RectTransform>();
        _startPos = _myRect.localPosition;
        _moveThreshold = _startPos.x - 300;
        _currPos = _startPos;
        _hidden = true;
        _moving = false;
        //Debug.Log(_startPos);
        SetUI();
    }

    void SetUI()
    {
        _mainPanel = transform.GetChild(0).GetComponent<CanvasGroup>();
        _mainPanel.alpha = 0;
        _mainPanel.blocksRaycasts = false;

        _abilityPanel = transform.GetChild(1).GetComponent<AbilityOptions>();
        _abilityPanel.Init(this);
    }

    public void ShowUI(Character currentChar)
    {
        if(currentChar != _currCharacter)
        {
            _abilityPanel.SetSkills(currentChar);
            _currCharacter = currentChar; 
            
            if (_hidden)
            {
                ShowUI();
            }
        }
    }

    public void ShowUI()
    {
            CalculateMove();
            GameUpdate.Subscribe += FadeInUI;
    }

    public void HideUI()
    {
        _movePos = _startPos;
        StartMove();
        if (!_hidden || !_abilityPanel.IsHidden)
        {
            _hidden = true;
            GameUpdate.Subscribe += FadeOutUI;
        }
    }

    public void BackToMenu()
    {
        GridHandler.StopSelection();
        ShowUI();
    }

    public void SelectMove()
    {
        ChangeBattleInteraction(UIInteractions.MOVESELECT);
        HideUI();
        _currCharacter.StartMove();
    }

    public void SelectAttack()
    {
        ChangeBattleInteraction(UIInteractions.ATTACKSELECT);
    }

    public void SelectAbility()
    {
        _abilityPanel.BringUpAbilities();
        CalculateMove();
        GameUpdate.Subscribe += FadeOutUI;
    }

    public void CalculateMove()
    {
        if (_currPos.x > _moveThreshold)
        {
            _movePos = _currPos + (Vector3.left * 150f);
        }
        else
        {
            _movePos = _currPos + (Vector3.right * 150f);
        }

        StartMove();
    }

    public void StartMove()
    {
        if (!_moving)
        {
            _moving = true;
            _startTime = Time.time;
            GameUpdate.Subscribe += MoveUI;
        }
    }

    public void FadeInUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime > 1)
        {
            _mainPanel.alpha = 1;
            _mainPanel.blocksRaycasts = true;
            _hidden = false;
            GameUpdate.Subscribe -= FadeInUI;
        }

        _mainPanel.alpha = RandomThings.Interpolate(_fadeCurrTime, 0, 1);
    }

    public void FadeOutUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime > 1)
        {
            _mainPanel.alpha = 0;
            _mainPanel.blocksRaycasts = false;
            _hidden = true;
            GameUpdate.Subscribe -= FadeOutUI;
        }

        _mainPanel.alpha = RandomThings.Interpolate(_fadeCurrTime, 1, 0);
    }

    public void MoveUI()
    {
        _moveCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_moveCurrTime > 1)
        {
            _myRect.localPosition = _movePos;
            _currPos = _myRect.localPosition;
            _moving = false;
            GameUpdate.Subscribe -= MoveUI;
        }

        _myRect.localPosition = RandomThings.Interpolate(_moveCurrTime, _currPos, _movePos);
    }

    public void ChangeBattleInteraction(UIInteractions state)
    {
        _uiRef.GetInsteractionState = state;
    }

    public void ResetUIMovements()
    {
        Debug.Log("real ui hide called");
        _currCharacter = null;
        _abilityPanel.ResetFading();
        HideUI();
    }
}
