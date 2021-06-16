using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class TeamSetUp : MonoBehaviour, IPointerClickHandler
{
    List<DraggableCharacter> _playerTeam;
    public List<DraggableCharacter> GetPlayerTeam { get { return _playerTeam; } }

    //int _xSpacing = 30;
    //int _ySpacing = 30;
    //int _ydisplacement;
    //int _charactersPerLine = 3;
    int _totalUnitsAllowed = 8;

    RectTransform _myRect;
    public RectTransform GetTransform { get { return _myRect; } }

    CharacterSelectUI _uiRef;

    public void Init(CharacterSelectUI ui)
    {
        _myRect = GetComponent<RectTransform>();
        _playerTeam = new List<DraggableCharacter>();
        _uiRef = ui;
    }

    public void AddTeamMember(DraggableCharacter newCharacter)
    {
        if(transform.childCount < _totalUnitsAllowed)
        {
            newCharacter.transform.SetParent(transform);
            newCharacter.GetComponent<RectTransform>().localScale = Vector3.one;
            newCharacter.GetComponent<Image>().raycastTarget = true;
            newCharacter.OnTeam = true;

            _playerTeam.Add(newCharacter);
        }
        else
        {
            _uiRef.RejectTeamAddition();

            //Debug.Log("teamsetupkill");
        }
    }

    public void RemoveTeamMember(DraggableCharacter chara)
    {
        _playerTeam.Remove(chara);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _uiRef.CheckForTeamAdd();
    }
}
