using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHolder : MonoBehaviour
{
    //Before Fight UI
    [SerializeField]
    GameObject _BFUI;
    [SerializeField]
    Text _numberOfEnemiesText;
    int _numberOfEnemies;
    [SerializeField]
    Text _gridSizeText;
    int _gridX;
    int _gridY;

    //After Fight UI
    [SerializeField]
    GameObject _AFUI;
    [SerializeField]
    Text _historyText;
    [SerializeField]
    Text _gridHistoryText;
    [SerializeField]
    Text _numberOfActionsText;
    bool _fightStarted;

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
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeHistoryText(true);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeHistoryText(false);
            }
        }
    }

    //Sets Text to Current Number of Enemies
    void SetNumberOfEnemiesText()
    {
        _numberOfEnemiesText.text = _numberOfEnemies.ToString();
    }

    //Called Whenever _numberOfEnemies needs to be changed
    public void SetNumberOfEnemiesText(bool positiveIncrement)
    {
        if (positiveIncrement && _numberOfEnemies < 10)
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
        if (increase && _gridX < 10)
        {
            _gridX++;
        }
        else if(!increase && _gridX > 4)
        {
            _gridX--;
        }

        SetGridSizeText();
    }

    public void ChangeGridY(bool increase)
    {
        if (increase && _gridY < 10)
        {
            _gridY++;
        }
        else if (!increase && _gridY > 4)
        {
            _gridY--;
        }

        SetGridSizeText();
    }

    public void StartFight()
    {
        _BFUI.SetActive(false);
        GridHandler.CreateNewGrid(_gridX, _gridY);
        HistoryHandler.Init();
        FightHandler.Init(_numberOfEnemies, this);
        _AFUI.SetActive(true);

        SetHistoryText();
        _fightStarted = true;
    }

    void SetHistoryText()
    {
        _historyText.text = HistoryHandler.GetCurrentHistory();
        _gridHistoryText.text = HistoryHandler.GetCurrentGridHistory();
        _numberOfActionsText.text = HistoryHandler.GetCurrentActionNumber().ToString();
    }

    public void ChangeHistoryText(bool goForwardinHistory)
    {
        if(goForwardinHistory)
        {
            HistoryHandler.AdvanceHistory();
        }
        else
        {
            HistoryHandler.ReverseHistory();
        }

        SetHistoryText();
    }
}
