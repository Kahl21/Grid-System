using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityOptions : MonoBehaviour, IFadeable
{
    CanvasGroup _abilityMenu;
    GameObject _buttonPrefab;

    CanvasGroup _descHolder;
    Image _descImage;
    Text _descText;

    BattleUI _battleRef;
    CharacterOptions _optionsRef;
    Character _selectedChar;

    List<Ability> _currAbilities;
    List<Button> _currButtons;

    float _moveFadeSpeed = .3f;
    float _startTime;
    float _fadeCurrTime;

    bool _hidden, _moving;
    public bool IsHidden { get { return _hidden; } }
    public bool IsMoving { get { return _moving; } }

    public void Init(BattleUI bat, CharacterOptions opt)
    {
        _battleRef = bat;
        _optionsRef = opt;
        _selectedChar = null;
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

        _currAbilities = new List<Ability>();
        _currButtons = new List<Button>();
    }

    public void SetSkills(Character charRef)
    {
        if(_currAbilities.Count > 0)
        {
            ResetAbilityButtons();
        }

        _selectedChar = charRef;

        _currAbilities = _selectedChar.Abilities;

        float height = _buttonPrefab.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < _currAbilities.Count; i++)
        {
            GameObject newButton = Instantiate<GameObject>(_buttonPrefab, transform);

            Vector3 spawnPos = Vector3.zero + (Vector3.up * (((height * _currAbilities.Count) / 2) - (height * i)));

            newButton.GetComponent<RectTransform>().localPosition = spawnPos;

            //interesting. makes a delegate and then adds the function to it? then allows it to be used?
            newButton.GetComponent<Button>().onClick.AddListener(delegate { ShowSelectedSkill(_currAbilities[newButton.transform.GetSiblingIndex() - 2]); });
            _currButtons.Add(newButton.GetComponent<Button>());

            Text newtext = newButton.transform.GetChild(0).GetComponent<Text>();
            newtext.text = _currAbilities[i].Name;
        }
    }

    public void ShowSelectedSkill(Ability ab)
    {
        _battleRef.AbilitySelected(ab);
        HideUI();
    }

    public void HideUI()
    {
        _optionsRef.HideAbilities();
        _startTime = Time.time;
        GameUpdate.Subscribe += FadeOutUI;
    }

    public void BringUpAbilities()
    {
        GridHandler.StopSelection();
        //Debug.Log(_currAbilities.Count);
        _startTime = Time.time;
        GameUpdate.Subscribe += FadeInUI;
    }

    public void PreviousMenu()
    {
        _optionsRef.ShowUI();
        _startTime = Time.time;
        GameUpdate.Subscribe += FadeOutUI;
    }

    public void FadeInUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime > 1)
        {
            _abilityMenu.alpha = 1;
            _abilityMenu.blocksRaycasts = true;
            _hidden = false;
            _moving = false;
            GameUpdate.Subscribe -= FadeInUI;
        }

        _abilityMenu.alpha = RandomThings.Interpolate(_fadeCurrTime, _abilityMenu.alpha, 1);
    }

    public void FadeOutUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime > 1)
        {
            _abilityMenu.alpha = 0;
            _abilityMenu.blocksRaycasts = false;
            _descHolder.alpha = 0;
            _hidden = true;
            _moving = false;
            GameUpdate.Subscribe -= FadeOutUI;
        }

        _abilityMenu.alpha = RandomThings.Interpolate(_fadeCurrTime, _abilityMenu.alpha, 0);
        _descHolder.alpha = RandomThings.Interpolate(_fadeCurrTime, _descHolder.alpha, 0);
    }

    void ResetAbilityButtons()
    {
        for (int i = 0; i < _currAbilities.Count; i++)
        {
            Destroy(_currButtons[i].gameObject);
        }

        _currButtons = new List<Button>();
        _currAbilities = new List<Ability>();
    }

    public void ResetFading()
    {
        if (!_moving)
        {
            _moving = true;
            _startTime = Time.time;
            GameUpdate.Subscribe += FadeOutUI;
        }
    }
}
