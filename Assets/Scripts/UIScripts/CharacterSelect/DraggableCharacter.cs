using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableCharacter : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    bool _onTeam = false;
    public bool OnTeam { get { return _onTeam; } set { _onTeam = value; } }

    Vector3 _startPos;

    PlayerClasses _myClass;
    public PlayerClasses GetClass { get { return _myClass; } }

    Sprite _mySprite;
    Color _myimageColor;
    public Color GetColor { get { return _myimageColor; } }
    string _myDescription;
    public string GetDescription { get { return _myDescription; } }

    CharacterSelectUI _uiRef;
    TeamSetUp _teamRef;
    Transform _parent;

    RectTransform _myRect;
    public void Init(PlayerClasses myrepresentedclass, CharacterSelectUI ui)
    {
        _parent = transform.parent;
        _myRect = GetComponent<RectTransform>();
        _startPos = _myRect.position;
        _myClass = myrepresentedclass;
        _uiRef = ui;

        switch (_myClass)
        {
            case PlayerClasses.WARRIOR:
                _myimageColor = Color.green;
                _mySprite = Resources.Load<Sprite>("UI/CharacterInfo/Sprites/Warrior");
                GetComponent<Image>().sprite = _mySprite;
                _myDescription = "A fighter through and through. He knows his way around the battlefield as well as many weapons. Good in a fight, bad at trivia night.";
                break;
            case PlayerClasses.MAGE:
                _myimageColor = Color.blue;
                GetComponent<Image>().color = _myimageColor;
                _myDescription = "A man shoruded in magic. His knowledge of the Arcane Arts allows him to use a plethora of spells and magical resistances. Just don't Expect him to help in a fist fight.";
                break;
            default:
                break;
        }

        /*Rect myrect = _myRect.rect;
        myrect.width = 150;
        myrect.height = myrect.width;
        Debug.Log(myrect.width);
        */
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _uiRef.ChangeCharacterInfo(_myDescription, _mySprite);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _uiRef.SelectedACharacter(this.GetComponent<RectTransform>());
    }

    public void UpdatePosition()
    {
        transform.position = Input.mousePosition;
    }

    public void ResetPosition()
    {
        _myRect.position = _startPos;
        _myRect.localScale = Vector3.one;
        GetComponent<Image>().raycastTarget = true;
        transform.SetParent(_parent);
    }

}
