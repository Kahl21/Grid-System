using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityOptions : MonoBehaviour
{
    CanvasGroup _abilityMenu;
    GameObject _buttonPrefab;

    CanvasGroup _descHolder;
    Image _descImage;
    Text _descText;

    CharacterOptions _optionsRef;

    List<Ability> _currAbilities;

    float _moveFadeSpeed = .3f;
    float _startTime;
    float _fadeCurrTime;

    bool _hidden;
    public bool IsHidden { get { return _hidden; } }

    public void Init(CharacterOptions opt)
    {
        _optionsRef = opt;
        _hidden = true;
        SetUI();
    }

    void SetUI()
    {
        _abilityMenu = GetComponent<CanvasGroup>();
        _abilityMenu.alpha = 0;
        _abilityMenu.blocksRaycasts = false;

        _buttonPrefab = Resources.Load<GameObject>("UI/AbilityUI/AbilityNameButton");

        _descHolder = transform.GetChild(0).GetComponent<CanvasGroup>();
        _descHolder.alpha = 0;
        _descHolder.blocksRaycasts = false;
        _descImage = transform.GetChild(0).GetComponent<Image>();
        _descText = transform.GetChild(1).GetComponent<Text>();
    }

    public void SetSkills(Character charRef)
    {
        _currAbilities = charRef.Abilities;

        float height = _buttonPrefab.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < _currAbilities.Count; i++)
        {
            GameObject newButton = Instantiate<GameObject>(_buttonPrefab, transform);

            Vector3 spawnPos = Vector3.zero + (Vector3.up * (((height * _currAbilities.Count) / 2) - (height * i)));

            newButton.GetComponent<RectTransform>().localPosition = spawnPos;

            //interesting. makes a delegate and then adds the function to it? then allows it to be used?
            newButton.GetComponent<Button>().onClick.AddListener(delegate { UseSelectedSkill(_currAbilities[newButton.transform.GetSiblingIndex() - 2]); });

            Text newtext = newButton.transform.GetChild(0).GetComponent<Text>();
            newtext.text = _currAbilities[i].Name;
        }
    }

    public void UseSelectedSkill(Ability ab)
    {
        Debug.Log("it works");
    }

    public void BringUpAbilities()
    {
        Debug.Log(_currAbilities.Count);
        _startTime = Time.time;
        GameUpdate.Subscribe += FadeMainInAbility;
    }

    public void PreviousMenu()
    {
        _optionsRef.ShowUI();
        _startTime = Time.time;
        GameUpdate.Subscribe += FadeMainOutAbility;
    }

    public void FadeMainInAbility()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime > 1)
        {
            _abilityMenu.alpha = 1;
            _abilityMenu.blocksRaycasts = true;
            _hidden = false;

            GameUpdate.Subscribe -= FadeMainInAbility;
        }

        _abilityMenu.alpha = RandomThings.Interpolate(_fadeCurrTime, 0, 1);
    }

    public void FadeMainOutAbility()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime > 1)
        {
            _abilityMenu.alpha = 0;
            _abilityMenu.blocksRaycasts = false;
            _descHolder.alpha = 0;
            _hidden = true;

            GameUpdate.Subscribe -= FadeMainOutAbility;
        }

        _abilityMenu.alpha = RandomThings.Interpolate(_fadeCurrTime, 1, 0);
        _descHolder.alpha = RandomThings.Interpolate(_fadeCurrTime, _descHolder.alpha, 0);
    } 

    public void ResetFading()
    {
        if (!_hidden)
        {
            _startTime = Time.time;
            GameUpdate.Subscribe += FadeMainOutAbility;
        }
    }
}
