using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDescription: MonoBehaviour, IMoveable, IFadeable
{
    BattleUI _uiRef;

    CanvasGroup _mainPanel;
    RectTransform _myRect;
    Image _detPort;
    Text _detCharInfo;
    Vector3 _detailStartPos, _detailEndPos, _movePos;
    float _currTime, _startTime;
    float _speedDelta = .5f;
    float _fadeCurrTime;
    bool _hidden;

    public void Init(BattleUI ui)
    {
        _uiRef = ui;
        _mainPanel = GetComponent<CanvasGroup>();
        _myRect = GetComponent<RectTransform>();
        SetUI();
        _hidden = true;
    }

    public void SetUI()
    {
        CalculateMoveDistance();
        _myRect.anchoredPosition = _detailStartPos;
        _detPort = transform.GetChild(0).GetComponent<Image>();
        _detCharInfo = transform.GetChild(1).GetComponent<Text>();
        _mainPanel.alpha = 0;
        _mainPanel.blocksRaycasts = false;
    }

    public void ShowUI(Character charInfo)
    {
        PlugInfo(charInfo);
        CalculateMove();
        GameUpdate.Subscribe += FadeInUI;
    }

    public void HideUI()
    {
        CalculateMove();
        GameUpdate.Subscribe += FadeOutUI;
    }

    void PlugInfo(Character info)
    {
        //Plug in all character details
        _detPort.sprite = info.GetSprite;

        string characterInfo = "Team " + info.Team + "\n";
        characterInfo += info.Name + "\n"
                        + "Weapon: " + info.HeldWeapon.Name + " --> " + info.HeldWeapon.WeaponElement + "\n\n"
                        + "Health: " + info.CurrHealth + "/" + info.MaxHealth + "\n"
                        + "Mana: " + info.CurrMana + "/" + info.MaxMana + "\n"
                        + "Strength: " + (info.Offense.Strength + info.HeldWeapon.StrengthMod) + " (" + info.HeldWeapon.StrengthMod + ")" + "\n"
                        + "Magic: " + (info.Offense.Strength + info.HeldWeapon.MagicMod) + "(" + info.HeldWeapon.MagicMod + ")" + "\n"
                        + "Defense: " + info.Defense.BaseDefense + "\n"
                        + "Accuracy: " + (info.Offense.Accuracy + info.HeldWeapon.Accuracy) + "%" + " (" + info.HeldWeapon.Accuracy + ")" + "\n"
                        + "Critical Chance: " + (info.Offense.CriticalChance + info.HeldWeapon.Crit) + "%" + " (" + info.HeldWeapon.Crit + ")" + "\n"
                        + "Speed: " + info.Speed + "\n"
                        + "Movement: " + info.Movement + "\n\n"
                        + "Resistances: Fire/" + info.Defense.FireRes + "\n"
                        + "             Ice/" + info.Defense.IceRes + "\n"
                        + "             Thunder/" + info.Defense.ThunderRes + "\n"
                        + "             Light/" + info.Defense.LightRes + "\n"
                        + "             Dark/" + info.Defense.DarkRes + "\n\n";

        _detCharInfo.text = characterInfo;
    }

    void CalculateMoveDistance()
    {
        _detailStartPos = _myRect.anchoredPosition;
        _detailStartPos.x += _myRect.rect.width;
        _detailEndPos = Vector3.zero;
    }

    public void CalculateMove()
    {
        if(!_hidden)
        {
            _movePos = _detailStartPos;
        }
        else
        {
            _movePos = _detailEndPos;
        }

        StartMove();
    }

    public void StartMove()
    {
        //turn on the UI and move it into position
        _startTime = Time.time;
        GameUpdate.Subscribe += MoveUI;
    }

    public void MoveUI()
    {
        _currTime = (Time.time - _startTime) / _speedDelta;
        //Debug.Log("MovingUI on");
        if (_currTime > 1)
        {
            _currTime = 1;
            //Debug.Log("details on");
            GameUpdate.Subscribe -= MoveUI;
        }


        //move UI on screen
        Vector3 newPos = _myRect.anchoredPosition;
        newPos.x = RandomThings.Interpolate(_currTime, _myRect.anchoredPosition.x, _movePos.x);
        _myRect.anchoredPosition = newPos;
    }

    public void FadeInUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _speedDelta;
        if (_fadeCurrTime > 1)
        {
            _mainPanel.alpha = 1;
            _mainPanel.blocksRaycasts = true;
            _hidden = false;
            _uiRef.GetInteractionState = UIInteractions.ZOOMED;
            GameUpdate.Subscribe -= FadeInUI;
        }

        _mainPanel.alpha = RandomThings.Interpolate(_fadeCurrTime, _mainPanel.alpha, 1);
    }

    public void FadeOutUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _speedDelta;
        if (_fadeCurrTime > 1)
        {
            _mainPanel.alpha = 0;
            _mainPanel.blocksRaycasts = false;
            _hidden = true;
            _uiRef.GetInteractionState = UIInteractions.FREE;
            GameUpdate.Subscribe -= FadeOutUI;
        }

        _mainPanel.alpha = RandomThings.Interpolate(_fadeCurrTime, _mainPanel.alpha, 0);
    }
}
