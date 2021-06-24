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

    public void Init()
    {
        _characterOrderList = new List<Character>();
        _characterOrderList = FightHandler.GetAllCharacters; 
        PopulateTimeline();
    }

    public void Init(BattleUI bref)
    {
        _battleRef = bref;
        _currentCharacterBorder = this.transform.GetChild(0).GetComponent<RectTransform>();

        Init();
    }
    

    private void PopulateTimeline()
    {
        _characterImages = new List<Image>();

        //Debug.Log("populate called");
        
        for (int i = 0; i < _characterOrderList.Count; i++)
        {
            GameObject spawnObj = new GameObject();
            spawnObj.AddComponent<RectTransform>();
            spawnObj.AddComponent<Image>();
            spawnObj.transform.SetParent(transform);
            spawnObj.GetComponent<Image>().sprite = _characterOrderList[i].GetSprite;
            Rect rectref = spawnObj.GetComponent<RectTransform>().rect;
            rectref.width = _imageSize;
            rectref.height = _imageSize;
            spawnObj.transform.localScale = Vector3.one;
            _characterImages.Add(spawnObj.GetComponent<Image>());
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
        _battleRef.StartNextTurn(_characterOrderList[0]);
    }

    public void EndOfTurn()
    {
        if(_characterImages.Count != 0 && _characterOrderList.Count != 0)
        {
            Image ImTemp = _characterImages[0];
            _characterImages.RemoveAt(0);
            _characterImages.Add(ImTemp);

            Character CharaTemp = _characterOrderList[0];
            _characterOrderList.RemoveAt(0);
            _characterOrderList.Add(CharaTemp);

            AlignTimeline();

            //next turn call here
            ContinueFight();
        }
        else
        {
            Debug.Log("no need to end turn probably because battle is over");
            return;
        }
        
    }

    public void RemovecharacterFromTimeline(Character deadchara)
    {
        int deadSpot = _characterOrderList.IndexOf(deadchara);

        GameObject portraitToDestroy = _characterImages[deadSpot].gameObject;
        _characterImages.RemoveAt(deadSpot);
        Destroy(portraitToDestroy);
        AlignTimeline();
    }

    public void ResetTimeline()
    {
        //_characterOrderList.Clear();
        for (int i = 0; i < _characterImages.Count; i++)
        {
            Destroy(_characterImages[i].gameObject);
        }
        _characterImages.Clear();
    }

    public Character GetCurrentTurnCharacter { get { return _characterOrderList[0]; } }
}
