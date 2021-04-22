using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTimeLine : MonoBehaviour
{
    List<Character> _characterOrderList;
    List<Image> _characterImages;

    int _imageSize = 100;

    BattleUI _battleRef;
    RectTransform _currentCharacterBorder;

    public void Init(BattleUI bref)
    {
        _characterOrderList = new List<Character>();
        _characterOrderList = FightHandler.GetAllCharacters;
        _battleRef = bref;
        _currentCharacterBorder = this.transform.GetChild(0).GetComponent<RectTransform>();
        
        PopulateTimeline();
    }

    private void PopulateTimeline()
    {
        _characterImages = new List<Image>();

        //Debug.Log("populate called");
        GameObject spawnObj = new GameObject();
        spawnObj.AddComponent<RectTransform>();
        spawnObj.AddComponent<Image>();
        Rect rectref = spawnObj.GetComponent<RectTransform>().rect;
        rectref.width = _imageSize;
        rectref.height = _imageSize;

        for (int i = 0; i < _characterOrderList.Count; i++)
        {
            GameObject newObj = Instantiate(spawnObj, transform);
            newObj.GetComponent<Image>().sprite = _characterOrderList[i].GetSprite;
            _characterImages.Add(newObj.GetComponent<Image>());
        }

        AlignTimeline();
    }

    private void AlignTimeline()
    {

        float newxPos = 0;

        newxPos = -1 *(_imageSize / 2) * (_characterImages.Count-1);

        _currentCharacterBorder.anchoredPosition = new Vector3(newxPos, 0, 0);

        for (int i = 0; i < _characterImages.Count; i++)
        {
            if(i != 0)
            {
                newxPos += _imageSize;
            }

            _characterImages[i].rectTransform.anchoredPosition = new Vector3(newxPos, 0, 0);
            _characterImages[i].transform.SetAsFirstSibling();
        }
    }

    //Starts the fight and asks enemies what they want to do
    //does not end until 1 enemy is left
    public void ContinueFight()
    {
        Debug.Log(_characterOrderList[0].ClassName + " going");
        _characterOrderList[0].StartTurn();
    }

    public void EndOfTurn()
    {
        Image ImTemp = _characterImages[0];
        _characterImages.RemoveAt(0);
        _characterImages.Add(ImTemp);
        
        Character CharaTemp = _characterOrderList[0];
        _characterOrderList.RemoveAt(0);
        _characterOrderList.Add(CharaTemp);

        AlignTimeline();

        //next turn call here
    }

    public void RemovePlayerFromTimeline(Character deadchara)
    {
        _characterImages.RemoveAt(_characterOrderList.IndexOf(deadchara));
        _characterOrderList.Remove(deadchara);
        AlignTimeline();
    }

    public void ResetTimeline()
    {
        _characterOrderList.Clear();
        for (int i = 0; i < _characterImages.Count; i++)
        {
            Destroy(_characterImages[i].gameObject);
        }
        _characterImages.Clear();
    }

    public Character GetCurrentTurnCharacter { get { return _characterOrderList[0]; } }
}
