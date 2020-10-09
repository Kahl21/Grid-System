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
    InputField _numberOfEnemiesText;
    int _numberOfEnemies;
    [SerializeField]
    InputField _numberOfTeamsText;
    int _numberOfTeams;
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
    [SerializeField]
    Slider _autoSpeedSlider;
    [SerializeField]
    GameObject _ddOBJ;
    bool _fightStarted;

    [Header("AutoFight Vars")]
    [SerializeField]
    Text _autoText;
    float _autoSpeed = 1;
    public float AutoSpeed { get { return _autoSpeed; } set { _autoSpeed = value; } }

    float _currtime;
    float _starttime;

    bool _autoFight;


    //Init
    private void Awake()
    {
        SpellBook.LoadSpellBook();
        _fightStarted = false;
        _BFUI.SetActive(true);
        _AFUI.SetActive(false);
        _numberOfTeams = 2;
        SetNumberOfTeamsText();
        _numberOfEnemies = 1;
        SetNumberOfEnemiesText();
        _gridX = 4;
        _gridY = 4;
        SetGridSizeText();
    }

    public void ResetGame()
    {
        _ddOBJ.SetActive(false);
        _autoFight = false;
        _autoSpeed = 0;
        _autoSpeedSlider.value = 0;
        _autoText.text = "Off";
        _AFUI.SetActive(false);
        _BFUI.SetActive(true);
    }

    private void Update()
    {
        if (_fightStarted)
        {
            if(_autoFight)
            {
                _currtime = (Time.time - _starttime) * _autoSpeed;
                if(_currtime > 1)
                {
                    if (FightHandler.DoubleDamage)
                    {
                        ActivateDoubleDamage();
                    }
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

    public void AdjustAutoSpeed()
    {
        _autoSpeed = _autoSpeedSlider.value;
    }
    
    public void StopAuto()
    {
        _autoFight = false;
        _autoText.text = "Off";
    }

    public void ActivateDoubleDamage()
    {
        _ddOBJ.SetActive(true);
    }

    //Sets Text to Current Number of Enemies
    void SetNumberOfEnemiesText()
    {
        _numberOfEnemiesText.text = _numberOfEnemies.ToString();
    }

    public void CheckNumberOfEnemies()
    {
        try
        {
            int temp = System.Convert.ToInt32(_numberOfEnemiesText.text);

            if(temp * _numberOfTeams > _gridX * _gridY)
            {
                _numberOfEnemies = (_gridX * _gridY) / _numberOfTeams;
                SetNumberOfEnemiesText();
            }
            else
            {
                _numberOfEnemies = temp;
                SetNumberOfEnemiesText();
            }
        }
        catch
        {
            _numberOfEnemies = 1;
            SetNumberOfEnemiesText();
        }        
    }

    public void SetNumberOfTeamsText()
    {
        _numberOfTeamsText.text = _numberOfTeams.ToString();
    }

    public void CheckNumberOfTeams()
    { 
        try
        {
            int temp = System.Convert.ToInt32(_numberOfTeamsText.text);
            if(temp > _gridX * _gridY)
            {
                _numberOfTeams = _gridX * _gridY;
                CheckNumberOfEnemies();
                SetNumberOfTeamsText();
            }
            else
            {
                _numberOfTeams = temp;
                CheckNumberOfEnemies();
                SetNumberOfTeamsText();
            }
        }
        catch
        {
            _numberOfTeams = 2;
            CheckNumberOfEnemies();
            SetNumberOfTeamsText();
        }
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
        FightHandler.Init(_numberOfEnemies, _numberOfTeams);
        _AFUI.SetActive(true);
        _ddOBJ.SetActive(false);
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
            if (FightHandler.DoubleDamage)
            {
                ActivateDoubleDamage();
            }

            HistoryHandler.AdvanceHistory();
        }
        else
        {
            HistoryHandler.ReverseHistory();
        }

        SetFightInfo();
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
