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

    [SerializeField]
    float _moveFadeSpeed = .3f;
    float _startTime;
    float _fadeCurrTime;

    bool _hidden, _moving;
    public bool IsHidden { get { return _hidden; } }
    public bool IsMoving { get { return _moving; } }

    //Initialize Ability Options
    public void Init(BattleUI bat, CharacterOptions opt)
    {
        _battleRef = bat;
        _optionsRef = opt;
        _selectedChar = null;
        _hidden = true;
        SetUI();
    }

    //Adds references of all child objects or loadables
    //sets this Ui piece to its base state
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
    
    //looks at all skills that selected character has
    //creates buttons for however many skills there are
    //binds function calls of appropriate skills to generated buttons
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

    //sends info of whatever skill button is clicked on to BattleUI
    //then hides itself, to clear screen, so BattleUI can highlight tiles on screen
    public void ShowSelectedSkill(Ability ab)
    {
        _battleRef.AbilitySelected(ab);
        HideUI();
    }

    //adds "fading" interface functions to GameUpdate to fade out UI
    public void HideUI()
    {
        FullReset();
        _startTime = Time.time;
        GameUpdate.UISubscribe += FadeOutUI;
    }

    //adds "fading" interface functions to GameUpdate to fade out UI
    public void BringUpAbilities()
    {
        WorldGridHandler.WorldInstance.ResetPanels();
        //Debug.Log(_currAbilities.Count);
        _startTime = Time.time;
        GameUpdate.UISubscribe += FadeInUI;
    }

    //calling this hides AbilityOptions UI
    //Then brings the CharacterOptions UI back up
    public void PreviousMenu()
    {
        _optionsRef.ShowUI();
        _startTime = Time.time;
        GameUpdate.UISubscribe += FadeOutUI;
    }

    //IFadeable Interface
    //Fades in UI to full alpha
    public void FadeInUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime >= 1)
        {
            _abilityMenu.alpha = 1;
            _abilityMenu.blocksRaycasts = true;
            _hidden = false;
            _moving = false;
            GameUpdate.UISubscribe -= FadeInUI;
        }

        _abilityMenu.alpha = RandomThings.Interpolate(_fadeCurrTime, _abilityMenu.alpha, 1);
    }

    //IFadeable Interface
    //Fades out UI to no alpha
    public void FadeOutUI()
    {
        _fadeCurrTime = (Time.time - _startTime) / _moveFadeSpeed;
        if (_fadeCurrTime >= 1)
        {
            _abilityMenu.alpha = 0;
            _abilityMenu.blocksRaycasts = false;
            _descHolder.alpha = 0;
            _hidden = true;
            _moving = false;
            GameUpdate.UISubscribe -= FadeOutUI;
        }

        _abilityMenu.alpha = RandomThings.Interpolate(_fadeCurrTime, _abilityMenu.alpha, 0);
        _descHolder.alpha = RandomThings.Interpolate(_fadeCurrTime, _descHolder.alpha, 0);
    }

    //destroys all current buttons for abilities
    //resets all lists to empty for next set of abilities to come in
     public void ResetAbilityButtons()
    {
        for (int i = 0; i < _currAbilities.Count; i++)
        {
            Destroy(_currButtons[i].gameObject);
        }

        _currButtons = new List<Button>();
        _currAbilities = new List<Ability>();
    }

    //resets ability options (from whatever state its in)
    //makes itself hidden and unclickable
    public void ResetFading()
    {
        if (!_moving)
        {
            _moving = true;
            _startTime = Time.time;
            GameUpdate.UISubscribe += FadeOutUI;
        }
    }
    
    public void FullReset()
    {
        ResetFading();
        _optionsRef.ResetPosition();
    }
}
