﻿using UnityEngine;
using UnityEngine.UI;

public enum UIInteractions
{
    NONE,
    FREE,
    ZOOMED,
    MENUOPEN,
    ABILITYOPEN,
    MOVESELECT,
    ATTACKSELECT,
    ABILITYSELECT,
    ABILITYUSE
}

public class BattleUI : MonoBehaviour
{
    UIHolder _uiRef;

    Character _selectedCharacter;
    Ability _selectedAbility;
    Vector2 _abilityTarget;

    //upper UI
    Text _heightText;
    GameObject _timeline;

    //short description
    GameObject _shortHolder;
    Image _shortPort;
    Text _shortHealth;
    Text _shortMana;

    //detailed description
    CharacterDescription _detailHolder;


    //battle menu
    CharacterOptions _optionsRef;
    CameraFollow _battleCam;
    public Camera BattleCamera { get { return _battleCam.GetComponent<Camera>(); } }

    UIInteractions _interactState = UIInteractions.NONE;
    public UIInteractions GetInteractionState { get { return _interactState; } set { _interactState = value; } }

    //initalize
    public void Init(UIHolder ui, CameraFollow camref)
    {
        _uiRef = ui;
        _battleCam = camref;
        _interactState = UIInteractions.FREE;

        SetUI();
    }

    //grabs all relevent UI components on children
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
        _optionsRef.Init(this);

        _detailHolder = transform.GetChild(4).GetComponent<CharacterDescription>();
        _detailHolder.Init(this);

        GameUpdate.Subscribe += PlayerInteract;
    }

    //Sets the height number of height UI
    public void SetHeight(float height)
    {
        _heightText.text = "H " + height.ToString();
    }

    //Interactions with 3D space that changes depending on 
    void PlayerInteract()
    {
        CheckingHeight();
        CheckingCharacter(); 
        UndoSelection();

        switch (_interactState)
        {
            case UIInteractions.NONE:
                break;
            case UIInteractions.FREE:
                LookingMode();
                break;
            case UIInteractions.ZOOMED:
                CheckForExitingClick();
                break;
            case UIInteractions.MENUOPEN:
                UndoSelection();
                break;
            case UIInteractions.MOVESELECT:
                MoveInteract();
                break;
            case UIInteractions.ATTACKSELECT:
                AttackInteract();
                break;
            case UIInteractions.ABILITYSELECT:
                AbilityInteract();
                break;
            case UIInteractions.ABILITYUSE:
                FinalizeAbility();
                break;
            default:
                break;
        }
    }

    void CheckingHeight()
    {
        Ray ray = _battleCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Debug.DrawRay(ray.origin, ray.direction, Color.black);
        //Debug.Log("casting");
        if (Physics.Raycast(ray, out hit))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = hit.collider;

            //set height UI
            SetHeight(objCollider.transform.position.y);
        }
        else
        {
            //Set height to nothing
            SetHeight(0f);
        }
    }

    void CheckingCharacter()
    {
        Ray ray = _battleCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = hit.collider;

            if (objCollider.GetComponent<Tile>())
            {
                //hover over tile
                //set short character UI

                //Debug.Log("tile hover");
                Tile tile = objCollider.GetComponent<Tile>();

                if (tile.PersonOnMe != null)
                {
                    HighlightCharacter(tile);
                }
            }
            else if (objCollider.GetComponent<GridToken>())
            {
                //Debug.Log("character hover");
                GridToken gt = objCollider.GetComponent<GridToken>();

                HighlightCharacter(gt.GetTile);
            }
        }
        else
        {
            //turn off character detail
            UnHighlightCharacter();
        }
    }

    void LookingMode()
    {
        if (_battleCam.GetCameraMode == CameraModes.FREE && !_optionsRef.IsMoving)
        {
            Ray ray = _battleCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(_selectedCharacter == null)
            {
                //Debug.DrawRay(ray.origin, ray.direction, Color.black);
                //Debug.Log("casting");
                if (Physics.Raycast(ray, out hit))
                {
                    //if hits something

                    //Debug.Log("hit something");
                    Collider objCollider = hit.collider;

                    if (objCollider.GetComponent<Tile>())
                    {
                        //hover over tile
                        //set short character UI

                        //Debug.Log("tile hover");
                        Tile tile = objCollider.GetComponent<Tile>();
                        if (tile.PersonOnMe != null)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                //if left click
                                //move camera to focus on character

                                ShowActionsMenu(tile);
                            }
                            else if (Input.GetMouseButtonDown(1))
                            {
                                //if right click
                                //focus and zoom on character
                                //show long detail on right side

                                ShowDetail(tile);
                            }
                        }
                    }
                    else if (objCollider.GetComponent<GridToken>())
                    {
                        //Debug.Log("character hover");
                        GridToken gt = objCollider.GetComponent<GridToken>(); 
                        
                        if (Input.GetMouseButtonDown(0))
                        {
                            //if left click
                            //move camera to focus on character

                            ShowActionsMenu(gt.GetTile);
                        }
                        else if (Input.GetMouseButtonDown(1))
                        {
                            //if right click
                            //focus and zoom on character
                            //show long detail on right side

                            ShowDetail(gt.GetTile);
                        }
                    }
                }
            }
            
        }
    }

    void MoveInteract()
    {
        Ray ray = _battleCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = hit.collider;

            if (objCollider.GetComponent<Tile>())
            {
                //hover over tile
                //set short character UI

                Tile tile = objCollider.GetComponent<Tile>();
                if (tile.IsTargetable && Input.GetMouseButtonDown(0))
                {
                    _battleCam.CameraFullStop();
                    GridHandler.DijkstraMove(_selectedCharacter, tile);
                }
            }
        }
    }

    void AttackInteract()
    {
        Ray ray = _battleCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = hit.collider;

            if (objCollider.GetComponent<Tile>())
            {
                //hover over tile
                //set short character UI

                InitiateAttack(objCollider.GetComponent<Tile>());
                
            }
            else if (objCollider.GetComponent<GridToken>())
            {
                InitiateAttack(objCollider.GetComponent<GridToken>().GetTile);
            }
        }
    }

    void InitiateAttack(Tile tile)
    {
        if (tile.IsTargetable && Input.GetMouseButtonDown(0) && tile.PersonOnMe != null)
        {
            _battleCam.CameraFullStop();
            _selectedCharacter.Attack(GridHandler.RetrieveCharacter(tile.GetXPosition, tile.GetYPosition));
        }
    }

    public void AbilitySelected(Ability ability)
    {
        _selectedAbility = ability;
        GridHandler.ShowReleventGrid(_selectedCharacter.CurrentPosition, _selectedAbility.TargetRange, Color.red, Actions.ATTACK);
        _interactState = UIInteractions.ABILITYSELECT;
    }

    void AbilityInteract()
    {
        Ray ray = _battleCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //if hits something

            //Debug.Log("hit something");
            Collider objCollider = hit.collider;

            if (objCollider.GetComponent<Tile>())
            {
                Tile tile = objCollider.GetComponent<Tile>();
                if (tile.IsTargetable && Input.GetMouseButtonDown(0))
                {
                    _abilityTarget = new Vector2(tile.GetXPosition, tile.GetYPosition);
                    GridHandler.ShowReleventGrid(_abilityTarget, _selectedAbility.SplashRange, Color.red, Actions.ABILITY);
                    _interactState = UIInteractions.ABILITYUSE;
                    _battleCam.FocusObject(tile.gameObject, CameraModes.ZOOMING);
                }

            }
            else if(objCollider.GetComponent<GridToken>())
            {
                GridToken gt = objCollider.GetComponent<GridToken>();
                if (gt.GetTile.IsTargetable && Input.GetMouseButtonDown(0))
                {
                    _abilityTarget = new Vector2(gt.GetTile.GetXPosition, gt.GetTile.GetYPosition);
                    GridHandler.ShowReleventGrid(_abilityTarget, _selectedAbility.SplashRange, Color.red, Actions.ABILITY);
                    _interactState = UIInteractions.ABILITYUSE;
                    _battleCam.FocusObject(gt.gameObject, CameraModes.ZOOMING);
                }
            }
        }
    }

    void FinalizeAbility()
    {
        if (Input.GetMouseButtonDown(0) && GridHandler.CheckForEnemyWithinSpashZone(_abilityTarget, _selectedAbility.SplashRange))
        {
            _optionsRef.HideUI();
            _selectedCharacter.Strategy.HasAttacked = true;
            _selectedAbility.ActivateSkill(_selectedCharacter, GridHandler.GetTargetsInSplashZone(_abilityTarget, _selectedAbility));
            _battleCam.FocusObject(GridHandler.RetrieveTile(_abilityTarget).gameObject, CameraModes.MOVING); 
        }
    }

    public void CharacterDoneMoving(GridToken currChar)
    {
        if (!_selectedCharacter.CheckForMoreMove(currChar.GetTile.GetXPosition, currChar.GetTile.GetYPosition))
        {
            _interactState = UIInteractions.MOVESELECT;
        }
        else
        {
            _optionsRef.ShowUI();
            _interactState = UIInteractions.MENUOPEN;
        }

        _battleCam.FocusObject(currChar.gameObject, CameraModes.MOVING);
    }

    public void CharacterDoneAttacking()
    {
        _optionsRef.ShowUI();
        _interactState = UIInteractions.MENUOPEN;
        _selectedCharacter.Strategy.HasAttacked = true;
        _battleCam.FocusObject(GridHandler.RetrieveToken(_selectedCharacter.CurrentPosition).gameObject, CameraModes.MOVING);
    }

    //if zoomed and click, unzoom and put details off screen
    void CheckForExitingClick()
    {
        if (!_battleCam.IsMoving)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                _interactState = UIInteractions.NONE;
                UnShowDetail();
            }
        }
    }

    void UndoSelection()
    {
        if (!_optionsRef.IsMoving)
        {
            if (CancelButtonPressed())
            {
                switch (_interactState)
                {
                    case UIInteractions.MENUOPEN:
                        if (!_optionsRef.IsHidden)
                        {
                            HideActionsMenu();
                        }
                        else
                        {
                            _optionsRef.BackToMenu();
                            _optionsRef.GetAbilityPanel.ResetFading();
                        }
                        break;
                    case UIInteractions.MOVESELECT:
                        _battleCam.FocusObject(GridHandler.RetrieveToken(_selectedCharacter.CurrentPosition).gameObject, CameraModes.MOVING);
                        _interactState = UIInteractions.MENUOPEN;
                        _optionsRef.BackToMenu();
                        _optionsRef.GetAbilityPanel.ResetFading();
                        break;
                    case UIInteractions.ATTACKSELECT:
                        _battleCam.FocusObject(GridHandler.RetrieveToken(_selectedCharacter.CurrentPosition).gameObject, CameraModes.MOVING);
                        _interactState = UIInteractions.MENUOPEN;
                        _optionsRef.BackToMenu();
                        _optionsRef.GetAbilityPanel.ResetFading();
                        break;
                    case UIInteractions.ABILITYSELECT:
                        _battleCam.FocusObject(GridHandler.RetrieveToken(_selectedCharacter.CurrentPosition).gameObject, CameraModes.MOVING);
                        _interactState = UIInteractions.MENUOPEN;
                        _optionsRef.ShowOnlyAbilites();
                        break;
                    case UIInteractions.ABILITYUSE:
                        _battleCam.FocusObject(GridHandler.RetrieveTile(_abilityTarget).gameObject, CameraModes.MOVING);
                        AbilitySelected(_selectedAbility);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    bool CancelButtonPressed()
    {
        //Debug.Log("Button canceling check");
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
        {
            //Debug.Log("back button pressed");
            return true;
        }
        else
        {
            //Debug.Log("back button not pressed");
            return false;
        }
    }

    //activates the short character detail UI
    void HighlightCharacter(Tile highlightedToken)
    {
        //get character info from GridHandler by sending GridToken Position
        Character highlightedCharacter = GridHandler.RetrieveCharacter(highlightedToken.GetXPosition, highlightedToken.GetYPosition);

        //turn on short detail if it isnt on 
        //plug character info into UI
        _shortPort.sprite = highlightedCharacter.GetSprite;
        _shortHealth.text = highlightedCharacter.CurrHealth.ToString() + "/" + highlightedCharacter.MaxHealth.ToString();
        _shortMana.text = highlightedCharacter.CurrMana.ToString() + "/" + highlightedCharacter.MaxMana.ToString();

        _shortHolder.SetActive(true);
    }

    //Turn off short character detail if it is active
    void UnHighlightCharacter()
    {
        if (_shortHolder.activeInHierarchy)
        {
            _shortHolder.SetActive(false);
        }
    }

    //show the long character details
    void ShowDetail(Tile clickedOnToken)
    {
        _interactState = UIInteractions.NONE;
        //Debug.Log("detail shown");

        HideActionsMenu();

        //get character info from GridHandler by sending GridToken Position
        Character clickedOnCharacter = GridHandler.RetrieveCharacter(clickedOnToken.GetXPosition, clickedOnToken.GetYPosition);

        _detailHolder.ShowUI(clickedOnCharacter);
        _battleCam.FocusObject(clickedOnToken.gameObject, CameraModes.ZOOMING);
    }

    //Turns off long character details
    void UnShowDetail()
    {
        //Debug.Log("detail unshown");
        _detailHolder.HideUI();
        _battleCam.ResetCamera();
    }
    
    //turn on character interaction UI
    void ShowActionsMenu(Tile chara)
    {
        _battleCam.FocusObject(chara.gameObject, CameraModes.MOVING);
        
        Character gridChar = GridHandler.RetrieveCharacter(chara.GetXPosition, chara.GetYPosition);
        if(gridChar.Team == TeamType.PLAYER && !_optionsRef.IsMoving)
        {
            _interactState = UIInteractions.MENUOPEN;
            _optionsRef.ShowUI(gridChar);
            _selectedCharacter = gridChar;
        }
        else
        {
            HideActionsMenu();
        }
    }
    
    //Turn off character Interaction UI
    public void HideActionsMenu()
    {
        //Debug.Log("hide called");
        _optionsRef.ResetUIMovements();
        _interactState = UIInteractions.FREE;
        _selectedCharacter = null;
        _selectedAbility = null;
    }
}
