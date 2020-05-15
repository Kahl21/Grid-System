using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    UIHolder _uiRef;

    //upper UI
    Text _heightText;
    GameObject _timeline;

    //short description
    GameObject _shortHolder;
    Image _shortPort;
    Text _shortHealth;
    Text _shortMana;

    //detailed description
    GameObject _detailHolder;
    Image _detPort;
    Text _detCharInfo;
    bool _movingDetailsIntoPosition = false, _moving = false;
    Vector3 _detailStartPos, _detailEndPos;
    float _currTime, _startTime;
    float _speedDelta = 2f;

    //battle menu
    CharacterOptions _optionsRef;
    CameraFollow _battleCam;

    public void Init(UIHolder ui, CameraFollow camref)
    {
        _uiRef = ui;
        _battleCam = camref;

        SetUI();
    }

    private void Update()
    {
        if (_moving && !MoveDetailsOnScreen())
        {
            _moving = false;
        }
    }

    void SetUI()
    {
        GameObject heightholder = transform.GetChild(0).gameObject;
        _heightText = heightholder.transform.GetChild(0).GetComponent<Text>();

        GameObject timelineholder = transform.GetChild(1).gameObject;
        _timeline = timelineholder.transform.GetChild(0).gameObject;

        _shortHolder = transform.GetChild(2).gameObject;
        _shortPort = _shortHolder.transform.GetChild(0).GetComponent<Image>();
        _shortHealth = _shortHolder.transform.GetChild(1).GetComponent<Text>();
        _shortMana = _shortHolder.transform.GetChild(2).GetComponent<Text>();
        _shortHolder.SetActive(false);

        _optionsRef = transform.GetChild(3).GetComponent<CharacterOptions>();
        _optionsRef.gameObject.SetActive(false);
        _optionsRef.Init(this);

        _detailHolder = transform.GetChild(4).gameObject;
        _detailStartPos = _detailHolder.GetComponent<RectTransform>().position;
        _detailEndPos = _detailStartPos;
        _detailEndPos.x = Screen.width;
        _detPort = _detailHolder.transform.GetChild(0).GetComponent<Image>();
        _detCharInfo = _detailHolder.transform.GetChild(1).GetComponent<Text>();
        _detailHolder.SetActive(false);
    }

    public void SetHeight(float height)
    {
        _heightText.text = "H " + height.ToString();
    }

    public void HighlightCharacter(GridToken highlightedToken)
    {
        Character highlightedCharacter = highlightedToken.GetCharacter;

        if (!_shortHolder.activeInHierarchy)
        {
            _shortPort.sprite = highlightedCharacter.GetSprite;
            _shortHealth.text = highlightedCharacter.CurrHealth.ToString() + "/" + highlightedCharacter.MaxHealth.ToString();
            _shortMana.text = highlightedCharacter.CurrMana.ToString() + "/" + highlightedCharacter.MaxMana.ToString();

            _shortHolder.SetActive(true);
        }
    }

    public void UnHighlightCharacter()
    {
        if (_shortHolder.activeInHierarchy)
        {
            _shortHolder.SetActive(false);
        }
    }

    public void ShowDetail(GridToken clickedOnToken)
    {
        Character clickedOnCharacter = clickedOnToken.GetCharacter;

        _detPort.sprite = clickedOnCharacter.GetSprite;

        string characterInfo = "Team " + clickedOnCharacter.Team + "\n";
        characterInfo += clickedOnCharacter.Name + "\n"
                        + "Weapon: " + clickedOnCharacter.HeldWeapon.Name + " --> " + clickedOnCharacter.HeldWeapon.WeaponElement + "\n\n"
                        + "Health: " + clickedOnCharacter.CurrHealth + "/" + clickedOnCharacter.MaxHealth + "\n"
                        + "Mana: " + clickedOnCharacter.CurrMana + "/" + clickedOnCharacter.MaxMana + "\n"
                        + "Strength: " + (clickedOnCharacter.Offense.Strength + clickedOnCharacter.HeldWeapon.StrengthMod) + " (" + clickedOnCharacter.HeldWeapon.StrengthMod + ")" + "\n"
                        + "Magic: " + (clickedOnCharacter.Offense.Strength + clickedOnCharacter.HeldWeapon.MagicMod) + "(" + clickedOnCharacter.HeldWeapon.MagicMod + ")" + "\n"
                        + "Defense: " + clickedOnCharacter.Defense.BaseDefense + "\n"
                        + "Accuracy: " + (clickedOnCharacter.Offense.Accuracy + clickedOnCharacter.HeldWeapon.Accuracy) + "%" + " (" + clickedOnCharacter.HeldWeapon.Accuracy + ")" + "\n"
                        + "Critical Chance: " + (clickedOnCharacter.Offense.CriticalChance + clickedOnCharacter.HeldWeapon.Crit) + "%" + " (" + clickedOnCharacter.HeldWeapon.Crit + ")" + "\n"
                        + "Speed: " + clickedOnCharacter.Speed + "\n"
                        + "Movement: " + clickedOnCharacter.Movement + "\n\n"
                        + "Resistances: Fire/" + clickedOnCharacter.Defense.FireRes + "\n"
                        + "             Ice/" + clickedOnCharacter.Defense.IceRes + "\n"
                        + "             Thunder/" + clickedOnCharacter.Defense.ThunderRes + "\n"
                        + "             Light/" + clickedOnCharacter.Defense.LightRes + "\n"
                        + "             Dark/" + clickedOnCharacter.Defense.DarkRes + "\n\n";

        _detCharInfo.text = characterInfo;

        _detailHolder.SetActive(true);
        _movingDetailsIntoPosition = true;
        _battleCam.FocusObject(clickedOnToken.gameObject, CameraModes.ZOOMING);
        _moving = true;
        _startTime = Time.time;
    }

    public void UnShowDetail()
    {
        _movingDetailsIntoPosition = false;
        _battleCam.ResetCamera();
        _moving = true;
        _startTime = Time.time;
    }

    public bool MoveDetailsOnScreen() 
    {
        _currTime = (Time.time - _startTime) / _speedDelta;
        Debug.Log("MovingUI");
        if(_currTime > 1)
        {
            _currTime = 1;
            if (!_movingDetailsIntoPosition)
            {
                _detailHolder.SetActive(false);
            }
            return false;
        }

        Vector3 currPos = _detailHolder.GetComponent<RectTransform>().position;

        if (_movingDetailsIntoPosition)
        {
            _detailHolder.GetComponent<RectTransform>().position = RandomThings.Interpolate(_currTime, currPos, _detailEndPos);
        }
        else
        {
            _detailHolder.GetComponent<RectTransform>().position = RandomThings.Interpolate(_currTime, currPos, _detailStartPos);
        }

        return true;
    }

    public void ShowActionsMenu(GridToken chara)
    {
        if(chara.GetCharacter.Team == TeamType.PLAYER)
        {
            //_optionsRef.gameObject.SetActive(true);
        }

        _battleCam.FocusObject(chara.gameObject, CameraModes.MOVING);
    }
}
