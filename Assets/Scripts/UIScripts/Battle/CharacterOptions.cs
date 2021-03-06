﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum CharInfoState
{
    NONE,
    FADEIN,
    FADEOUT,
}

public class CharacterOptions : MonoBehaviour, IFadeable, IMoveable
{
    CanvasGroup _mainPanel;
    Button _moveButt, _attButt, _abilButt, _endButt;
    AbilityOptions _abilityPanel;
    public AbilityOptions GetAbilityPanel { get { return _abilityPanel; } }

    [SerializeField]
    float _moveFadeSpeed = .3f;
    float _startTime;
    float _fadeCurrTime, _moveCurrTime;

    bool _hidden, _moving;
    public bool IsMoving { get { return _moving; } }
    public bool IsHidden { get { return _hidden; } }

    RectTransform _myRect;
    Vector3 _startPos;
    float _moveThreshold;
    Vector3 _currPos;
    Vector3 _movePos;

    Character _currCharacter;
    BattleUI _batRef;
  
    public void Init()
    {
        _batRef = UIHolder.UIInstance.GetBattleUI;
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

        _moveButt = _mainPanel.transform.GetChild(0).GetComponent<Button>();
        _attButt = _mainPanel.transform.GetChild(1).GetComponent<Button>();
        _abilButt = _mainPanel.transform.GetChild(2).GetComponent<Button>();
        _endButt = _mainPanel.transform.GetChild(3).GetComponent<Button>();

        _abilityPanel = transform.GetChild(1).GetComponent<AbilityOptions>();
        _abilityPanel.Init(_batRef, this);
    }

    public void ShowUI(Character currentChar)
    {
        _abilityPanel.SetSkills(currentChar);
        _currCharacter = currentChar;

        if (_hidden)
        {
            ShowUI();
        }
    }

    public void ShowUI()
    {
        CheckForDisabledButtons();
        CalculateMove();
        GameUpdate.UISubscribe += FadeInUI;
    }

    void CheckForDisabledButtons()
    {
        if(_currCharacter.CurrentMovement > 0)
        {
            _moveButt.interactable = true;
        }
        else
        {
            _moveButt.interactable = false;
        }

        if(!_currCharacter.Strategy.HasAttacked)
        {
            _attButt.interactable = true;
            _abilButt.interactable = true;
        }
        else
        {
            _attButt.interactable = false;
            _abilButt.interactable = false;
        }
    }

    public void HideUI()
    {
        _movePos = _startPos;
        StartMove();
        if (!_hidden || !_abilityPanel.IsHidden)
        {
            _hidden = true;
            GameUpdate.UISubscribe += FadeOutUI;
        }
    }

    public void BackToMenu()
    {
        if (_currCharacter != null)
        {
            WorldGridHandler.WorldInstance.ResetPanels();
            ShowUI();
        }
    }

    public void SelectMove()
    {
        if (_currCharacter != null)
        {
            ChangeBattleInteraction(UIInteractions.MOVESELECT);
            HideUI();
            //Debug.Log(_currCharacter.ClassName);
            _currCharacter.Move();

        }
    }

    public void SelectAttack()
    {
        if (_currCharacter != null)
        {
            ChangeBattleInteraction(UIInteractions.ATTACKSELECT);
            HideUI();
            _currCharacter.StartAttack();
        } 
    }


    public void SelectAbility()
    {
        if (_currCharacter != null)
        {
            _abilityPanel.BringUpAbilities();
            CalculateMove();
            GameUpdate.UISubscribe += FadeOutUI;
        }
    }

    public void ShowAbilityPanel()
    {
        CalculateMove();
        _abilityPanel.BringUpAbilities();
    }

    public void HideAbilities()
    {
        CalculateMove();
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
            GameUpdate.UISubscribe += MoveUI;
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
            GameUpdate.UISubscribe -= FadeInUI;
        }

        _mainPanel.alpha = RandomThings.Interpolate(_fadeCurrTime, _mainPanel.alpha, 1);
    }

    public void FadeOutUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime > 1)
        {
            _mainPanel.alpha = 0;
            _mainPanel.blocksRaycasts = false;
            _hidden = true;
            GameUpdate.UISubscribe -= FadeOutUI;
        }

        _mainPanel.alpha = RandomThings.Interpolate(_fadeCurrTime, _mainPanel.alpha, 0);
    }

    public void MoveUI()
    {
        _moveCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_moveCurrTime > 1)
        {
            _myRect.localPosition = _movePos;
            _currPos = _myRect.localPosition;
            _moving = false;
            GameUpdate.UISubscribe -= MoveUI;
        }

        _myRect.localPosition = RandomThings.Interpolate(_moveCurrTime, _currPos, _movePos);
    }

    public void ChangeBattleInteraction(UIInteractions state)
    {
        _batRef.GetInteractionState = state;
    }

    public void ResetPosition()
    {
        _movePos = _startPos;
        StartMove();
    }

    public void EndButtonPressed()
    {
        _currCharacter = null;
        _batRef.EndCurrentTurn();
    }

    public void GoStraightToAbilityMenu()
    {
        _movePos = _currPos + (Vector3.left * 300f);
    }

    public void ResetUIMovements()
    {
        //Debug.Log("real ui hide called");
        _abilityPanel.ResetFading();
        HideUI();
    }
}
