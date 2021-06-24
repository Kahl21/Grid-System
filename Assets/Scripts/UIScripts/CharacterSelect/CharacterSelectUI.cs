using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Character Select UI")]
    GameObject _characterSelectedInfo;
    Image _characterSelectedImage;
    Text _characterSelectedInfoText;
    TeamSetUp _playerTeamInfo;
    DraggableCharacter _currDC;
    Button _toGridButton;
    GameObject _trashcan;

    UIHolder _uiRef;

    bool _initalized = false;

    public void Init()
    {
        if (!_initalized)
        {
            _uiRef = UIHolder.UIInstance;
            SetUI();
            _initalized = true;
        }

        GameUpdate.PlayerSubscribe += TryMoveSelectedUI;
    }


    void SetUI()
    {
        _characterSelectedInfo = this.transform.GetChild(1).gameObject;
        _characterSelectedImage = _characterSelectedInfo.transform.GetChild(0).GetComponent<Image>();
        _characterSelectedInfoText = _characterSelectedInfo.transform.GetChild(1).GetComponent<Text>();

        GameObject CharacterSelectBox = this.transform.GetChild(2).gameObject;

        GameObject teamholderholder = this.transform.GetChild(3).gameObject;
        _playerTeamInfo = teamholderholder.transform.GetChild(1).GetComponent<TeamSetUp>();
        _playerTeamInfo.Init(this);

        _trashcan = this.transform.GetChild(4).gameObject;
        _trashcan.SetActive(false);

        GameObject _characterSelectPrefab = Resources.Load<GameObject>("UI/CharacterInfo/CharacterUI");

        for (int i = 0; i <= (int)PlayerClasses.WARRIOR; i++)
        {
            GameObject newobject = Instantiate(_characterSelectPrefab, CharacterSelectBox.transform);

            newobject.GetComponent<DraggableCharacter>().Init((PlayerClasses)i, this);
        }
    }

    public void NextMenu()
    {
        if(_playerTeamInfo.GetPlayerTeam.Count > 0)
        {
            _uiRef.CharacterSelectDone(_playerTeamInfo.GetPlayerTeam);
            GameUpdate.PlayerSubscribe -= TryMoveSelectedUI;
        }
    }

    public void ChangeCharacterInfo(string characterDesc, Sprite characterColor)
    {
        _characterSelectedImage.sprite = characterColor;
        _characterSelectedInfoText.text = characterDesc;
    }

    public void SelectedACharacter(RectTransform charaSquare)
    {
        DraggableCharacter dragObj = charaSquare.GetComponent<DraggableCharacter>();

        if (_currDC == null && charaSquare != null)
        {
            if (dragObj.OnTeam)
            {
                _currDC = dragObj;
                _currDC.GetComponent<Image>().raycastTarget = false;
                _currDC.transform.SetParent(transform);
                _trashcan.SetActive(true);
            }
            else
            {
                SpawnNewCharaSquare(charaSquare);
            }
        }
        else if (_currDC != null)
        {
            RejectTeamAddition();

            SpawnNewCharaSquare(charaSquare);
        }
    }

    void SpawnNewCharaSquare(RectTransform charaSquare)
    {
        RectTransform newchara = Instantiate(charaSquare, Input.mousePosition, Quaternion.identity, this.transform);

        //set width and height of new instantiated object cuz it wont start on the canvas for some reason >:(
        newchara.sizeDelta = new Vector2(150, 150);

        newchara.GetComponent<Image>().raycastTarget = false;

        _currDC = newchara.GetComponent<DraggableCharacter>();
        _currDC.Init(charaSquare.GetComponent<DraggableCharacter>().GetClass, this);
        _trashcan.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currDC != null)
        {
            _playerTeamInfo.RemoveTeamMember(_currDC);
            RejectTeamAddition();
            // Debug.Log("onpointerkill");
        }
    }

    void TryMoveSelectedUI()
    {
        if (_currDC != null)
        {
            _currDC.UpdatePosition();
        }
    }

    public void CheckForTeamAdd()
    {
        if (_currDC != null)
        {
            _playerTeamInfo.AddTeamMember(_currDC);
            _currDC = null;
            _trashcan.SetActive(false);
        }
    }
    public void RejectTeamAddition()
    {
        Destroy(_currDC.gameObject);
        _currDC = null;
        _trashcan.SetActive(false);
    }
}
