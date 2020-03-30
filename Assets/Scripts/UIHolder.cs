using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHolder : MonoBehaviour
{
    [Header("Set-up UI")]
    [SerializeField]
    GameObject _BFUI;
    [SerializeField]
    Text _numberOfEnemiesText;
    int _numberOfEnemies;
    [SerializeField]
    Text _gridSizeText;
    int _gridX;
    int _gridY;

    [Header("Fight UI")]
    [SerializeField]
    GameObject _AFUI;
    [SerializeField]
    Text _historyText;
    [SerializeField]
    Text _gridHistoryText;
    [SerializeField]
    Text _characterInfoText;
    [SerializeField]
    Text _numberOfActionsText;
    bool _fightStarted;

    [Header("AutoFight Vars")]
    [SerializeField]
    Text _autoText;
    [SerializeField]
    float _autoSpeed = 1;
    float _currtime;
    float _starttime;

    bool _autoFight;


    //Init
    private void Awake()
    {
        _fightStarted = false;
        _BFUI.SetActive(true);
        _AFUI.SetActive(false);
        _numberOfEnemies = 2;
        SetNumberOfEnemiesText();
        _gridX = 4;
        _gridY = 4;
        SetGridSizeText();
    }

    private void Update()
    {
        if (_fightStarted)
        {
            if(_autoFight)
            {
                _currtime = (Time.time - _starttime) / _autoSpeed;
                if(_currtime > 1)
                {
                    HistoryHandler.AdvanceHistory();
                    SetFightInfo();
                    _starttime = Time.time;
                    _currtime = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeFightHistory(true);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeFightHistory(false);
            }
        }
    }

    public void AutoBattle()
    {
        if (_autoFight)
        {
            StopAuto();
        }
        else
        {
            _autoText.text = "On";

            _currtime = 0;
            _starttime = Time.time;

            _autoFight = true;
        }
    }
    
    public void StopAuto()
    {
        _autoFight = false;
        _autoText.text = "Off";
    }

    //Sets Text to Current Number of Enemies
    void SetNumberOfEnemiesText()
    {
        _numberOfEnemiesText.text = _numberOfEnemies.ToString();
    }

    //Called Whenever _numberOfEnemies needs to be changed
    public void SetNumberOfEnemiesText(bool positiveIncrement)
    {
        if (positiveIncrement && _numberOfEnemies < (_gridX * _gridY))
        {
            _numberOfEnemies++;
        }
        else if(!positiveIncrement && _numberOfEnemies > 2)
        {
            _numberOfEnemies--;
        }

        SetNumberOfEnemiesText();
    }

    void SetGridSizeText()
    {
        _gridSizeText.text = _gridX + " x " + _gridY;
    }

    public void ChangeGridX(bool increase)
    {
        if (increase)
        {
            _gridX++;
        }
        else if(!increase && (_gridX * _gridY) > 2 && ((_gridX - 1) * _gridY) > 0)
        {
            _gridX--;
            if(_numberOfEnemies > (_gridX * _gridY))
            {
                _numberOfEnemies = (_gridX * _gridY);
                SetNumberOfEnemiesText();
            }
        }

        SetGridSizeText();
    }

    public void ChangeGridY(bool increase)
    {
        if (increase)
        {
            _gridY++;
        }
        else if (!increase && (_gridX * _gridY) > 2 && (_gridX * (_gridY - 1)) > 0)
        {
            _gridY--;
            if (_numberOfEnemies > (_gridX * _gridY))
            {
                _numberOfEnemies = (_gridX * _gridY);
                SetNumberOfEnemiesText();
            }
        }

        SetGridSizeText();
    }

    public void StartFight()
    {
        _BFUI.SetActive(false);
        GridHandler.CreateNewGrid(_gridX, _gridY);
        HistoryHandler.Init(this);
        FightHandler.Init(_numberOfEnemies);
        _AFUI.SetActive(true);

        SetFightInfo();
        _fightStarted = true;
    }

    public void SetFightInfo()
    {
        _historyText.text = HistoryHandler.GetCurrentHistory();
        _gridHistoryText.text = HistoryHandler.GetCurrentGridHistory();
        _characterInfoText.text = HistoryHandler.GetCurrentCharacterInfo();
        _numberOfActionsText.text = HistoryHandler.GetCurrentActionNumber().ToString();
    }

    public void ChangeFightHistory(bool goForwardinHistory)
    {
        if (_autoFight)
        {
            StopAuto();
        }

        if (goForwardinHistory)
        {
            HistoryHandler.AdvanceHistory();
        }
        else
        {
            HistoryHandler.ReverseHistory();
        }

        SetFightInfo();
    }
}
